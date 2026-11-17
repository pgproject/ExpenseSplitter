import type { ReactNode } from "react";

import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";

import AppLayout from "./components/layout/AppLayout";

import "./App.css";

import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import GroupDetailsPage from "./pages/GroupDetailsPage";
import SettingsPage from "./pages/SettingsPage";
import LoginSecurityPage from "./pages/LoginSecurityPage";
import DashboardPage from "./pages/DashboardPage";

import { useAuth } from "./context/AuthContext";


function ProtectedRoute({ children }: { children: ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return <p>Loading...</p>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <AppLayout />
            </ProtectedRoute>
          }
        >
          <Route index element={<DashboardPage />} />

          <Route
            path="groups/:groupId"
            element={<GroupDetailsPage />}
          />

          <Route
            path="settings"
            element={<SettingsPage />}
          />

          <Route
            path="settings/login-security"
            element={<LoginSecurityPage />}
          />
        </Route>

        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
