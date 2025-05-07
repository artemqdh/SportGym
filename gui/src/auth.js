import { jwtDecode } from 'jwt-decode';

/**
 * Extracts the user's role from the JWT stored in localStorage.
 * @returns {string|null} The role claim, or null if not present.
 */
export function getUserRole() {
  const token = localStorage.getItem('token');
  if (!token) return null;
  try {
    const decoded = jwtDecode(token);
    return decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || null;
  } catch {
    return null;
  }
}
