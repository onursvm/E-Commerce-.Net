import React, { useEffect, useState } from 'react';
import { Container, Grid, Card, CardContent, Typography, CardActions, Button } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import api from '../api/client';

const MyProperties = () => {
  const [items, setItems] = useState([]);
  const navigate = useNavigate();

  const load = () => api.get('/api/Property/my-properties').then((res) => setItems(res.data.data || []));

  useEffect(() => {
    load();
  }, []);

  const remove = async (id) => {
    await api.delete(`/api/Property/${id}`);
    load();
  };

  return (
    <Container sx={{ mt: 4 }}>
      <Typography variant="h5" gutterBottom>İlanlarım</Typography>
      <Button variant="contained" component={Link} to="/properties/create" sx={{ mb: 2 }}>Yeni İlan</Button>
      <Grid container spacing={2}>
        {items.map((p) => (
          <Grid item xs={12} md={6} key={p.id}>
            <Card>
              <CardContent>
                <Typography variant="h6">{p.title}</Typography>
                <Typography>{p.price} ₺</Typography>
              </CardContent>
              <CardActions>
                <Button size="small" component={Link} to={`/properties/${p.id}/edit`}>Düzenle</Button>
                <Button size="small" color="error" onClick={() => remove(p.id)}>Sil</Button>
                <Button size="small" onClick={() => navigate(`/property/${p.id}`)}>Görüntüle</Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Container>
  );
};

export default MyProperties; 