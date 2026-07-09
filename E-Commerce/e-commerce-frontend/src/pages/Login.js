import React, { useState } from 'react';
import { Container, TextField, Button, Typography, Box, Alert, CircularProgress } from '@mui/material';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(email, password);
      navigate('/');
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.response?.data?.Message || 'Giriş başarısız.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 6 }}>
      <Typography variant="h4" gutterBottom align="center">
        Giriş Yap
      </Typography>
      
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      
      <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
        <TextField
          fullWidth
          label="E-posta"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          margin="normal"
          required
          disabled={loading}
        />
        <TextField
          fullWidth
          type="password"
          label="Şifre"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          margin="normal"
          required
          disabled={loading}
        />
        <Button
          variant="contained"
          type="submit"
          fullWidth
          size="large"
          disabled={loading}
          sx={{ mt: 3 }}
        >
          {loading ? <CircularProgress size={24} /> : 'Giriş Yap'}
        </Button>
      </Box>
    </Container>
  );
};

export default Login; 