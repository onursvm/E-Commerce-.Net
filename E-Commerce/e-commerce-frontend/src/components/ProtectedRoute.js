import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { CircularProgress, Box } from '@mui/material';

const ProtectedRoute = ({ children, requireAdmin = false }) => {
  const { user, loading, isAdmin } = useAuth();

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
        <CircularProgress />
      </Box>
    );
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (requireAdmin && !isAdmin) {
    console.log('🔴 ProtectedRoute: Admin access denied for user:', { 
      userId: user.id, 
      userName: user.userName, 
      roles: user.roles, 
      isAdmin 
    });
    return <Navigate to="/" replace />;
  }

  return children;
};

export default ProtectedRoute; 