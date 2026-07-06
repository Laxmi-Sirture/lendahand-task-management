import { useEffect, useState } from 'react'
import Layout from '../components/Layout'
import api from '../api/axios'
import { useAuth } from '../context/AuthContext'

export default function AdminDashboard() {
  const { user } = useAuth()
  const [stats, setStats] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    api.get('/dashboard/admin')
      .then(res => setStats(res.data))
      .catch(() => setError('Could not load dashboard stats.'))
  }, [])

  return (
    <Layout>
      <div className="page-head">
        <div>
          <div className="page-eyebrow">Overview</div>
          <h1 className="page-title">Good to see you, {user?.fullName?.split(' ')[0]}</h1>
          <p className="page-sub">Here's how the team is tracking today.</p>
        </div>
      </div>

      {error && <div className="auth-error">{error}</div>}

      {stats && (
        <div className="stat-grid">
          <div className="stat-card">
            <div className="stat-num">{stats.totalEmployees}</div>
            <div className="stat-label">Total employees</div>
          </div>
          <div className="stat-card accent">
            <div className="stat-num">{stats.totalTasks}</div>
            <div className="stat-label">Total tasks</div>
          </div>
          <div className="stat-card good">
            <div className="stat-num">{stats.completedTasks}</div>
            <div className="stat-label">Completed</div>
          </div>
          <div className="stat-card">
            <div className="stat-num">{stats.inProgressTasks}</div>
            <div className="stat-label">In progress</div>
          </div>
          <div className="stat-card bad">
            <div className="stat-num">{stats.pendingTasks}</div>
            <div className="stat-label">Pending</div>
          </div>
        </div>
      )}

      <div className="card card-pad">
        <h3 style={{ fontSize: 15, marginBottom: 8 }}>Quick actions</h3>
        <p className="page-sub" style={{ marginBottom: 14 }}>Jump straight into managing your team.</p>
        <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}>
          <a className="btn btn-primary" href="/employees">Manage employees</a>
          <a className="btn btn-ghost" href="/tasks">View all tasks</a>
        </div>
      </div>
    </Layout>
  )
}
