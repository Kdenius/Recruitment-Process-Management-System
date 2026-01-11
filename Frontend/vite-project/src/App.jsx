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
import { Jobs } from './pages/job'
import { Candidates } from './pages/candidate'
import { CandidateLogin } from './pages/candidateLogin'
import { CandidateDashboard } from './pages/canDashboard'
import { Applications } from './pages/applications'
import { Interviews } from './pages/interviews'

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
        <Route path="candidate-login" element={<CandidateLogin/>}/>
{/* <Route path="/cdash" element={<Layout><CandidateDashboard/></Layout>} /> */}
        <Route path="interviews" element={<ProtectedRoute><Layout><Interviews/></Layout></ProtectedRoute>}/>
        <Route path="application" element={<ProtectedRoute allowedRoles={['Candidate', 'Reviewer', 'Recruiter']}><Layout><Applications/></Layout></ProtectedRoute>}/>
        <Route path="dashboard" element={<ProtectedRoute><Layout><Dashboard/></Layout></ProtectedRoute>}/>
        <Route path="users" element={<ProtectedRoute allowedRoles={'Admin'}><Layout><Users/></Layout></ProtectedRoute>}/>
        <Route path="candidates" element={<ProtectedRoute><Layout><Candidates/></Layout></ProtectedRoute>}/>
        <Route path="" element={<ProtectedRoute><Layout><Dashboard/></Layout></ProtectedRoute>}/>
        <Route path="jobs" element={<Layout><Jobs/></Layout>}/>
      </Routes>
    </Router>
    </AuthProvider>
  )
}

export default App