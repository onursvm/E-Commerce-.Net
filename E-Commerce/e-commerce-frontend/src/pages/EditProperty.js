import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Container, TextField, Button, Typography, Box, MenuItem, Grid } from '@mui/material';
import api from '../api/client';

const EditProperty = () => {
  const { id } = useParams();
  const [form, setForm] = useState({
    title: '', description: '', price: '', currency: 0, endDate: '', location: '', propertyTypeId: '', propertyStatusId: ''
  });
  const [types, setTypes] = useState([]);
  const [statuses, setStatuses] = useState([]);

    useEffect(() => {
        Promise.all([
            api.get(`/api/Property/${id}`),
            api.get('/api/PropertyType'),
            api.get('/api/PropertyStatus'),
        ]).then(([p, t, s]) => {
            console.log("Property API Response:", p.data);

            const data = p.data.Data || p.data.data || {};

            setForm({
                title: data.Title || '',
                description: data.Description || '',
                price: data.Price || '',
                currency: data.Currency ?? 0,
                endDate: data.EndDate ? data.EndDate.slice(0, 10) : '',
                location: data.Location || '',
                propertyTypeId: data.PropertyTypeId ?? '',
                propertyStatusId: data.PropertyStatusId ?? '',
            });

            setTypes(t.data.Data || t.data.data || []);
            setStatuses(s.data.Data || s.data.data || []);
        });
    }, [id]);



  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                Title: form.title,
                Description: form.description,
                Price: parseFloat(form.price),
                Currency: parseInt(form.currency, 10),
                EndDate: form.endDate ? new Date(form.endDate).toISOString() : new Date().toISOString(),
                Location: form.location,
                PropertyTypeId: parseInt(form.propertyTypeId, 10),
                PropertyStatusId: parseInt(form.propertyStatusId, 10),
            };

            console.log('Payload gönderiliyor:', payload); // Payload’u kontrol et
            await api.put(`/api/Property/${id}`, payload);
            window.location.href = '/my-properties';
        } catch (err) {
            console.error('Kaydetme hatası:', err.response?.data || err.message);
            alert('İlan kaydedilirken hata oluştu. Konsolu kontrol edin.');
        }
    };


  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h5" gutterBottom>İlan Düzenle</Typography>
      <Box component="form" onSubmit={handleSubmit}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}><TextField fullWidth name="title" label="Başlık" value={form.title} onChange={handleChange} /></Grid>
          <Grid item xs={12} md={6}><TextField fullWidth name="price" label="Fiyat" value={form.price} onChange={handleChange} /></Grid>
          <Grid item xs={12}><TextField fullWidth multiline rows={3} name="description" label="Açıklama" value={form.description} onChange={handleChange} /></Grid>
          <Grid item xs={12} md={6}><TextField fullWidth name="location" label="Lokasyon" value={form.location} onChange={handleChange} /></Grid>
          <Grid item xs={12} md={6}><TextField fullWidth name="endDate" type="date" label="Bitiş Tarihi" InputLabelProps={{ shrink: true }} value={form.endDate} onChange={handleChange} /></Grid>
          <Grid item xs={12} md={6}>
          <TextField select fullWidth name="propertyTypeId" label="Tür" value={form.propertyTypeId} onChange={handleChange}>
               {(types || []).map((t) => (<MenuItem key={t.Id} value={t.Id}>{t.Name}</MenuItem>))}
            </TextField>
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField select fullWidth name="propertyStatusId" label="Durum" value={form.propertyStatusId} onChange={handleChange}>
                {(statuses || []).map((s) => (<MenuItem key={s.Id} value={s.Id}>{s.Name}</MenuItem>))}
          </TextField>
          </Grid>
        </Grid>
        <Button sx={{ mt: 2 }} variant="contained" type="submit">Kaydet</Button>
      </Box>
    </Container>
  );
};

export default EditProperty; 