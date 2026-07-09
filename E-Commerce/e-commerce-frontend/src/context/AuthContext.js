import React, { createContext, useContext, useEffect, useMemo, useState } from 'react';
import api from '../api/client';

const AuthContext = createContext(null);

// Normalize various backend payload shapes into a consistent user object with roles: string[]
function normalizeUserPayload(rawPayload) {
  if (!rawPayload || typeof rawPayload !== 'object') return null;

  // If payload is wrapped: { user: {...} } or { User: {...} }
  const candidate = rawPayload.user || rawPayload.User || rawPayload;
  if (!candidate || typeof candidate !== 'object') return null;

  const id = candidate.id ?? candidate.Id ?? null;
  const userName = candidate.userName ?? candidate.UserName ?? null;
  const email = candidate.email ?? candidate.Email ?? null;
  const fullName = candidate.fullName ?? candidate.FullName ?? null;

  let rolesRaw = candidate.roles ?? candidate.Roles ?? candidate.roleNames ?? candidate.RoleNames ?? candidate.userRoles ?? candidate.UserRoles ?? null;

  // If roles are objects, map to string names
  let roles = [];
  if (Array.isArray(rolesRaw)) {
    roles = rolesRaw.map(r => {
      if (typeof r === 'string') return r;
      if (r && typeof r === 'object') {
        return r.name ?? r.Name ?? r.roleName ?? r.RoleName ?? null;
      }
      return null;
    }).filter(Boolean);
  }

  return { id, userName, email, fullName, roles };
}

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [accessToken, setAccessToken] = useState(localStorage.getItem('accessToken'));
  const [refreshToken, setRefreshToken] = useState(localStorage.getItem('refreshToken'));
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (accessToken) localStorage.setItem('accessToken', accessToken);
    else localStorage.removeItem('accessToken');
  }, [accessToken]);

  useEffect(() => {
    if (refreshToken) localStorage.setItem('refreshToken', refreshToken);
    else localStorage.removeItem('refreshToken');
  }, [refreshToken]);

  // Load user profile on app start
  useEffect(() => {
    const loadProfile = async () => {
      if (accessToken) {
        try {
          const { data } = await api.get('/api/Auth/profile');
          const payload = data?.data ?? data?.Data ?? data;
          const normalized = normalizeUserPayload(payload);
          setUser(normalized);
        } catch (error) {
          console.error('Failed to load profile:', error);
          await logout();
        }
      }
      setLoading(false);
    };

    loadProfile();
  }, [accessToken]);

  const login = async (email, password) => {
    try {
      const { data } = await api.post('/api/Auth/login', { Email: email, Password: password });
      console.log('🔴 Login response:', data);
      
      const payload = data?.data || data?.Data || data;
      const token = payload?.token || payload?.Token || {};
      const access = token.accessToken || token.AccessToken || null;
      const refresh = token.refreshToken || token.RefreshToken || null;

      // Persist immediately for interceptors
      if (access) localStorage.setItem('accessToken', access);
      if (refresh) localStorage.setItem('refreshToken', refresh);

      setAccessToken(access);
      setRefreshToken(refresh);
      
      // Try to get user from login payload first
      let normalized = normalizeUserPayload(payload);

      // If user missing or roles empty, fetch profile to ensure roles are present
      if (!normalized || !Array.isArray(normalized.roles) || normalized.roles.length === 0) {
        try {
          const profileRes = await api.get('/api/Auth/profile');
          const profPayload = profileRes?.data?.data ?? profileRes?.data?.Data ?? profileRes?.data;
          normalized = normalizeUserPayload(profPayload) || normalized;
        } catch (e) {
          console.warn('Profile fetch after login failed:', e);
        }
      }

      setUser(normalized);

      // Admin kullanıcı ise doğrudan yönlendir
      if (normalized?.roles?.includes('Admin')) {
        console.log('🔴 Admin user detected in login/profile, redirecting to admin dashboard');
        setTimeout(() => {
          window.location.href = '/admin/dashboard';
        }, 100);
      }
    } catch (error) {
      console.error('🔴 Login error:', error);
      throw error;
    }
  };

  const register = async ({ userName, email, password, fullName }) => {
    const { data } = await api.post('/api/Auth/register', { UserName: userName, Email: email, Password: password, FullName: fullName });
    const payload = data?.data || data?.Data || data;
    const token = payload?.token || payload?.Token || {};
    const access = token.accessToken || token.AccessToken || null;
    const refresh = token.refreshToken || token.RefreshToken || null;
    if (access) localStorage.setItem('accessToken', access);
    if (refresh) localStorage.setItem('refreshToken', refresh);
    setAccessToken(access);
    setRefreshToken(refresh);
    setUser(normalizeUserPayload(payload));
  };

  const logout = async () => {
    try {
      if (localStorage.getItem('accessToken')) {
        await api.post('/api/Auth/logout');
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      setUser(null);
      setAccessToken(null);
      setRefreshToken(null);
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    }
  };

  const isAdmin = useMemo(() => {
    const adminStatus = user?.roles?.includes('Admin') || false;
    console.log('🔴 isAdmin calculation:', { 
      user: user?.id, 
      userName: user?.userName, 
      roles: user?.roles, 
      adminStatus,
      rolesType: typeof user?.roles,
      rolesLength: user?.roles?.length
    });
    return adminStatus;
  }, [user]);

  const isUser = useMemo(() => {
    const userStatus = user?.roles?.includes('User') || false;
    console.log('🔴 isUser calculation:', { 
      user: user?.id, 
      userName: user?.userName, 
      roles: user?.roles, 
      userStatus 
    });
    return userStatus;
  }, [user]);

  const value = useMemo(() => ({ 
    user, 
    accessToken, 
    refreshToken, 
    loading,
    isAdmin,
    isUser,
    login, 
    logout, 
    register 
  }), [user, accessToken, refreshToken, loading, isAdmin, isUser]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext); 