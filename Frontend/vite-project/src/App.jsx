import React from 'react'
import { BrowserRouter as Router, Navigate, Route, Routes } from 'react-router-dom'
import { Login } from './pages/login'
import { Signup } from './pages/signup'
import { AuthProvider } from './context/AuthContext'
import { Toaster } from 'react-hot-toast'
import Verify from './pages/verify'
import { Dashboard } from './pages/dashboard'
import { Layout } from './components/layout'

function App() {
  return (
    <AuthProvider>
    <Toaster/>
    <Router>
      <Routes>
        <Route path="verify/:token/:email" element={<Verify/>}/>
        <Route path="login" element={<Login/>}/>
        <Route path="signup" element={<Signup/>}/>

        <Route path="dashboard" element={<Layout><Dashboard/></Layout>}/>

      </Routes>
    </Router>
    </AuthProvider>
  )
}

export default App