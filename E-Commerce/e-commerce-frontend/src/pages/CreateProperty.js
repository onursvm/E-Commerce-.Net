import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  TextField,
  Button,
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  CircularProgress
} from '@mui/material';
import api from '../api/client';

const CreateProperty = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [propertyTypes, setPropertyTypes] = useState([]);
  const [propertyStatuses, setPropertyStatuses] = useState([]);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    price: '',
    location: '',
    propertyTypeId: '',
    propertyStatusId: ''
  });

  useEffect(() => {
    fetchPropertyTypesAndStatuses();
  }, []);

  const fetchPropertyTypesAndStatuses = async () => {
    try {
      const [typesRes, statusesRes] = await Promise.all([
        api.get('/api/PropertyType'),
        api.get('/api/PropertyStatus')
      ]);

      console.log('PropertyTypes response:', typesRes.data);
      console.log('PropertyStatuses response:', statusesRes.data);

      // API response yapısını kontrol et
      let typesData, statusesData;
      
      if (typesRes.data?.data) {
        typesData = typesRes.data.data;
      } else if (typesRes.data?.Data) {
        typesData = typesRes.data.Data;
      } else if (typesRes.data?.Success && typesRes.data?.data) {
        typesData = typesRes.data.data;
      } else {
        typesData = typesRes.data;
      }

      if (statusesRes.data?.data) {
        statusesData = statusesRes.data.data;
      } else if (statusesRes.data?.Data) {
        statusesData = statusesRes.data.Data;
      } else if (statusesRes.data?.Success && statusesRes.data?.data) {
        statusesData = statusesRes.data.data;
      } else {
        statusesData = statusesRes.data;
      }

      setPropertyTypes(typesData || []);
      setPropertyStatuses(statusesData || []);
    } catch (err) {
      console.error('Fetch error:', err);
      setError('Emlak türleri ve durumları yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      await api.post('/api/Property', formData);
      navigate('/my-properties');
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.response?.data?.Message || 'İlan oluşturulurken hata oluştu';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        Yeni İlan Oluştur
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
        <TextField
          fullWidth
          name="title"
          label="İlan Başlığı"
          value={formData.title}
          onChange={handleChange}
          margin="normal"
          required
          disabled={loading}
        />

        <TextField
          fullWidth
          name="description"
          label="Açıklama"
          value={formData.description}
          onChange={handleChange}
          margin="normal"
          multiline
          rows={4}
          required
          disabled={loading}
        />

        <TextField
          fullWidth
          name="price"
          label="Fiyat"
          type="number"
          value={formData.price}
          onChange={handleChange}
          margin="normal"
          required
          disabled={loading}
        />

        <TextField
          fullWidth
          name="location"
          label="Konum"
          value={formData.location}
          onChange={handleChange}
          margin="normal"
          required
          disabled={loading}
        />

        <FormControl fullWidth margin="normal" required>
          <InputLabel>Emlak Türü</InputLabel>
          <Select
            name="propertyTypeId"
            value={formData.propertyTypeId}
            onChange={handleChange}
            disabled={loading}
          >
            {propertyTypes.map((type) => (
              <MenuItem key={type.id} value={type.id}>
                {type.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControl fullWidth margin="normal" required>
          <InputLabel>Durum</InputLabel>
          <Select
            name="propertyStatusId"
            value={formData.propertyStatusId}
            onChange={handleChange}
            disabled={loading}
          >
            {propertyStatuses.map((status) => (
              <MenuItem key={status.id} value={status.id}>
                {status.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={loading}
          >
            {loading ? <CircularProgress size={24} /> : 'İlan Oluştur'}
          </Button>
          <Button
            variant="outlined"
            size="large"
            onClick={() => navigate('/my-properties')}
            disabled={loading}
          >
            İptal
          </Button>
        </Box>
      </Box>
    </Container>
  );
};

export default CreateProperty; 