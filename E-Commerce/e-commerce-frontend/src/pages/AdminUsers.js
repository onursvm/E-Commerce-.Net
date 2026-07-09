import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Button,
  IconButton,
  Switch,
  Alert,
  CircularProgress,
  Box,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Add as AddIcon
} from '@mui/icons-material';
import api from '../api/client';

const AdminUsers = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [openCreateDialog, setOpenCreateDialog] = useState(false);
  const [createUserData, setCreateUserData] = useState({
    userName: '',
    email: '',
    password: '',
    fullName: '',
    role: 'User'
  });

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await api.get('/api/User');
      const usersData = response.data?.Data || response.data?.data || [];
      setUsers(usersData);
    } catch (err) {
      console.error(err);
      setError('Kullanıcılar yüklenirken hata oluştu: ' + (err.response?.data?.message || err.message));
    } finally {
      setLoading(false);
    }
  };

  const handleToggleStatus = async (userId, currentStatus) => {
      const user = users.find(u => u.Id === userId);
      if (!user) return;

      const updatedUser = { ...user, IsActive: !currentStatus };

      try {
          await api.put(`/api/User/${userId}`, updatedUser); 
          fetchUsers();
      } catch (err) {
          console.error(err);
          setError('Kullanıcı durumu güncellenirken hata oluştu');
      }
  };

  const handleDeleteUser = async (userId) => {
    if (window.confirm('Bu kullanıcıyı silmek istediğinizden emin misiniz?')) {
      try {
        await api.delete(`/api/User/${userId}`);
        fetchUsers();
      } catch (err) {
        console.error(err);
        setError('Kullanıcı silinirken hata oluştu');
      }
    }
  };

  const handleCreateUser = async () => {
    try {
      await api.post('/api/User', createUserData);
      setOpenCreateDialog(false);
      setCreateUserData({ userName: '', email: '', password: '', fullName: '', role: 'User' });
      fetchUsers();
    } catch (err) {
      console.error(err);
      setError('Kullanıcı oluşturulurken hata oluştu: ' + (err.response?.data?.message || err.message));
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
        <Typography variant="h4">Kullanıcı Yönetimi</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpenCreateDialog(true)}>Yeni Kullanıcı</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Paper>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>ID</TableCell>
                <TableCell>Kullanıcı Adı</TableCell>
                <TableCell>E-posta</TableCell>
                <TableCell>Ad Soyad</TableCell>
                <TableCell>Roller</TableCell>
                <TableCell>Durum</TableCell>
                <TableCell>İşlemler</TableCell>
              </TableRow>
            </TableHead>
                      <TableBody>
                          {users.map((user) => (
                              <TableRow key={user.Id}>
                                  <TableCell>{user.Id}</TableCell>
                                  <TableCell>{user.UserName}</TableCell>
                                  <TableCell>{user.Email}</TableCell>
                                  <TableCell>{user.FullName}</TableCell>
                                  <TableCell>{user.Roles?.join(', ') || 'Rol yok'}</TableCell>
                                  <TableCell>
                                      <Switch
                                          checked={user.IsActive}
                                          onChange={() => handleToggleStatus(user.Id, user.IsActive)}
                                          color="primary"
                                      />
                                  </TableCell>
                                  <TableCell>
                                      <IconButton color="primary" onClick={() => {/* Düzenleme */ }}>
                                          <EditIcon />
                                      </IconButton>
                                      <IconButton color="error" onClick={() => handleDeleteUser(user.Id)}>
                                          <DeleteIcon />
                                      </IconButton>
                                  </TableCell>
                              </TableRow>
                          ))}
                      </TableBody>

          </Table>
        </TableContainer>
      </Paper>

      <Dialog open={openCreateDialog} onClose={() => setOpenCreateDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Yeni Kullanıcı Ekle</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
            <TextField label="Kullanıcı Adı" value={createUserData.userName} onChange={e => setCreateUserData({...createUserData, userName: e.target.value})} fullWidth required />
            <TextField label="E-posta" type="email" value={createUserData.email} onChange={e => setCreateUserData({...createUserData, email: e.target.value})} fullWidth required />
            <TextField label="Şifre" type="password" value={createUserData.password} onChange={e => setCreateUserData({...createUserData, password: e.target.value})} fullWidth required />
            <TextField label="Ad Soyad" value={createUserData.fullName} onChange={e => setCreateUserData({...createUserData, fullName: e.target.value})} fullWidth required />
            <FormControl fullWidth>
              <InputLabel>Rol</InputLabel>
              <Select value={createUserData.role} label="Rol" onChange={e => setCreateUserData({...createUserData, role: e.target.value})}>
                <MenuItem value="User">User</MenuItem>
                <MenuItem value="Admin">Admin</MenuItem>
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenCreateDialog(false)}>İptal</Button>
          <Button variant="contained" onClick={handleCreateUser}>Kullanıcı Ekle</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default AdminUsers;
