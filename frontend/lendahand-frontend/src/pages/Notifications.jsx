import { useEffect, useState } from 'react'
import Layout from '../components/Layout'
import api from '../api/axios'

function timeAgo(dateStr) {
  const diff = (Date.now() - new Date(dateStr).getTime()) / 1000
  if (diff < 60) return 'just now'
  if (diff < 3600) return `${Math.floor(diff / 60)}m ago`
  if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`
  return `${Math.floor(diff / 86400)}d ago`
}

export default function Notifications() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)

  const load = () => {
    setLoading(true)
    api.get('/notifications').then(res => setItems(res.data)).finally(() => setLoading(false))
  }

  useEffect(load, [])

  const markRead = async (id) => {
    await api.put(`/notifications/${id}/read`)
    load()
  }

  const markAllRead = async () => {
    await api.put('/notifications/read-all')
    load()
  }

  const unreadCount = items.filter(n => !n.isRead).length

  return (
    <Layout>
      <div className="page-head">
        <div>
          <div className="page-eyebrow">Inbox</div>
          <h1 className="page-title">Notifications</h1>
          <p className="page-sub">{unreadCount} unread of {items.length} total.</p>
        </div>
        {unreadCount > 0 && <button className="btn btn-ghost" onClick={markAllRead}>Mark all as read</button>}
      </div>

      <div className="card">
        {items.map(n => (
          <div className="notif-item" key={n.id}>
            <div className={`notif-dot${n.isRead ? ' read' : ''}`} />
            <div style={{ flex: 1 }}>
              <div className="notif-msg">{n.message}</div>
              <div className="notif-time">{timeAgo(n.createdAt)}</div>
            </div>
            {!n.isRead && (
              <button className="icon-btn" onClick={() => markRead(n.id)}>Mark read</button>
            )}
          </div>
        ))}

        {!loading && items.length === 0 && (
          <div className="empty-state">
            <div className="glyph">🔔</div>
            You're all caught up. Nothing here yet.
          </div>
        )}
      </div>
    </Layout>
  )
}
