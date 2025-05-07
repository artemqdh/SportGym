import { useEffect, useState } from 'react';
import axios from 'axios';
import { getUserRole } from './auth';

export default function TrainerManagement() {
  const role = getUserRole();
  // State variables
  const [trainers, setTrainers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [formMode, setFormMode] = useState('add'); // or 'edit'
  const [currentTrainer, setCurrentTrainer] = useState(null);
  const [formData, setFormData] = useState({
    name: '', birthDate: '', phoneNumber: '', email: '',
    gender: '', status: '', specialization: '', workingHours: '',
    login: '', passwordHash: ''
  });

  // Redirect if not authorized
  if (role !== 'Admin' && role !== 'Manager') {
    return <p className="text-danger mt-4 text-center">Access denied.</p>;
  }

  const apiBase = (process.env.REACT_APP_API_URL || 'http://localhost:3000/api/auth').replace('/auth','');

  // Helper to get Auth header
  const authHeader = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
  };

  // Fetch trainers
  const load = async () => {
    try {
      setLoading(true);
      const res = await axios.get(`${apiBase}/trainer`, authHeader());
      setTrainers(res.data);
      setError('');
    } catch {
      setError('Failed to load trainers.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  // Handle form input changes
  const handleChange = e => {
    const { id, value } = e.target;
    setFormData(fd => ({ ...fd, [id]: value }));
  };

  // Prepare form for edit
  const startEdit = t => {
    setFormMode('edit');
    setCurrentTrainer(t);
    setFormData({ ...t, passwordHash: '' });
  };

  // Reset form to "add" mode
  const resetForm = () => {
    setFormMode('add');
    setCurrentTrainer(null);
    setFormData({
      name: '', birthDate: '', phoneNumber: '', email: '',
      gender: '', status: '', specialization: '', workingHours: '',
      login: '', passwordHash: ''
    });
    setError('');
  };

  // Submit add or edit
  const handleSubmit = async e => {
    e.preventDefault();
    try {
      if (formMode === 'add') {
        await axios.post(`${apiBase}/trainer`, formData, authHeader());
      } else {
        await axios.put(
          `${apiBase}/trainer/${currentTrainer.id}`,
          { ...formData, id: currentTrainer.id },
          authHeader()
        );
      }
      resetForm();
      load();
    } catch {
      setError(`Failed to ${formMode} trainer.`);
    }
  };

  // Delete a trainer
  const handleDelete = async id => {
    if (!window.confirm('Delete this trainer?')) return;
    try {
      await axios.delete(`${apiBase}/trainer/${id}`, authHeader());
      load();
    } catch {
      setError('Failed to delete trainer.');
    }
  };

  if (loading) return <p className="text-center mt-4">Loading…</p>;
  if (error)   return <p className="text-danger text-center mt-4">{error}</p>;

  return (
    <div className="container mt-4">
      <h1>Trainer Management</h1>

      {/* Form */}
      <div className="card mb-4 p-3">
        <h5>{formMode === 'add' ? 'Add New Trainer' : 'Edit Trainer'}</h5>
        <form onSubmit={handleSubmit}>
          {['name','birthDate','phoneNumber','email','gender','status','specialization','workingHours','login'].map(field => (
            <div className="mb-2" key={field}>
              <label htmlFor={field} className="form-label">
                {field.charAt(0).toUpperCase() + field.slice(1)}
              </label>
              <input
                id={field}
                type={field === 'birthDate' ? 'date' : 'text'}
                className="form-control"
                value={formData[field]}
                onChange={handleChange}
                required
              />
            </div>
          ))}

          {/* Password only for add or reset */}
          <div className="mb-2">
            <label htmlFor="passwordHash" className="form-label">
              {formMode === 'add' ? 'Password' : 'New Password (leave blank to keep)'}
            </label>
            <input
              id="passwordHash"
              type="password"
              className="form-control"
              value={formData.passwordHash}
              onChange={handleChange}
              {...(formMode==='add' && { required: true })}
            />
          </div>

          <button type="submit" className="btn btn-primary me-2">
            {formMode === 'add' ? 'Add Trainer' : 'Save Changes'}
          </button>
          {formMode === 'edit' && (
            <button type="button" className="btn btn-secondary" onClick={resetForm}>
              Cancel
            </button>
          )}
        </form>
      </div>

      {/* List */}
      <ul className="list-group">
        {trainers.map(t => (
          <li key={t.id} className="list-group-item d-flex justify-content-between align-items-start">
            <div>
              <strong>{t.name}</strong> — {t.specialization}
            </div>
            <div>
              <button className="btn btn-sm btn-outline-secondary me-2" onClick={() => startEdit(t)}>
                Edit
              </button>
              <button className="btn btn-sm btn-outline-danger" onClick={() => handleDelete(t.id)}>
                Delete
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
