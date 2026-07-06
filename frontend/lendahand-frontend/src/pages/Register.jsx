import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Register() {
  const { register, loading } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({
    fullName: '', email: '', password: '', confirmPassword: '', role: 'Employee'
  })
  const [error, setError] = useState('')

  const handleChange = (e) => {
    setForm(prev => ({ ...prev, [e.target.name]: e.target.value }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    if (form.password !== form.confirmPassword) {
      setError('Passwords do not match')
      return
    }
    try {
      const user = await register(form)
      navigate(user.role === 'Admin' ? '/admin' : '/employee')
    } catch (err) {
      const errs = err.response?.data?.errors
      const msg = errs ? Object.values(errs).flat().join(', ') : (err.response?.data?.message || 'Registration failed')
      setError(msg)
    }
  }

  return (
    <div className="auth-shell">
      <div className="auth-card">
        <div className="auth-brand">
          <div className="brand-mark">LA</div>
          <div className="brand-name" style={{ color: 'var(--ink)' }}>LendAHand</div>
        </div>
        <h1 className="auth-title">Create your account</h1>
        <p className="auth-sub">Register to access the task management system.</p>

        {error && <div className="auth-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="field">
            <label>Full name</label>
            <input name="fullName" required value={form.fullName} onChange={handleChange} placeholder="Jane Doe" />
          </div>
          <div className="field">
            <label>Email</label>
            <input type="email" name="email" required value={form.email} onChange={handleChange} placeholder="you@lendahandindia.org" />
          </div>
          <div className="field">
            <label>Password</label>
            <input type="password" name="password" required value={form.password} onChange={handleChange} placeholder="Min 8 chars, upper+lower+number" />
          </div>
          <div className="field">
            <label>Confirm password</label>
            <input type="password" name="confirmPassword" required value={form.confirmPassword} onChange={handleChange} />
          </div>
          <div className="field">
            <label>Role</label>
            <select name="role" value={form.role} onChange={handleChange}>
              <option value="Employee">Employee</option>
              <option value="Admin">Admin</option>
            </select>
          </div>
          <button className="btn btn-primary" type="submit" disabled={loading} style={{ width: '100%', justifyContent: 'center' }}>
            {loading ? <span className="spinner" /> : 'Create account'}
          </button>
        </form>

        <div className="auth-switch">
          Already have an account? <Link to="/login"><button type="button">Sign in</button></Link>
        </div>
      </div>
    </div>
  )
}
