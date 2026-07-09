import React from 'react';
import { AppBar, Toolbar, Typography, Button, Box } from '@mui/material';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/');
  };

  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" component={RouterLink} to="/" sx={{ flexGrow: 1, textDecoration: 'none', color: 'inherit' }}>
          Emlak E-Ticaret
        </Typography>
        
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button color="inherit" component={RouterLink} to="/">
            Ana Sayfa
          </Button>
          
          {user ? (
            <>
              {isAdmin && (
                <>
                  <Button color="inherit" component={RouterLink} to="/admin/users">
                    Kullanıcı Yönetimi
                  </Button>
                  <Button color="inherit" component={RouterLink} to="/admin/properties">
                    Tüm İlanlar
                  </Button>
                  <Button color="inherit" component={RouterLink} to="/admin/dashboard">
                    Admin Panel
                  </Button>
                </>
              )}
              
              {!isAdmin && (
                <>
                  <Button color="inherit" component={RouterLink} to="/my-properties">
                    İlanlarım
                  </Button>
                  <Button color="inherit" component={RouterLink} to="/properties/create">
                    İlan Ekle
                  </Button>
                </>
              )}
              
              <Button color="inherit" component={RouterLink} to="/profile">
                Profil
              </Button>
              <Button color="inherit" onClick={handleLogout}>
                Çıkış
              </Button>
            </>
          ) : (
            <>
              <Button color="inherit" component={RouterLink} to="/login">
                Giriş
              </Button>
              <Button color="inherit" component={RouterLink} to="/register">
                Kayıt Ol
              </Button>
            </>
          )}
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar; 