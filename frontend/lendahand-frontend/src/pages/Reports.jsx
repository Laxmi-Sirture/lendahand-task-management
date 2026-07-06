import { useEffect, useState } from 'react'
import Layout from '../components/Layout'
import api from '../api/axios'

const tabs = [
  { key: 'completed', label: 'Completed Tasks', endpoint: '/reports/completed-tasks', exportType: 'completed' },
  { key: 'pending', label: 'Pending Tasks', endpoint: '/reports/pending-tasks', exportType: 'pending' },
  { key: 'employee', label: 'Employee-wise', endpoint: '/reports/employee-wise', exportType: 'employee' },
]

const badgeClass = (status) => {
  if (status === 'Completed') return 'badge badge-completed'
  if (status === 'InProgress') return 'badge badge-inprogress'
  return 'badge badge-pending'
}

export default function Reports() {
  const [activeTab, setActiveTab] = useState('completed')
  const [rows, setRows] = useState([])
  const [loading, setLoading] = useState(true)
  const [exporting, setExporting] = useState('')

  const tab = tabs.find(t => t.key === activeTab)

  useEffect(() => {
    setLoading(true)
    api.get(tab.endpoint).then(res => setRows(res.data)).finally(() => setLoading(false))
  }, [activeTab])

  const handleExport = async (format) => {
    setExporting(format)
    try {
      const res = await api.get('/reports/export', {
        params: { type: format, reportType: tab.exportType },
        responseType: 'blob'
      })
      const ext = format === 'excel' ? 'xlsx' : 'csv'
      const url = window.URL.createObjectURL(new Blob([res.data]))
      const link = document.createElement('a')
      link.href = url
      link.download = `${tab.exportType}_report.${ext}`
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)
    } catch {
      alert('Export failed. Please try again.')
    } finally {
      setExporting('')
    }
  }

  return (
    <Layout>
      <div className="page-head">
        <div>
          <div className="page-eyebrow">Insights</div>
          <h1 className="page-title">Reports</h1>
          <p className="page-sub">Task and employee performance at a glance.</p>
        </div>
        <div style={{ display: 'flex', gap: 8 }}>
          <button className="btn btn-ghost" onClick={() => handleExport('csv')} disabled={!!exporting}>
            {exporting === 'csv' ? <span className="spinner dark" /> : 'Export CSV'}
          </button>
          <button className="btn btn-amber" onClick={() => handleExport('excel')} disabled={!!exporting}>
            {exporting === 'excel' ? <span className="spinner dark" /> : 'Export Excel'}
          </button>
        </div>
      </div>

      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        {tabs.map(t => (
          <button
            key={t.key}
            className={t.key === activeTab ? 'btn btn-primary btn-sm' : 'btn btn-ghost btn-sm'}
            onClick={() => setActiveTab(t.key)}
          >
            {t.label}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-wrap">
          {activeTab === 'employee' ? (
            <table>
              <thead>
                <tr>
                  <th>Employee</th><th>Department</th><th>Total</th><th>Completed</th><th>Pending</th><th>Overdue</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((r, i) => (
                  <tr key={i}>
                    <td style={{ fontWeight: 700 }}>{r.employeeName}</td>
                    <td>{r.department}</td>
                    <td className="mono">{r.totalTasks}</td>
                    <td className="mono">{r.completedTasks}</td>
                    <td className="mono">{r.pendingTasks}</td>
                    <td className="mono">{r.overdueTasks}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Title</th><th>Status</th><th>Priority</th><th>Employee</th><th>Start</th><th>Due</th>
                </tr>
              </thead>
              <tbody>
                {rows.map(r => (
                  <tr key={r.taskId}>
                    <td style={{ fontWeight: 700 }}>{r.title}</td>
                    <td><span className={badgeClass(r.status)}>{r.status}</span></td>
                    <td>{r.priority}</td>
                    <td>{r.assignedEmployeeName}</td>
                    <td className="mono">{new Date(r.startDate).toLocaleDateString()}</td>
                    <td className="mono">{new Date(r.dueDate).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {!loading && rows.length === 0 && (
          <div className="empty-state">
            <div className="glyph">📊</div>
            No data for this report yet.
          </div>
        )}
      </div>
    </Layout>
  )
}
