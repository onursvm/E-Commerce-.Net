import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { CssBaseline, Container } from '@mui/material';
import PropertyList from './pages/PropertyList';
import PropertyDetail from './pages/PropertyDetail';
import MyProperties from './pages/MyProperties';
import CreateProperty from './pages/CreateProperty';
import EditProperty from './pages/EditProperty';
import Login from './pages/Login';
import Register from './pages/Register';
import Profile from './pages/Profile';
import AdminDashboard from './pages/AdminDashboard';
import AdminUsers from './pages/AdminUsers';
import AdminProperties from './pages/AdminProperties';
import Debug from './pages/Debug';
import Navbar from './components/Navbar';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  return (
    <AuthProvider>
      <CssBaseline />
      <Router>
        <Navbar />
        <Container maxWidth="lg">
          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<PropertyList />} />
            <Route path="/property/:id" element={<PropertyDetail />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/debug" element={<Debug />} />
            
            {/* User Routes */}
            <Route path="/my-properties" element={<ProtectedRoute><MyProperties /></ProtectedRoute>} />
            <Route path="/properties/create" element={<ProtectedRoute><CreateProperty /></ProtectedRoute>} />
             <Route path="/edit-property/:id" element={<ProtectedRoute><EditProperty /></ProtectedRoute>} />
            <Route path="/profile" element={<ProtectedRoute><Profile /></ProtectedRoute>} />
            
            {/* Admin Routes */}
            <Route path="/admin/dashboard" element={<ProtectedRoute requireAdmin><AdminDashboard /></ProtectedRoute>} />
            <Route path="/admin/users" element={<ProtectedRoute requireAdmin><AdminUsers /></ProtectedRoute>} />
            <Route path="/admin/properties" element={<ProtectedRoute requireAdmin><AdminProperties /></ProtectedRoute>} />
          </Routes>
        </Container>
      </Router>
    </AuthProvider>
  );
}

export default App;
