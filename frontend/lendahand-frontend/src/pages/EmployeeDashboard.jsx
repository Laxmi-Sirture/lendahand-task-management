import { useEffect, useState } from 'react'
import Layout from '../components/Layout'
import api from '../api/axios'
import { useAuth } from '../context/AuthContext'

export default function EmployeeDashboard() {
  const { user } = useAuth()
  const [stats, setStats] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    api.get('/dashboard/employee')
      .then(res => setStats(res.data))
      .catch(() => setError('Could not load your dashboard. If this is your first login, ask an Admin to add you as an employee record.'))
  }, [])

  return (
    <Layout>
      <div className="page-head">
        <div>
          <div className="page-eyebrow">Overview</div>
          <h1 className="page-title">Welcome, {user?.fullName?.split(' ')[0]}</h1>
          <p className="page-sub">A snapshot of your assigned work.</p>
        </div>
      </div>

      {error && <div className="auth-error">{error}</div>}

      {stats && (
        <div className="stat-grid">
          <div className="stat-card">
            <div className="stat-num">{stats.myTasks}</div>
            <div className="stat-label">My tasks</div>
          </div>
          <div className="stat-card good">
            <div className="stat-num">{stats.completedTasks}</div>
            <div className="stat-label">Completed</div>
          </div>
          <div className="stat-card accent">
            <div className="stat-num">{stats.pendingTasks}</div>
            <div className="stat-label">Pending</div>
          </div>
          <div className="stat-card bad">
            <div className="stat-num">{stats.overdueTasks}</div>
            <div className="stat-label">Overdue</div>
          </div>
        </div>
      )}

      <div className="card card-pad">
        <h3 style={{ fontSize: 15, marginBottom: 8 }}>Quick actions</h3>
        <p className="page-sub" style={{ marginBottom: 14 }}>Check what's on your plate.</p>
        <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}>
          <a className="btn btn-primary" href="/tasks">View my tasks</a>
          <a className="btn btn-ghost" href="/notifications">Notifications</a>
        </div>
      </div>
    </Layout>
  )
}
