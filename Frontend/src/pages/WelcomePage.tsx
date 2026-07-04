import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getCurrentUser, getWeather, logout, isAuthenticated } from '../services/api';

export const WelcomePage = () => {
  const [user, setUser] = useState<string | null>(null);
  const [temperature, setTemperature] = useState<number | null>(null);
  const [place, setPlace] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      if (!isAuthenticated()) {
        navigate('/login');
        return;
      }

      try {
        const username = await getCurrentUser();
        const weather = await getWeather();

        if (username) setUser(username);
        if (weather) {
          setTemperature(weather.temperature);
          setPlace(weather.place);
        }
      } catch (err) {
        setError('Failed to load data');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [navigate]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!user) {
    return <div>Not authenticated, redirecting...</div>;
  }

  return (
    <div style={{ maxWidth: '600px', margin: '50px auto', padding: '20px', textAlign: 'center' }}>
      <h1>Hello {user} welcome!</h1>
      {temperature !== null && place && (
        <p style={{ fontSize: '1.5em', margin: '20px 0' }}>
          It's {temperature} degrees outside in {place}
        </p>
      )}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <button
        onClick={handleLogout}
        style={{ padding: '10px 20px', background: '#dc3545', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', marginTop: '20px' }}
      >
        Logout
      </button>
    </div>
  );
};
