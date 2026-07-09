import React, { useState, useEffect } from 'react';
import { Container, Typography, Paper, Box, Button, Alert, Divider, TextField } from '@mui/material';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';

const Debug = () => {
  const { user, isAdmin, isUser, accessToken, loading } = useAuth();
  const [loginResult, setLoginResult] = useState(null);
  const [profileResult, setProfileResult] = useState(null);
  const [testEmail, setTestEmail] = useState('admin@emlak.com');
  const [testPassword, setTestPassword] = useState('admin123');

  const testAdminLogin = async () => {
    try {
      console.log('🔴 Testing admin login...');
      const response = await api.post('/api/Auth/login', {
        Email: testEmail,
        Password: testPassword
      });
      console.log('🔴 Admin login response:', response.data);
      setLoginResult(response.data);
      
      // Reload page to update auth context
      setTimeout(() => {
        window.location.reload();
      }, 1000);
    } catch (error) {
      console.error('🔴 Admin login error:', error);
      setLoginResult({ error: error.message });
    }
  };

  const testUserProfile = async () => {
    try {
      console.log('🔴 Testing user profile...');
      const response = await api.get('/api/Auth/profile');
      console.log('🔴 User profile response:', response.data);
      setProfileResult(response.data);
    } catch (error) {
      console.error('🔴 User profile error:', error);
      setProfileResult({ error: error.message });
    }
  };

  const testAdminUsers = async () => {
    try {
      console.log('🔴 Testing admin users endpoint...');
      const response = await api.get('/api/User');
      console.log('🔴 Admin users response:', response.data);
      alert('Admin users endpoint çalışıyor! Console\'da detayları görebilirsin.');
    } catch (error) {
      console.error('🔴 Admin users error:', error);
      alert('Admin users endpoint hatası: ' + error.message);
    }
  };

  const forceAdminRedirect = () => {
    console.log('🔴 Force admin redirect...');
    window.location.href = '/admin/dashboard';
  };

  const clearResults = () => {
    setLoginResult(null);
    setProfileResult(null);
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        🔍 Debug Sayfası - Admin Test
      </Typography>

      <Paper sx={{ p: 3, mb: 3, bgcolor: 'error.light' }}>
        <Typography variant="h6" gutterBottom>
          🚨 ÖNEMLİ: Admin Test Adımları
        </Typography>
        <Typography variant="body2" paragraph>
          1. <strong>Önce çıkış yap</strong> (eğer giriş yapmışsan)
          2. <strong>Test bilgilerini kontrol et</strong> (aşağıda)
          3. <strong>Admin Giriş Test</strong> butonuna tıkla
          4. <strong>Console'u aç (F12)</strong> ve log'ları kontrol et
          5. <strong>Sayfa yenilendikten sonra</strong> kullanıcı bilgilerini kontrol et
        </Typography>
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          📝 Test Bilgileri
        </Typography>
        <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
          <TextField
            label="Email"
            value={testEmail}
            onChange={(e) => setTestEmail(e.target.value)}
            size="small"
          />
          <TextField
            label="Şifre"
            value={testPassword}
            onChange={(e) => setTestPassword(e.target.value)}
            size="small"
            type="password"
          />
        </Box>
        <Typography variant="body2" color="text.secondary">
          <strong>Varsayılan:</strong> admin@emlak.com / admin123
        </Typography>
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          📊 Kullanıcı Bilgileri
        </Typography>
        <Box sx={{ mb: 2 }}>
          <Typography><strong>🔑 Access Token:</strong> {accessToken ? 'Var' : 'Yok'}</Typography>
          <Typography><strong>👤 Kullanıcı:</strong> {user ? 'Giriş yapılmış' : 'Giriş yapılmamış'}</Typography>
          <Typography><strong>👑 Admin:</strong> {isAdmin ? '✅ Evet' : '❌ Hayır'}</Typography>
          <Typography><strong>👥 User:</strong> {isUser ? '✅ Evet' : '❌ Hayır'}</Typography>
          <Typography><strong>⏳ Loading:</strong> {loading ? '✅ Evet' : '❌ Hayır'}</Typography>
        </Box>
        
        {user && (
          <Box>
            <Divider sx={{ my: 2 }} />
            <Typography variant="h6" gutterBottom>Kullanıcı Detayları:</Typography>
            <Typography><strong>ID:</strong> {user.id}</Typography>
            <Typography><strong>Kullanıcı Adı:</strong> {user.userName}</Typography>
            <Typography><strong>E-posta:</strong> {user.email}</Typography>
            <Typography><strong>Ad Soyad:</strong> {user.fullName}</Typography>
            <Typography><strong>Roller:</strong> {user.roles?.join(', ') || 'Rol yok'}</Typography>
            <Typography><strong>Roller (JSON):</strong> {JSON.stringify(user.roles)}</Typography>
            <Typography><strong>Roller Tipi:</strong> {typeof user.roles}</Typography>
            <Typography><strong>Roller Uzunluğu:</strong> {user.roles?.length || 0}</Typography>
          </Box>
        )}
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          🧪 Test Butonları
        </Typography>
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', mb: 2 }}>
          <Button variant="contained" color="primary" onClick={testAdminLogin}>
            🔐 Admin Giriş Test
          </Button>
          <Button variant="contained" color="secondary" onClick={testUserProfile}>
            👤 Profil Test
          </Button>
          <Button variant="contained" color="info" onClick={testAdminUsers}>
            👥 Admin Users Test
          </Button>
          <Button variant="contained" color="warning" onClick={forceAdminRedirect}>
            🚀 Force Admin Redirect
          </Button>
          <Button variant="outlined" onClick={clearResults}>
            🗑️ Sonuçları Temizle
          </Button>
        </Box>
      </Paper>

      {loginResult && (
        <Paper sx={{ p: 3, mb: 3, bgcolor: 'success.light' }}>
          <Typography variant="h6" gutterBottom>
            🔐 Login Sonucu
          </Typography>
          <pre style={{ overflow: 'auto', fontSize: '12px' }}>
            {JSON.stringify(loginResult, null, 2)}
          </pre>
        </Paper>
      )}

      {profileResult && (
        <Paper sx={{ p: 3, mb: 3, bgcolor: 'info.light' }}>
          <Typography variant="h6" gutterBottom>
            👤 Profil Sonucu
          </Typography>
          <pre style={{ overflow: 'auto', fontSize: '12px' }}>
            {JSON.stringify(profileResult, null, 2)}
          </pre>
        </Paper>
      )}

      <Paper sx={{ p: 3, bgcolor: 'warning.light' }}>
        <Typography variant="h6" gutterBottom>
          📋 Test Adımları
        </Typography>
        <Typography variant="body2" paragraph>
          1. <strong>Admin Giriş Test</strong> butonuna tıkla
          2. Console'da (F12) login response'unu kontrol et
          3. Sayfa yenilendikten sonra kullanıcı bilgilerini kontrol et
          4. Admin rolü doğru şekilde geliyorsa ana sayfada admin dashboard görünmeli
          5. Eğer hala çalışmıyorsa <strong>Force Admin Redirect</strong> butonunu dene
        </Typography>
      </Paper>
    </Container>
  );
};

export default Debug; 