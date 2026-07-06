import { useEffect, useRef, useState } from 'react'
import Layout from '../components/Layout'
import api from '../api/axios'
import { useAuth } from '../context/AuthContext'

function formatBytes(bytes) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

const emptyForm = {
  title: '', description: '', priority: 'Medium', status: 'Pending',
  startDate: '', dueDate: '', assignedEmployeeId: ''
}

const badgeClass = (status) => {
  if (status === 'Completed') return 'badge badge-completed'
  if (status === 'InProgress') return 'badge badge-inprogress'
  return 'badge badge-pending'
}

const priorityClass = (p) => {
  if (p === 'High') return 'badge badge-high'
  if (p === 'Medium') return 'badge badge-medium'
  return 'badge badge-low'
}

export default function Tasks() {
  const { user } = useAuth()
  const isAdmin = user?.role === 'Admin'

  const [tasks, setTasks] = useState([])
  const [employees, setEmployees] = useState([])
  const [loading, setLoading] = useState(true)
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [form, setForm] = useState(emptyForm)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)

  // Attachments
  const [filesTask, setFilesTask] = useState(null)
  const [taskFiles, setTaskFiles] = useState([])
  const [filesLoading, setFilesLoading] = useState(false)
  const [uploadError, setUploadError] = useState('')
  const [uploading, setUploading] = useState(false)
  const fileInputRef = useRef(null)

  const loadTasks = () => {
    setLoading(true)
    api.get('/tasks').then(res => setTasks(res.data)).finally(() => setLoading(false))
  }

  useEffect(() => {
    loadTasks()
    if (isAdmin) {
      api.get('/employees', { params: { page: 1, pageSize: 100 } })
        .then(res => setEmployees(res.data.data))
        .catch(() => {})
    }
  }, [])

  const openCreate = () => {
    setEditing(null)
    setForm(emptyForm)
    setFormError('')
    setModalOpen(true)
  }

  const openEdit = (task) => {
    setEditing(task)
    setForm({
      title: task.title,
      description: task.description || '',
      priority: task.priority,
      status: task.status,
      startDate: task.startDate?.slice(0, 10),
      dueDate: task.dueDate?.slice(0, 10),
      assignedEmployeeId: task.assignedEmployeeId
    })
    setFormError('')
    setModalOpen(true)
  }

  const handleChange = (e) => setForm(prev => ({ ...prev, [e.target.name]: e.target.value }))

  const handleSubmit = async (e) => {
    e.preventDefault()
    setSaving(true)
    setFormError('')
    try {
      const payload = { ...form, startDate: new Date(form.startDate).toISOString(), dueDate: new Date(form.dueDate).toISOString() }
      if (editing) {
        await api.put(`/tasks/${editing.id}`, payload)
      } else {
        await api.post('/tasks', payload)
      }
      setModalOpen(false)
      loadTasks()
    } catch (err) {
      const errs = err.response?.data?.errors
      setFormError(errs ? Object.values(errs).flat().join(', ') : (err.response?.data?.message || 'Save failed'))
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (task) => {
    if (!confirm(`Delete task "${task.title}"?`)) return
    await api.delete(`/tasks/${task.id}`)
    loadTasks()
  }

  // Quick status change for employees (and admins) without opening the full form
  const quickStatusChange = async (task, status) => {
    try {
      await api.put(`/tasks/${task.id}`, {
        title: task.title,
        description: task.description,
        priority: task.priority,
        status,
        startDate: task.startDate,
        dueDate: task.dueDate,
        assignedEmployeeId: task.assignedEmployeeId
      })
      loadTasks()
    } catch (err) {
      alert(err.response?.data?.message || 'Could not update status')
    }
  }

  const openFiles = async (task) => {
    setFilesTask(task)
    setUploadError('')
    setFilesLoading(true)
    try {
      const { data } = await api.get(`/tasks/${task.id}/files`)
      setTaskFiles(data)
    } catch {
      setTaskFiles([])
    } finally {
      setFilesLoading(false)
    }
  }

  const handleUpload = async (e) => {
    const file = e.target.files[0]
    if (!file) return
    setUploadError('')

    if (file.size > 5 * 1024 * 1024) {
      setUploadError('File size cannot exceed 5MB')
      if (fileInputRef.current) fileInputRef.current.value = ''
      return
    }
    const allowed = ['.pdf', '.jpg', '.jpeg', '.png']
    const ext = '.' + file.name.split('.').pop().toLowerCase()
    if (!allowed.includes(ext)) {
      setUploadError('Only PDF, JPG, PNG files are allowed')
      if (fileInputRef.current) fileInputRef.current.value = ''
      return
    }

    setUploading(true)
    const formData = new FormData()
    formData.append('file', file)
    try {
      await api.post(`/tasks/${filesTask.id}/files`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      })
      const { data } = await api.get(`/tasks/${filesTask.id}/files`)
      setTaskFiles(data)
    } catch (err) {
      setUploadError(err.response?.data?.message || 'Upload failed')
    } finally {
      setUploading(false)
      if (fileInputRef.current) fileInputRef.current.value = ''
    }
  }

  const handleDeleteFile = async (fileId) => {
    if (!confirm('Remove this attachment?')) return
    await api.delete(`/tasks/files/${fileId}`)
    const { data } = await api.get(`/tasks/${filesTask.id}/files`)
    setTaskFiles(data)
  }

  return (
    <Layout>
      <div className="page-head">
        <div>
          <div className="page-eyebrow">Work</div>
          <h1 className="page-title">{isAdmin ? 'All tasks' : 'My tasks'}</h1>
          <p className="page-sub">{tasks.length} task{tasks.length !== 1 ? 's' : ''} {isAdmin ? 'across the team' : 'assigned to you'}.</p>
        </div>
        {isAdmin && <button className="btn btn-amber" onClick={openCreate}>+ New task</button>}
      </div>

      <div className="card">
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Title</th>
                {isAdmin && <th>Assigned to</th>}
                <th>Priority</th>
                <th>Status</th>
                <th>Due</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {tasks.map(task => (
                <tr key={task.id}>
                  <td style={{ fontWeight: 700, maxWidth: 240 }}>{task.title}</td>
                  {isAdmin && <td>{task.assignedEmployeeName}</td>}
                  <td><span className={priorityClass(task.priority)}>{task.priority}</span></td>
                  <td><span className={badgeClass(task.status)}>{task.status}</span></td>
                  <td className="mono">{new Date(task.dueDate).toLocaleDateString()}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 6, alignItems: 'center' }}>
                      {task.status !== 'Completed' && (
                        <select
                          className="search-input"
                          style={{ minWidth: 0, padding: '5px 8px', fontSize: 12.5 }}
                          value={task.status}
                          onChange={(e) => quickStatusChange(task, e.target.value)}
                        >
                          <option value="Pending">Pending</option>
                          <option value="InProgress">In Progress</option>
                          <option value="Completed">Completed</option>
                        </select>
                      )}
                      <button className="icon-btn" onClick={() => openFiles(task)}>
                        Files{task.fileUploads?.length ? ` (${task.fileUploads.length})` : ''}
                      </button>
                      {isAdmin && <button className="icon-btn" onClick={() => openEdit(task)}>Edit</button>}
                      {isAdmin && <button className="icon-btn" onClick={() => handleDelete(task)} style={{ color: 'var(--red)' }}>Delete</button>}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {!loading && tasks.length === 0 && (
          <div className="empty-state">
            <div className="glyph">✓</div>
            No tasks {isAdmin ? 'yet. Create the first one.' : 'assigned to you right now.'}
          </div>
        )}
      </div>

      {modalOpen && (
        <div className="modal-backdrop" onClick={() => setModalOpen(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-head">
              <h3 style={{ fontSize: 17 }}>{editing ? 'Edit task' : 'New task'}</h3>
              <button className="modal-close" onClick={() => setModalOpen(false)}>×</button>
            </div>

            {formError && <div className="auth-error">{formError}</div>}

            <form onSubmit={handleSubmit}>
              <div className="field">
                <label>Title</label>
                <input name="title" required maxLength={255} value={form.title} onChange={handleChange} />
              </div>
              <div className="field">
                <label>Description</label>
                <textarea name="description" rows={3} value={form.description} onChange={handleChange} />
              </div>
              <div className="form-row">
                <div className="field">
                  <label>Priority</label>
                  <select name="priority" value={form.priority} onChange={handleChange}>
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                  </select>
                </div>
                <div className="field">
                  <label>Status</label>
                  <select name="status" value={form.status} onChange={handleChange}>
                    <option value="Pending">Pending</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                  </select>
                </div>
              </div>
              <div className="form-row">
                <div className="field">
                  <label>Start date</label>
                  <input type="date" name="startDate" required value={form.startDate} onChange={handleChange} />
                </div>
                <div className="field">
                  <label>Due date</label>
                  <input type="date" name="dueDate" required value={form.dueDate} onChange={handleChange} />
                </div>
              </div>
              <div className="field">
                <label>Assign to</label>
                <select name="assignedEmployeeId" required value={form.assignedEmployeeId} onChange={handleChange}>
                  <option value="">Select employee</option>
                  {employees.map(emp => (
                    <option key={emp.id} value={emp.id}>{emp.name} — {emp.department}</option>
                  ))}
                </select>
              </div>
              <div style={{ display: 'flex', gap: 8, marginTop: 6 }}>
                <button className="btn btn-primary" type="submit" disabled={saving}>
                  {saving ? <span className="spinner" /> : (editing ? 'Save changes' : 'Create task')}
                </button>
                <button className="btn btn-ghost" type="button" onClick={() => setModalOpen(false)}>Cancel</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {filesTask && (
        <div className="modal-backdrop" onClick={() => setFilesTask(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-head">
              <h3 style={{ fontSize: 17 }}>Attachments — {filesTask.title}</h3>
              <button className="modal-close" onClick={() => setFilesTask(null)}>×</button>
            </div>

            {uploadError && <div className="auth-error">{uploadError}</div>}

            <div style={{ marginBottom: 16 }}>
              <label className="btn btn-ghost btn-sm" style={{ cursor: 'pointer' }}>
                {uploading ? <span className="spinner dark" /> : '+ Upload file'}
                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".pdf,.jpg,.jpeg,.png"
                  onChange={handleUpload}
                  disabled={uploading}
                  style={{ display: 'none' }}
                />
              </label>
              <div className="page-sub" style={{ marginTop: 6 }}>PDF, JPG or PNG — max 5MB</div>
            </div>

            {filesLoading ? (
              <div style={{ textAlign: 'center', padding: 20 }}><span className="spinner dark" /></div>
            ) : taskFiles.length === 0 ? (
              <div className="empty-state" style={{ padding: 24 }}>
                <div className="glyph">📎</div>
                No files attached yet.
              </div>
            ) : (
              <div className="card" style={{ boxShadow: 'none', border: '1px solid var(--line)' }}>
                {taskFiles.map(f => (
                  <div key={f.id} className="notif-item" style={{ alignItems: 'center' }}>
                    <div style={{ flex: 1 }}>
                      <div className="notif-msg" style={{ fontWeight: 700 }}>{f.fileName}</div>
                      <div className="notif-time">{formatBytes(f.fileSize)} · {new Date(f.createdAt).toLocaleDateString()}</div>
                    </div>
                    <button className="icon-btn" onClick={() => handleDeleteFile(f.id)} style={{ color: 'var(--red)' }}>Delete</button>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
    </Layout>
  )
}
