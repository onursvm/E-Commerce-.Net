import React, { useState } from 'react';
import { Container, TextField, Button, Typography, Box, Alert, CircularProgress } from '@mui/material';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const Register = () => {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ userName: '', email: '', password: '', fullName: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await register(form);
      navigate('/');
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.response?.data?.Message || 'Kayıt başarısız.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 6 }}>
      <Typography variant="h4" gutterBottom align="center">
        Kayıt Ol
      </Typography>
      
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      
      <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
        <TextField
          fullWidth
          name="userName"
          label="Kullanıcı Adı"
          value={form.userName}
          onChange={handleChange}
          margin="normal"
          required
          disabled={loading}
        />
        <TextField
          fullWidth
          name="fullName"
          label="Ad Soyad"
          value={form.fullName}
          onChange={handleChange}
          margin="normal"
          required
          disabled={loading}
        />
        <TextField
          fullWidth
          name="email"
          label="E-posta"
          type="email"
          value={form.email}
          onChange={handleChange}
          margin="normal"
          required
          disabled={loading}
        />
        <TextField
          fullWidth
          name="password"
          type="password"
          label="Şifre"
          value={form.password}
          onChange={handleChange}
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
          {loading ? <CircularProgress size={24} /> : 'Kayıt Ol'}
        </Button>
      </Box>
    </Container>
  );
};

export default Register; 