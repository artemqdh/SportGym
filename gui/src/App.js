import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginForm from './LoginForm';
import Dashboard from './Dashboard';
import TrainerManagement from './TrainerManagement';
import ScheduleManagement from './ScheduleManagement';

function App() {
  const token = localStorage.getItem('token');

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={
          token ? <Navigate to="/dashboard" replace/> : <Navigate to="/login" replace/>
        }/>
        <Route path="/login" element={
          !token ? <LoginForm /> : <Navigate to="/dashboard" replace/>
        }/>
        <Route path="/dashboard" element={
          token ? <Dashboard /> : <Navigate to="/login" replace/>
        }/>
        <Route path="/schedules" element={
          token ? <ScheduleManagement/> : <Navigate to="/login" replace/>
        }/>
        <Route path="/trainers" element={
          token ? <TrainerManagement /> : <Navigate to="/login" replace/>
        }/>
        <Route path="*" element={<Navigate to="/" replace/>}/>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
