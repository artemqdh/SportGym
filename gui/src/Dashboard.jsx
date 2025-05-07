import { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import { getUserRole } from './auth';

export default function Dashboard() {
  const [trainers, setTrainers] = useState(null);
  const [error, setError]       = useState('');
  const role                     = getUserRole();

  const handleLogout = () => {
    localStorage.removeItem('token');
    window.location.href = '/login';
  };

  useEffect(() => {
    const fetchTrainers = async () => {
      try {
        const token   = localStorage.getItem('token');
        const apiBase = (process.env.REACT_APP_API_URL || 'http://localhost:3000/api/auth')
                        .replace('/auth', '');
        const res     = await axios.get(`${apiBase}/trainer`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        setTrainers(res.data);
      } catch {
        setError('Failed to fetch trainers.');
        setTrainers([]); 
      }
    };
    fetchTrainers();
  }, []);

  if (trainers === null) return <p className="text-center mt-4">Loading trainers…</p>;
  if (error)             return <p className="text-danger text-center mt-4">{error}</p>;

  return (
    <div className="container mt-4">
      <button className="btn btn-secondary mb-3" onClick={handleLogout}>
        Logout
      </button>

      {(role==='Admin' || role==='Manager') && (
        <>
          <Link to="/trainers" className="btn btn-primary mb-3 ms-2">
            Manage Trainers
          </Link>
          <Link to="/schedules" className="btn btn-success mb-3 ms-2">
            Manage Schedules
          </Link>
        </>
      )}

      {trainers.length === 0 ? (
        <p>No trainers found.</p>
      ) : (
        <>
          <h1>Our Trainers</h1>
          <ul className="list-group">
            {trainers.map(t => (
              <li key={t.id} className="list-group-item">
                <strong>{t.name}</strong> — {t.specialization}
              </li>
            ))}
          </ul>
        </>
      )}
    </div>
  );
}
