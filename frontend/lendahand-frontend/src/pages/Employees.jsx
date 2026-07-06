import { useEffect, useState } from 'react'
import Layout from '../components/Layout'
import api from '../api/axios'

const emptyForm = { name: '', email: '', department: '', designation: '', password: '' }

export default function Employees() {
  const [data, setData] = useState({ data: [], totalCount: 0, page: 1, pageSize: 10, totalPages: 1 })
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [modalOpen, setModalOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [form, setForm] = useState(emptyForm)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)

  const load = () => {
    setLoading(true)
    api.get('/employees', { params: { page, pageSize: 10, search: search || undefined } })
      .then(res => setData(res.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }

  useEffect(load, [page])

  const handleSearch = (e) => {
    e.preventDefault()
    setPage(1)
    load()
  }

  const openCreate = () => {
    setEditing(null)
    setForm(emptyForm)
    setFormError('')
    setModalOpen(true)
  }

  const openEdit = (emp) => {
    setEditing(emp)
    setForm({ name: emp.name, email: emp.email, department: emp.department, designation: emp.designation, password: '' })
    setFormError('')
    setModalOpen(true)
  }

  const handleChange = (e) => setForm(prev => ({ ...prev, [e.target.name]: e.target.value }))

  const handleSubmit = async (e) => {
    e.preventDefault()
    setSaving(true)
    setFormError('')
    try {
      if (editing) {
        await api.put(`/employees/${editing.id}`, {
          name: form.name, email: form.email, department: form.department, designation: form.designation
        })
      } else {
        await api.post('/employees', form)
      }
      setModalOpen(false)
      load()
    } catch (err) {
      const errs = err.response?.data?.errors
      setFormError(errs ? Object.values(errs).flat().join(', ') : (err.response?.data?.message || 'Save failed'))
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (emp) => {
    if (!confirm(`Remove ${emp.name}? This cannot be undone.`)) return
    await api.delete(`/employees/${emp.id}`)
    load()
  }

  return (
    <Layout>
      <div className="page-head">
        <div>
          <div className="page-eyebrow">Team</div>
          <h1 className="page-title">Employees</h1>
          <p className="page-sub">{data.totalCount} people across the organisation.</p>
        </div>
        <button className="btn btn-amber" onClick={openCreate}>+ Add employee</button>
      </div>

      <div className="toolbar">
        <form onSubmit={handleSearch} style={{ display: 'flex', gap: 8 }}>
          <input
            className="search-input"
            placeholder="Search by name, email, department..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <button className="btn btn-ghost btn-sm" type="submit">Search</button>
        </form>
      </div>

      <div className="card">
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Name</th><th>Email</th><th>Department</th><th>Designation</th><th>Joined</th><th></th>
              </tr>
            </thead>
            <tbody>
              {data.data.map(emp => (
                <tr key={emp.id}>
                  <td style={{ fontWeight: 700 }}>{emp.name}</td>
                  <td>{emp.email}</td>
                  <td>{emp.department}</td>
                  <td>{emp.designation}</td>
                  <td className="mono">{new Date(emp.createdAt).toLocaleDateString()}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 6 }}>
                      <button className="icon-btn" onClick={() => openEdit(emp)}>Edit</button>
                      <button className="icon-btn" onClick={() => handleDelete(emp)} style={{ color: 'var(--red)' }}>Delete</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {!loading && data.data.length === 0 && (
          <div className="empty-state">
            <div className="glyph">👥</div>
            No employees found. Add one to get started.
          </div>
        )}
      </div>

      {data.totalPages > 1 && (
        <div className="pagination">
          <button className="icon-btn" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>Prev</button>
          Page {data.page} of {data.totalPages}
          <button className="icon-btn" disabled={page >= data.totalPages} onClick={() => setPage(p => p + 1)}>Next</button>
        </div>
      )}

      {modalOpen && (
        <div className="modal-backdrop" onClick={() => setModalOpen(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-head">
              <h3 style={{ fontSize: 17 }}>{editing ? 'Edit employee' : 'Add employee'}</h3>
              <button className="modal-close" onClick={() => setModalOpen(false)}>×</button>
            </div>

            {formError && <div className="auth-error">{formError}</div>}

            <form onSubmit={handleSubmit}>
              <div className="field">
                <label>Full name</label>
                <input name="name" required value={form.name} onChange={handleChange} />
              </div>
              <div className="field">
                <label>Email</label>
                <input type="email" name="email" required value={form.email} onChange={handleChange} />
              </div>
              <div className="form-row">
                <div className="field">
                  <label>Department</label>
                  <input name="department" required value={form.department} onChange={handleChange} />
                </div>
                <div className="field">
                  <label>Designation</label>
                  <input name="designation" required value={form.designation} onChange={handleChange} />
                </div>
              </div>
              {!editing && (
                <div className="field">
                  <label>Password</label>
                  <input type="password" name="password" required value={form.password} onChange={handleChange} placeholder="Min 8 chars, upper+lower+number" />
                </div>
              )}
              <div style={{ display: 'flex', gap: 8, marginTop: 6 }}>
                <button className="btn btn-primary" type="submit" disabled={saving}>
                  {saving ? <span className="spinner" /> : (editing ? 'Save changes' : 'Create employee')}
                </button>
                <button className="btn btn-ghost" type="button" onClick={() => setModalOpen(false)}>Cancel</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </Layout>
  )
}
