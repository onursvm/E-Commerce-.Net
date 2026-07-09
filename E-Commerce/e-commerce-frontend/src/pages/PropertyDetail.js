import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  Container, 
  Typography, 
  Card, 
  CardMedia, 
  CardContent, 
  Grid, 
  Box, 
  Button,
  CircularProgress,
  Alert
} from '@mui/material';
import { ArrowBack as ArrowBackIcon } from '@mui/icons-material';
import api from '../api/client';

const PropertyDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [property, setProperty] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProperty = async () => {
      try {
        const response = await api.get(`/api/Property/${id}`);
        console.log('Property response:', response.data);
        
        // API response yapısını kontrol et
        let propertyData;
        if (response.data?.data) {
          propertyData = response.data.data;
        } else if (response.data?.Data) {
          propertyData = response.data.Data;
        } else if (response.data?.success && response.data?.data) {
          propertyData = response.data.data;
        } else {
          propertyData = response.data;
        }
        
        setProperty(propertyData);
      } catch (err) {
        console.error('Property detail error:', err);
        setError('İlan detayları yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message));
      } finally {
        setLoading(false);
      }
    };

    fetchProperty();
  }, [id]);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Container sx={{ mt: 4 }}>
        <Alert severity="error">{error}</Alert>
        <Button 
          startIcon={<ArrowBackIcon />} 
          onClick={() => navigate('/')}
          sx={{ mt: 2 }}
        >
          Ana Sayfaya Dön
        </Button>
      </Container>
    );
  }

  if (!property) {
    return (
      <Container sx={{ mt: 4 }}>
        <Alert severity="warning">İlan bulunamadı</Alert>
        <Button 
          startIcon={<ArrowBackIcon />} 
          onClick={() => navigate('/')}
          sx={{ mt: 2 }}
        >
          Ana Sayfaya Dön
        </Button>
      </Container>
    );
  }

  return (
    <Container sx={{ mt: 4 }}>
      <Button 
        startIcon={<ArrowBackIcon />} 
        onClick={() => navigate('/')}
        sx={{ mb: 2 }}
      >
        Geri Dön
      </Button>

      <Card>
        <CardMedia
          component="img"
          height="400"
          image={property.imageUrl || 'https://via.placeholder.com/600x400?text=İlan+Resmi'}
          alt={property.title}
        />
        <CardContent>
          <Typography variant="h4" gutterBottom>
            {property.title}
          </Typography>
          
          <Typography variant="h5" color="primary" gutterBottom>
            ₺{property.price?.toLocaleString()}
          </Typography>
          
          <Typography variant="body1" paragraph>
            {property.description}
          </Typography>
          
          <Grid container spacing={2} sx={{ mt: 2 }}>
            <Grid item xs={12} sm={6}>
              <Typography variant="subtitle1" color="text.secondary">
                <strong>Konum:</strong> {property.location}
              </Typography>
            </Grid>
            <Grid item xs={12} sm={6}>
              <Typography variant="subtitle1" color="text.secondary">
                <strong>Emlak Türü:</strong> {property.propertyTypeName || property.propertyType}
              </Typography>
            </Grid>
            <Grid item xs={12} sm={6}>
              <Typography variant="subtitle1" color="text.secondary">
                <strong>Durum:</strong> {property.propertyStatusName || property.propertyStatus}
              </Typography>
            </Grid>
            {property.startDate && (
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle1" color="text.secondary">
                  <strong>İlan Tarihi:</strong> {new Date(property.startDate).toLocaleDateString('tr-TR')}
                </Typography>
              </Grid>
            )}
          </Grid>
        </CardContent>
      </Card>
    </Container>
  );
};

export default PropertyDetail; 