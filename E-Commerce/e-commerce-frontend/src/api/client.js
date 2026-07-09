import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5051',
});

api.interceptors.request.use((config) => {
  const accessToken = localStorage.getItem('accessToken');
  if (accessToken) {
    config.headers = config.headers || {};
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

let isRefreshing = false;
let subscribers = [];

function onRefreshed(newToken) {
  subscribers.forEach((cb) => cb(newToken));
  subscribers = [];
}

function addSubscriber(callback) {
  subscribers.push(callback);
}

api.interceptors.response.use(
  (res) => res,
  async (error) => {
    const originalRequest = error.config;
    if (error.response && error.response.status === 401 && !originalRequest._retry) {
      const refreshToken = localStorage.getItem('refreshToken');
      const accessToken = localStorage.getItem('accessToken');
      if (!refreshToken || !accessToken) return Promise.reject(error);

      if (isRefreshing) {
        return new Promise((resolve) => {
          addSubscriber((newToken) => {
            originalRequest.headers.Authorization = `Bearer ${newToken}`;
            resolve(api(originalRequest));
          });
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;
      try {
        const { data } = await axios.post('http://localhost:5051/api/Auth/refresh-token', {
          AccessToken: accessToken,
          RefreshToken: refreshToken,
        });
        const payload = data?.data || data?.Data;
        const newAccess = payload?.accessToken || payload?.AccessToken;
        const newRefresh = payload?.refreshToken || payload?.RefreshToken;
        if (newAccess) localStorage.setItem('accessToken', newAccess);
        if (newRefresh) localStorage.setItem('refreshToken', newRefresh);
        api.defaults.headers.common.Authorization = `Bearer ${newAccess}`;
        onRefreshed(newAccess);
        return api(originalRequest);
      } catch (e) {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        return Promise.reject(e);
      } finally {
        isRefreshing = false;
      }
    }
    return Promise.reject(error);
  }
);

export default api; 