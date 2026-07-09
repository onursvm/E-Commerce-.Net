import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Grid,
  Card,
  CardContent,
  Typography,
  CardMedia,
  CircularProgress,
  Box,
  Alert,
  Button,
  Paper
} from '@mui/material';
import { Add as AddIcon, Dashboard as DashboardIcon, People as PeopleIcon, Home as HomeIcon } from '@mui/icons-material';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';

const PropertyList = () => {
  const [properties, setProperties] = useState([]);
  const [error, setError] = useState(null);
  const { user, isAdmin, loading: authLoading } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    fetchProperties();
  }, []);

  // Admin kullanıcısı giriş yaptıktan sonra admin dashboard'a yönlendir
  useEffect(() => {
    if (!authLoading && isAdmin && user) {
      console.log('🔴 Admin user detected, redirecting to admin dashboard...', { isAdmin, user });
      navigate('/admin/dashboard', { replace: true });
    }
  }, [isAdmin, user, navigate, authLoading]);

  const fetchProperties = async () => {
    try {
      const response = await api.get('/api/Property');
      const propertyData = response.data?.data || response.data?.Data || [];
      setProperties(propertyData);
    } catch (err) {
      setError('İlanlar yüklenirken hata oluştu');
      console.error('Fetch properties error:', err);
    } finally {
      // setLoading(false); // This line was removed as per the edit hint.
    }
  };

  const handlePropertyClick = (propertyId) => {
    navigate(`/property/${propertyId}`);
  };

  // Auth loading sırasında loading göster
  if (authLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  // Admin kullanıcısı ise admin dashboard'u göster
  if (isAdmin && user) {
    return (
      <Container sx={{ mt: 4 }}>
        <Paper sx={{ p: 4, textAlign: 'center', bgcolor: 'primary.light', color: 'white' }}>
          <Typography variant="h3" gutterBottom>
            🎉 Admin Kullanıcısı Hoş Geldiniz!
          </Typography>
          <Typography variant="h6" paragraph>
            Admin panelinize yönlendiriliyorsunuz...
          </Typography>
          <Box sx={{ mt: 3 }}>
            <Button
              variant="contained"
              size="large"
              startIcon={<DashboardIcon />}
              onClick={() => navigate('/admin/dashboard')}
              sx={{ mr: 2 }}
            >
              Admin Dashboard'a Git
            </Button>
            <Button
              variant="outlined"
              size="large"
              startIcon={<PeopleIcon />}
              onClick={() => navigate('/admin/users')}
              sx={{ mr: 2 }}
            >
              Kullanıcı Yönetimi
            </Button>
            <Button
              variant="outlined"
              size="large"
              startIcon={<HomeIcon />}
              onClick={() => navigate('/admin/properties')}
            >
              İlan Yönetimi
            </Button>
          </Box>
        </Paper>
      </Container>
    );
  }

  // Normal kullanıcılar için property list
  return (
    <Container sx={{ mt: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4" gutterBottom>
          Emlak İlanları
        </Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          {isAdmin && (
            <Button
              variant="contained"
              color="secondary"
              startIcon={<DashboardIcon />}
              onClick={() => navigate('/admin/dashboard')}
              size="large"
            >
              Admin Panel
            </Button>
          )}
          {user && (
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => navigate('/properties/create')}
            >
              İlan Ekle
            </Button>
          )}
        </Box>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {properties.length === 0 ? (
        <Box textAlign="center" py={4}>
          <Typography variant="h6" color="text.secondary">
            Henüz ilan bulunmuyor
          </Typography>
        </Box>
      ) : (
        <Grid container spacing={3}>
          {properties.map(property => (
            <Grid item xs={12} sm={6} md={4} key={property.id}>
              <Card 
                sx={{ cursor: 'pointer', height: '100%' }}
                onClick={() => handlePropertyClick(property.id)}
              >
                <CardMedia
                  component="img"
                  height="200"
                  image={property.imageUrl || 'https://via.placeholder.com/300x200?text=İlan+Resmi'}
                  alt={property.title}
                />
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {property.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    {property.description?.substring(0, 100)}...
                  </Typography>
                  <Typography variant="h6" color="primary" gutterBottom>
                    ₺{property.price?.toLocaleString()}
                  </Typography>
                  <Typography variant="body2" gutterBottom>
                    <strong>Konum:</strong> {property.location}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    <strong>Tür:</strong> {property.propertyType}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Container>
  );
};

export default PropertyList; 