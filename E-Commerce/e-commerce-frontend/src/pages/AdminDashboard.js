import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Grid,
  Paper,
  Box,
  Card,
  CardContent,
  Button,
  Alert,
  CircularProgress,
  Divider
} from '@mui/material';
import {
  People as PeopleIcon,
  Home as HomeIcon,
  Dashboard as DashboardIcon,
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import api from '../api/client';

const AdminDashboard = () => {
  const navigate = useNavigate();
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalProperties: 0,
    activeProperties: 0,
    totalValue: 0
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      const [usersRes, propertiesRes] = await Promise.all([
        api.get('/api/User'),          // Büyük harfli endpoint
        api.get('/api/Property')
      ]);

      const users = usersRes.data?.Data || usersRes.data?.data || [];
      const properties = propertiesRes.data?.Data || propertiesRes.data?.data || [];

      setStats({
        totalUsers: users.length,
        totalProperties: properties.length,
        activeProperties: properties.filter(p => p.isActive !== false).length,
        totalValue: properties.reduce((sum, p) => sum + (parseFloat(p.price) || 0), 0)
      });
    } catch (err) {
      console.error('Dashboard error:', err);
      setError('İstatistikler yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4 }}>
      <Box display="flex" alignItems="center" mb={4}>
        <DashboardIcon sx={{ fontSize: 40, mr: 2, color: 'primary.main' }} />
        <Typography variant="h3">Admin Dashboard</Typography>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center' }}>
            <PeopleIcon sx={{ fontSize: 40, color: 'primary.main', mb: 1 }} />
            <Typography variant="h4">{stats.totalUsers}</Typography>
            <Typography variant="body2" color="text.secondary">Toplam Kullanıcı</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center' }}>
            <HomeIcon sx={{ fontSize: 40, color: 'secondary.main', mb: 1 }} />
            <Typography variant="h4">{stats.totalProperties}</Typography>
            <Typography variant="body2" color="text.secondary">Toplam İlan</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center' }}>
            <HomeIcon sx={{ fontSize: 40, color: 'success.main', mb: 1 }} />
            <Typography variant="h4">{stats.activeProperties}</Typography>
            <Typography variant="body2" color="text.secondary">Aktif İlan</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center' }}>
            <Typography variant="h4" color="primary">₺{stats.totalValue.toLocaleString()}</Typography>
            <Typography variant="body2" color="text.secondary">Toplam Değer</Typography>
          </Paper>
        </Grid>
      </Grid>

      <Divider sx={{ my: 4 }} />

      {/* Admin Yetkileri */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>👥 Kullanıcı Yönetimi</Typography>
              <Button
                variant="contained"
                startIcon={<PeopleIcon />}
                onClick={() => navigate('/admin/users')}
                fullWidth
              >
                Kullanıcı Yönetimi
              </Button>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>🏠 İlan Yönetimi</Typography>
              <Button
                variant="contained"
                color="secondary"
                startIcon={<HomeIcon />}
                onClick={() => navigate('/admin/properties')}
                fullWidth
              >
                İlan Yönetimi
              </Button>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
};

export default AdminDashboard;
