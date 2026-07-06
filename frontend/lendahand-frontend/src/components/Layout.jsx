import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const adminLinks = [
  { to: '/admin', label: 'Overview' },
  { to: '/employees', label: 'Employees' },
  { to: '/tasks', label: 'Tasks' },
  { to: '/reports', label: 'Reports' },
  { to: '/notifications', label: 'Notifications' },
]

const employeeLinks = [
  { to: '/employee', label: 'Overview' },
  { to: '/tasks', label: 'My Tasks' },
  { to: '/notifications', label: 'Notifications' },
]

export default function Layout({ children }) {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const links = user?.role === 'Admin' ? adminLinks : employeeLinks

  const handleLogout = async () => {
    await logout()
    navigate('/login')
  }

  const initials = (user?.fullName || '?')
    .split(' ')
    .map(p => p[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">LA</div>
          <div className="brand-name">LendAHand</div>
        </div>

        <div className="nav-eyebrow">{user?.role === 'Admin' ? 'Administration' : 'Workspace'}</div>
        {links.map(link => (
          <NavLink
            key={link.to}
            to={link.to}
            className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}
          >
            {link.label}
          </NavLink>
        ))}

        <div className="sidebar-footer">
          <div className="user-chip">
            <div className="user-avatar">{initials}</div>
            <div className="user-meta">
              <div className="user-name">{user?.fullName}</div>
              <div className="user-role">{user?.role}</div>
            </div>
          </div>
          <button className="logout-btn" onClick={handleLogout}>Log out</button>
        </div>
      </aside>

      <main className="main">{children}</main>
    </div>
  )
}
