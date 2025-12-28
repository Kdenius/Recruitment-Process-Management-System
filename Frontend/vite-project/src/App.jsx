import React from 'react'
import { BrowserRouter as Router, Navigate, Route, Routes } from 'react-router-dom'
import { Login } from './pages/login'
import { Signup } from './pages/signup'
import { AuthProvider } from './context/AuthContext'
import { Toaster } from 'react-hot-toast'
import Verify from './pages/verify'
import { Dashboard } from './pages/dashboard'
import { Layout } from './components/layout'
import ProtectedRoute from './components/protectedRoute'
import { AdminLogin } from './pages/adminLogin'
import { Users } from './pages/users'

function App() {
  return (
    <AuthProvider>
    <Toaster/>
    <Router>
      <Routes>
        <Route path="verify/:token/:email" element={<Verify/>}/>
        <Route path="login" element={<Login/>}/>
        <Route path="signup" element={<Signup/>}/>
        <Route path="admin-login" element={<AdminLogin/>}/>

        <Route path="dashboard" element={<ProtectedRoute><Layout><Dashboard/></Layout></ProtectedRoute>}/>
        <Route path="users" element={<ProtectedRoute allowedRoles={'Admin'}><Layout><Users/></Layout></ProtectedRoute>}/>
        <Route path="" element={<ProtectedRoute><Layout><Dashboard/></Layout></ProtectedRoute>}/>
      </Routes>
    </Router>
    </AuthProvider>
  )
}

export default App