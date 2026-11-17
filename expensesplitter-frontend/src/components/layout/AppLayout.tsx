import {
  LayoutDashboard,
  Users,
  Receipt,
  Settings,
} from "lucide-react";

import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

function AppLayout() {
  const { user, logout } = useAuth();
  const initials = user?.email?.charAt(0).toUpperCase() ?? "?";

  return (
    <div className="app">
      <aside className="sidebar">
        <h1 className="logo">ExpenseSplitter</h1>

        <nav>
          <NavLink
            to="/"
            end
            className={({ isActive }) => (isActive ? "active" : "")}
          >
            <LayoutDashboard size={20} /> Dashboard
          </NavLink>

          <NavLink
            to="/groups"
            className={({ isActive }) => (isActive ? "active" : "")}
          >
            <Users size={20} /> My groups
          </NavLink>

          <NavLink
            to="/expenses"
            className={({ isActive }) => (isActive ? "active" : "")}
          >
            <Receipt size={20} /> All expenses
          </NavLink>

          <NavLink
            to="/settings"
            className={({ isActive }) => (isActive ? "active" : "")}
          >
            <Settings size={20} /> Settings
          </NavLink>
        </nav>

        <div className="profile">
          <div className="avatar">{initials}</div>

          <div>
            <strong>{user?.email ?? "User"}</strong>
            <small>My account</small>

            <button className="text-button" onClick={logout}>
              Log out
            </button>
          </div>
        </div>
      </aside>

      <Outlet />
    </div>
  );
}

export default AppLayout;
