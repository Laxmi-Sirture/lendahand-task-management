import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from './context/AuthContext'
import ProtectedRoute from './components/ProtectedRoute'

import Login from './pages/Login'
import Register from './pages/Register'
import AdminDashboard from './pages/AdminDashboard'
import EmployeeDashboard from './pages/EmployeeDashboard'
import Employees from './pages/Employees'
import Tasks from './pages/Tasks'
import Notifications from './pages/Notifications'
import Reports from './pages/Reports'

function HomeRedirect() {
  const { user } = useAuth()
  if (!user) return <Navigate to="/login" replace />
  return <Navigate to={user.role === 'Admin' ? '/admin' : '/employee'} replace />
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<HomeRedirect />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />

      <Route path="/admin" element={
        <ProtectedRoute allowedRoles={['Admin']}><AdminDashboard /></ProtectedRoute>
      } />
      <Route path="/employee" element={
        <ProtectedRoute allowedRoles={['Employee']}><EmployeeDashboard /></ProtectedRoute>
      } />
      <Route path="/employees" element={
        <ProtectedRoute allowedRoles={['Admin']}><Employees /></ProtectedRoute>
      } />
      <Route path="/tasks" element={
        <ProtectedRoute allowedRoles={['Admin', 'Employee']}><Tasks /></ProtectedRoute>
      } />
      <Route path="/reports" element={
        <ProtectedRoute allowedRoles={['Admin']}><Reports /></ProtectedRoute>
      } />
      <Route path="/notifications" element={
        <ProtectedRoute allowedRoles={['Admin', 'Employee']}><Notifications /></ProtectedRoute>
      } />

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
