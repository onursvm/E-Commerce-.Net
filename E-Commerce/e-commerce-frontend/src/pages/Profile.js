import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Paper,
  Box,
  TextField,
  Button,
  Grid,
  Avatar,
  Divider,
  Alert,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions
} from '@mui/material';
import {
  Edit as EditIcon,
  Save as SaveIcon,
  Cancel as CancelIcon,
  Lock as LockIcon
} from '@mui/icons-material';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';

const Profile = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [changePasswordDialog, setChangePasswordDialog] = useState(false);
  const [formData, setFormData] = useState({
    userName: '',
    email: '',
    fullName: ''
  });
  const [passwordForm, setPasswordForm] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });

  useEffect(() => {
    if (user) {
      setFormData({
        userName: user.userName || '',
        email: user.email || '',
        fullName: user.fullName || ''
      });
    }
  }, [user]);

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleCancel = () => {
    setIsEditing(false);
    setFormData({
      userName: user.userName || '',
      email: user.email || '',
      fullName: user.fullName || ''
    });
  };

  const handleSave = async () => {
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      console.log('Updating profile with data:', formData);
      const response = await api.put('/api/Auth/profile', formData);
      console.log('Profile update response:', response.data);
      
      setSuccess('Profil başarıyla güncellendi');
      setIsEditing(false);
    } catch (err) {
      console.error('Update profile error:', err);
      const errorMessage = err.response?.data?.message || err.response?.data?.Message || 'Profil güncellenirken hata oluştu';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleChangePassword = async () => {
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      setError('Yeni şifreler eşleşmiyor');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      await api.post('/api/Auth/change-password', {
        currentPassword: passwordForm.currentPassword,
        newPassword: passwordForm.newPassword,
        confirmPassword: passwordForm.confirmPassword
      });
      setSuccess('Şifre başarıyla değiştirildi');
      setChangePasswordDialog(false);
      setPasswordForm({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      });
    } catch (err) {
      setError('Şifre değiştirilirken hata oluştu');
      console.error('Change password error:', err);
    } finally {
      setLoading(false);
    }
  };

  if (!user) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Alert severity="error">Kullanıcı bilgileri yüklenemedi</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        Profil Bilgileri
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}

      <Paper sx={{ p: 3 }}>
        <Box display="flex" alignItems="center" mb={3}>
          <Avatar sx={{ width: 80, height: 80, mr: 2 }}>
            {user.userName?.charAt(0)?.toUpperCase()}
          </Avatar>
          <Box>
            <Typography variant="h5">{user.fullName}</Typography>
            <Typography color="text.secondary">@{user.userName}</Typography>
          </Box>
        </Box>

        <Divider sx={{ mb: 3 }} />

        <Grid container spacing={3}>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Kullanıcı Adı"
              value={formData.userName}
              onChange={(e) => setFormData({ ...formData, userName: e.target.value })}
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="E-posta"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12}>
            <TextField
              fullWidth
              label="Ad Soyad"
              value={formData.fullName}
              onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
        </Grid>

        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
          {!isEditing ? (
            <>
              <Button
                variant="outlined"
                startIcon={<EditIcon />}
                onClick={handleEdit}
              >
                Düzenle
              </Button>
              <Button
                variant="outlined"
                startIcon={<LockIcon />}
                onClick={() => setChangePasswordDialog(true)}
              >
                Şifre Değiştir
              </Button>
            </>
          ) : (
            <>
              <Button
                variant="contained"
                startIcon={<SaveIcon />}
                onClick={handleSave}
                disabled={loading}
              >
                {loading ? <CircularProgress size={20} /> : 'Kaydet'}
              </Button>
              <Button
                variant="outlined"
                startIcon={<CancelIcon />}
                onClick={handleCancel}
                disabled={loading}
              >
                İptal
              </Button>
            </>
          )}
        </Box>
      </Paper>

      {/* Change Password Dialog */}
      <Dialog open={changePasswordDialog} onClose={() => setChangePasswordDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Şifre Değiştir</DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 2 }}>
            <TextField
              fullWidth
              label="Mevcut Şifre"
              type="password"
              value={passwordForm.currentPassword}
              onChange={(e) => setPasswordForm({ ...passwordForm, currentPassword: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Yeni Şifre"
              type="password"
              value={passwordForm.newPassword}
              onChange={(e) => setPasswordForm({ ...passwordForm, newPassword: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Yeni Şifre (Tekrar)"
              type="password"
              value={passwordForm.confirmPassword}
              onChange={(e) => setPasswordForm({ ...passwordForm, confirmPassword: e.target.value })}
              margin="normal"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setChangePasswordDialog(false)}>İptal</Button>
          <Button onClick={handleChangePassword} variant="contained" disabled={loading}>
            {loading ? <CircularProgress size={20} /> : 'Değiştir'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default Profile; 