import React, { useState, useEffect } from 'react';
import {
    Container, Typography, Grid, Card, CardMedia, CardContent,
    CardActions, Button, IconButton, Switch, Alert, CircularProgress,
    Box, Chip, Dialog, DialogTitle, DialogContent, DialogActions,
    TextField, FormControl, InputLabel, Select, MenuItem
} from '@mui/material';
import { Edit as EditIcon, Delete as DeleteIcon, Visibility as ViewIcon, Add as AddIcon } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import api from '../api/client';
import { useAuth } from '../context/AuthContext';

const AdminProperties = () => {
    const navigate = useNavigate();
    const { user } = useAuth();
    const [properties, setProperties] = useState([]);
    const [propertyTypes, setPropertyTypes] = useState([]);
    const [propertyStatuses, setPropertyStatuses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [openCreateDialog, setOpenCreateDialog] = useState(false);
    const [createPropertyData, setCreatePropertyData] = useState({
        title: '',
        description: '',
        price: '',
        location: '',
        propertyTypeId: '',
        propertyStatusId: '',
        endDate: '',
        photoUrl: ''
    });

    useEffect(() => {
        fetchProperties();
        fetchPropertyTypesAndStatuses();
    }, []);

    const fetchProperties = async () => {
        setLoading(true);
        try {
            const response = await api.get('/api/Property');
            const propertiesData = response.data?.Data || response.data?.data || [];
            setProperties(propertiesData);
        } catch (err) {
            setError('İlanlar yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message));
        } finally {
            setLoading(false);
        }
    };

    const fetchPropertyTypesAndStatuses = async () => {
        try {
            const [typesRes, statusesRes] = await Promise.all([
                api.get('/api/PropertyType'),
                api.get('/api/PropertyStatus')
            ]);
            setPropertyTypes(typesRes.data?.Data || []);
            setPropertyStatuses(statusesRes.data?.Data || []);
        } catch (err) {
            console.error('Fetch types/statuses error:', err);
        }
    };

    const handleToggleStatus = async (propertyId, currentStatus) => {
        try {
            await api.put(`/api/Property/${propertyId}/toggle-status`, { isActive: !currentStatus });
            fetchProperties();
        } catch (err) {
            setError('İlan durumu güncellenirken hata oluştu: ' + (err.response?.data?.message || err.message));
        }
    };

    const handleDeleteProperty = async (propertyId) => {
        if (!propertyId) return;
        if (!window.confirm('Bu ilanı silmek istediğinizden emin misiniz?')) return;
        try {
            await api.delete(`/api/Property/${propertyId}`);
            fetchProperties();
        } catch (err) {
            setError('İlan silinirken hata oluştu: ' + (err.response?.data?.message || err.message));
        }
    };

    const handleCreateProperty = async () => {
        if (!user || !user.id) {
            setError('Kullanıcı bilgisi alınamadı. Lütfen tekrar giriş yapın.');
            return;
        }

        if (!createPropertyData.title.trim() || !createPropertyData.description.trim() || !createPropertyData.location.trim()) {
            setError('Başlık, açıklama ve konum boş olamaz.');
            return;
        }

        if (createPropertyData.description.trim().length < 10) {
            setError('Açıklama en az 10 karakter olmalıdır.');
            return;
        }

        const price = Number(createPropertyData.price);
        if (!price || price <= 0) {
            setError('Lütfen geçerli bir fiyat girin.');
            return;
        }

        if (!createPropertyData.propertyTypeId || !createPropertyData.propertyStatusId) {
            setError('Lütfen emlak türü ve ilan durumu seçin.');
            return;
        }

        // Payload JSON olarak gönderilecek
        const payload = {
            Title: createPropertyData.title.trim(),
            Description: createPropertyData.description.trim(),
            Location: createPropertyData.location.trim(),
            Price: price,
            Currency: 0,
            UserId: Number(user.id),
            PropertyTypeId: parseInt(createPropertyData.propertyTypeId),
            PropertyStatusId: parseInt(createPropertyData.propertyStatusId),
            EndDate: createPropertyData.endDate
                ? new Date(createPropertyData.endDate).toISOString()
                : new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
            Photos: createPropertyData.photoUrl
                ? [{ ImageUrl: createPropertyData.photoUrl, IsMain: true }]
                : []
        };

        try {
            const response = await api.post('/api/Property', payload, {
                headers: { 'Content-Type': 'application/json' }
            });

            console.log('İlan oluşturuldu:', response.data);

            setOpenCreateDialog(false);
            setCreatePropertyData({
                title: '',
                description: '',
                price: '',
                location: '',
                propertyTypeId: '',
                propertyStatusId: '',
                endDate: '',
                photoUrl: ''
            });

            fetchProperties();
        } catch (err) {
            setError('İlan oluşturulurken hata oluştu: ' + JSON.stringify(err.response?.data || err.message));
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
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
                <Typography variant="h4">Tüm İlanlar</Typography>
                <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpenCreateDialog(true)}>Yeni İlan Ekle</Button>
            </Box>

            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

            <Grid container spacing={3}>
                {properties.map(property => (
                    <Grid item xs={12} sm={6} md={4} key={property.id}>
                        <Card>
                            <CardMedia
                                component="img"
                                height="200"
                                image={property.photos?.[0]?.url || 'https://via.placeholder.com/400x200?text=İlan+Resmi'}
                                alt={property.title}
                            />
                            <CardContent>
                                <Typography variant="h6">{property.title}</Typography>
                                <Typography variant="body2" color="text.secondary">{property.description?.substring(0, 100)}...</Typography>
                                <Typography variant="h6" color="primary">₺{property.price?.toLocaleString()}</Typography>
                                <Box sx={{ mb: 1 }}>
                                    <Chip label={property.propertyTypeName || property.propertyType} size="small" color="primary" sx={{ mr: 1 }} />
                                    <Chip label={property.propertyStatusName || property.propertyStatus} size="small" color="secondary" />
                                </Box>
                                <Typography variant="body2"><strong>Konum:</strong> {property.location}</Typography>
                                <Typography variant="body2" color="text.secondary">
                                    <strong>Durum:</strong>
                                    <Switch checked={property.isActive !== false} onChange={() => handleToggleStatus(property.id, property.isActive)} size="small" sx={{ ml: 1 }} />
                                </Typography>
                            </CardContent>
                            <CardActions>
                                <IconButton color="primary" onClick={() => navigate(`/properties/${property.Id || property.id}/edit`)}><ViewIcon /></IconButton>
                                <IconButton color="primary" onClick={() => navigate(`/edit-property/${property.Id || property.id}`)}><EditIcon /></IconButton>
                                <IconButton color="error" onClick={() => handleDeleteProperty(property.Id || property.id)}><DeleteIcon /></IconButton>
                            </CardActions>
                        </Card>
                    </Grid>
                ))}
            </Grid>

            {properties.length === 0 && (
                <Box textAlign="center" sx={{ mt: 4 }}>
                    <Typography variant="h6" color="text.secondary">Henüz ilan bulunmuyor</Typography>
                </Box>
            )}

            <Dialog open={openCreateDialog} onClose={() => setOpenCreateDialog(false)} maxWidth="md" fullWidth>
                <DialogTitle>Yeni İlan Ekle</DialogTitle>
                <DialogContent>
                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
                        <TextField label="İlan Başlığı" fullWidth value={createPropertyData.title} onChange={(e) => setCreatePropertyData({ ...createPropertyData, title: e.target.value })} required />
                        <TextField label="Açıklama" multiline rows={4} fullWidth value={createPropertyData.description} onChange={(e) => setCreatePropertyData({ ...createPropertyData, description: e.target.value })} required />
                        <Grid container spacing={2}>
                            <Grid item xs={12} sm={6}>
                                <TextField label="Fiyat" type="number" fullWidth value={createPropertyData.price} onChange={(e) => setCreatePropertyData({ ...createPropertyData, price: e.target.value })} required />
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <TextField label="Konum" fullWidth value={createPropertyData.location} onChange={(e) => setCreatePropertyData({ ...createPropertyData, location: e.target.value })} required />
                            </Grid>
                        </Grid>
                        <Grid container spacing={2}>
                            <Grid item xs={12} sm={6}>
                                <FormControl fullWidth>
                                    <InputLabel>Emlak Türü</InputLabel>
                                    <Select value={createPropertyData.propertyTypeId} onChange={(e) => setCreatePropertyData({ ...createPropertyData, propertyTypeId: e.target.value })}>
                                        {propertyTypes.map(type => (
                                            <MenuItem key={type.Id} value={type.Id}>{type.Name}</MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <FormControl fullWidth>
                                    <InputLabel>İlan Durumu</InputLabel>
                                    <Select value={createPropertyData.propertyStatusId} onChange={(e) => setCreatePropertyData({ ...createPropertyData, propertyStatusId: e.target.value })}>
                                        {propertyStatuses.map(status => (
                                            <MenuItem key={status.Id} value={status.Id}>{status.Name}</MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                            </Grid>
                        </Grid>
                        <Box>
                            <TextField
                                label="Fotoğraf URL"
                                fullWidth
                                value={createPropertyData.photoUrl}
                                onChange={(e) => setCreatePropertyData({ ...createPropertyData, photoUrl: e.target.value })}
                                placeholder="https://example.com/photo.jpg"
                            />
                        </Box>
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setOpenCreateDialog(false)}>İptal</Button>
                    <Button variant="contained" onClick={handleCreateProperty}>İlan Ekle</Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default AdminProperties;
