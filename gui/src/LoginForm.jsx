// src/LoginForm.jsx
import { useState } from 'react';
import axios from 'axios';

export default function LoginForm() {
  const [isLogin, setIsLogin] = useState(true);
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');
  const [message, setMessage] = useState('');
  
  const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:3000/api/auth';

  const toggleMode = () => {
    setIsLogin(!isLogin);
    setMessage('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const endpoint = isLogin ? 'login' : 'register';
      const payload  = isLogin
        ? { login, password }
        : { login, password, name };

      const { data } = await axios.post(`${API_URL}/${endpoint}`, payload);
      
      if (isLogin)
      {
        localStorage.setItem('token', data.token);
        window.location.href = '/';  // navigates to Dashboard
      }
      else
      {
        setMessage('✅ Registered! You can now log in.');
        setIsLogin(true);
      }
    } catch (err) {
      // show error from server or generic
      setMessage(err.response?.data || '❌ Error');
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
      <div className="card p-4 shadow" style={{ minWidth: 320 }}>
        <h2 className="text-center mb-3">
          {isLogin ? 'Login' : 'Register'}
        </h2>
        <form onSubmit={handleSubmit}>
          {!isLogin && (
            <div className="mb-3">
              <label className="form-label">Full Name</label>
              <input
                type="text"
                className="form-control"
                value={name}
                onChange={e => setName(e.target.value)}
                required
              />
            </div>
          )}
          <div className="mb-3">
            <label className="form-label">Login</label>
            <input
              type="text"
              className="form-control"
              value={login}
              onChange={e => setLogin(e.target.value)}
              required
            />
          </div>
          <div className="mb-3">
            <label className="form-label">Password</label>
            <input
              type="password"
              className="form-control"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
            />
          </div>
          <button type="submit" className="btn btn-primary w-100">
            {isLogin ? 'Login' : 'Register'}
          </button>
        </form>
        <p className="text-center mt-3 mb-0">
          <button
            type="button"
            className="btn btn-link p-0"
            onClick={toggleMode}
          >
            {isLogin
              ? "Don't have an account? Register"
              : 'Already have an account? Login'}
          </button>
        </p>
        {message && (
          <div
            className={`text-center mt-2 ${
              message.startsWith('✅') ? 'text-success' : 'text-danger'
            }`}
          >
            {message}
          </div>
        )}
      </div>
    </div>
  );
}
