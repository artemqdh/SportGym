import { useEffect, useState } from 'react';
import axios from 'axios';
import { getUserRole } from './auth';

export default function ScheduleManagement() {
  const role = getUserRole();
  // only Admin/Manager can see this
  if (role !== 'Admin' && role !== 'Manager') {
    return <p className="text-danger mt-4 text-center">Access denied.</p>;
  }

  // State
  const [schedules, setSchedules] = useState([]);
  const [loading, setLoading]     = useState(true);
  const [error, setError]         = useState('');
  const [mode, setMode]           = useState('add'); // 'add' or 'edit'
  const [current, setCurrent]     = useState(null);
  const [form, setForm]           = useState({
    type: '',
    trainerName: '',
    clientNames: '',
    time: '',
    gymName: '',
  });

  // Base API URL
  const apiBase = (process.env.REACT_APP_API_URL || 'http://localhost:3000/api/auth')
                  .replace('/auth','');

  // Auth header helper
  const authHeader = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
  };

  // Load schedules
  const load = async () => {
    try {
      setLoading(true);
      const res = await axios.get(`${apiBase}/TrainingSchedule`, authHeader());
      setSchedules(res.data);
      setError('');
    } catch {
      setError('Failed to load schedules.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  // Form change
  const onChange = e => {
    const { id, value } = e.target;
    setForm(f => ({ ...f, [id]: value }));
  };

  // Start editing
  const startEdit = sched => {
    setMode('edit');
    setCurrent(sched);
    setForm({
      type:         sched.type,
      trainerName:  sched.trainerName,
      clientNames:  sched.clientNames,
      time:         sched.time,
      gymName:      sched.gymName,
    });
  };

  // Reset form to add
  const resetForm = () => {
    setMode('add');
    setCurrent(null);
    setForm({ type:'', trainerName:'', clientNames:'', time:'', gymName:'' });
    setError('');
  };

  // Submit add/edit
  const onSubmit = async e => {
    e.preventDefault();
    try {
      if (mode === 'add') {
        await axios.post(`${apiBase}/TrainingSchedule`, form, authHeader());
      } else {
        await axios.put(
          `${apiBase}/TrainingSchedule/${current.id}`,
          { id: current.id, ...form },
          authHeader()
        );
      }
      resetForm();
      load();
    } catch {
      setError(`Failed to ${mode} schedule.`);
    }
  };

  // Delete
  const onDelete = async id => {
    if (!window.confirm('Delete this schedule?')) return;
    try {
      await axios.delete(`${apiBase}/TrainingSchedule/${id}`, authHeader());
      load();
    } catch {
      setError('Failed to delete schedule.');
    }
  };

  if (loading) return <p className="text-center mt-4">Loading schedules…</p>;
  if (error)   return <p className="text-danger text-center mt-4">{error}</p>;

  return (
    <div className="container mt-4">
      <h1>Training Schedule Management</h1>

      {/* Form */}
      <div className="card mb-4 p-3">
        <h5>{mode==='add' ? 'Add New Schedule' : 'Edit Schedule'}</h5>
        <form onSubmit={onSubmit}>
          {['type','trainerName','clientNames','time','gymName'].map(field => (
            <div className="mb-2" key={field}>
              <label htmlFor={field} className="form-label">
                {field.charAt(0).toUpperCase()+field.slice(1)}
              </label>
              <input
                id={field}
                type="text"
                className="form-control"
                value={form[field]}
                onChange={onChange}
                placeholder={
                  field==='time' 
                    ? 'e.g. Monday|08:00|10:00' 
                    : ''
                }
                required
              />
            </div>
          ))}
          <button type="submit" className="btn btn-primary me-2">
            {mode==='add' ? 'Add Schedule' : 'Save Changes'}
          </button>
          {mode==='edit' && (
            <button type="button" className="btn btn-secondary" onClick={resetForm}>
              Cancel
            </button>
          )}
        </form>
      </div>

      {/* List */}
      <ul className="list-group">
        {schedules.map(s => (
          <li key={s.id} className="list-group-item d-flex justify-content-between align-items-start">
            <div>
              <strong>{s.type}</strong> — {s.time}<br/>
              Trainer: {s.trainerName}<br/>
              Gym: {s.gymName}<br/>
              Clients: {s.clientNames || '—'}
            </div>
            <div>
              <button className="btn btn-sm btn-outline-secondary me-2" onClick={()=>startEdit(s)}>
                Edit
              </button>
              <button className="btn btn-sm btn-outline-danger" onClick={()=>onDelete(s.id)}>
                Delete
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
