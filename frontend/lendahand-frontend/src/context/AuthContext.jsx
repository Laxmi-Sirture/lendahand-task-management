import { createContext, useContext, useState } from 'react'
import api from '../api/axios'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem('user')
    return stored ? JSON.parse(stored) : null
  })
  const [loading, setLoading] = useState(false)

  const persistSession = (data) => {
    localStorage.setItem('accessToken', data.accessToken)
    localStorage.setItem('refreshToken', data.refreshToken)
    const userData = {
      fullName: data.fullName,
      email: data.email,
      role: data.role
    }
    localStorage.setItem('user', JSON.stringify(userData))
    setUser(userData)
    return userData
  }

  const login = async (email, password, rememberMe) => {
    setLoading(true)
    try {
      const { data } = await api.post('/auth/login', { email, password, rememberMe })
      return persistSession(data)
    } finally {
      setLoading(false)
    }
  }

  const register = async (payload) => {
    setLoading(true)
    try {
      const { data } = await api.post('/auth/register', payload)
      return persistSession(data)
    } finally {
      setLoading(false)
    }
  }

  const logout = async () => {
    try {
      await api.post('/auth/logout')
    } catch {
      // ignore — logging out locally regardless
    }
    localStorage.clear()
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)
