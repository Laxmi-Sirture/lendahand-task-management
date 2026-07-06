import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Login() {
  const { login, loading } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '', rememberMe: false })
  const [error, setError] = useState('')

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target
    setForm(prev => ({ ...prev, [name]: type === 'checkbox' ? checked : value }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    try {
      const user = await login(form.email, form.password, form.rememberMe)
      navigate(user.role === 'Admin' ? '/admin' : '/employee')
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid email or password')
    }
  }

  return (
    <div className="auth-shell">
      <div className="auth-card">
        <div className="auth-brand">
          <div className="brand-mark">LA</div>
          <div className="brand-name" style={{ color: 'var(--ink)' }}>LendAHand</div>
        </div>
        <h1 className="auth-title">Welcome back</h1>
        <p className="auth-sub">Sign in to reach your dashboard and tasks.</p>

        {error && <div className="auth-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="field">
            <label>Email</label>
            <input
              type="email" name="email" required
              value={form.email} onChange={handleChange}
              placeholder="you@lendahandindia.org"
            />
          </div>
          <div className="field">
            <label>Password</label>
            <input
              type="password" name="password" required
              value={form.password} onChange={handleChange}
              placeholder="••••••••"
            />
          </div>
          <div className="checkbox-row">
            <input
              type="checkbox" name="rememberMe" id="rememberMe"
              checked={form.rememberMe} onChange={handleChange}
            />
            <label htmlFor="rememberMe" style={{ margin: 0 }}>Remember me</label>
          </div>
          <button className="btn btn-primary" type="submit" disabled={loading} style={{ width: '100%', justifyContent: 'center' }}>
            {loading ? <span className="spinner" /> : 'Sign in'}
          </button>
        </form>

        <div className="auth-switch">
          Don't have an account? <Link to="/register"><button type="button">Create one</button></Link>
        </div>
      </div>
    </div>
  )
}
