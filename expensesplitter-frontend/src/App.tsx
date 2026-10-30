import {
  LayoutDashboard,
  Users,
  Receipt,
  Settings,
  Plus,
  ArrowUpRight,
  ArrowDownLeft,
  Wallet,
} from "lucide-react";

import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
  NavLink,
} from "react-router-dom";

import "./App.css";

import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";

function DashboardPage() {
  return (
    <div className="app">
      <aside className="sidebar">
        <h1 className="logo">ExpenseSplitter</h1>

        <nav>
          <NavLink to="/" className={({ isActive }) => isActive ? "active" : ""}>
            <LayoutDashboard size={20} /> Dashboard
          </NavLink>
          <a><Users size={20} /> My groups</a>
          <a><Receipt size={20} /> All expenses</a>
          <a><Settings size={20} /> Settings</a>
        </nav>

        <div className="profile">
          <div className="avatar">G</div>
          <div>
            <strong>Grzegorz</strong>
            <small>My account</small>
          </div>
        </div>
      </aside>

      <main className="main">
        <header className="header">
          <div>
            <h2>Dashboard</h2>
            <p>Overview of your shared expenses</p>
          </div>
          <button className="primary-button">
            <Plus size={18} /> Add expense
          </button>
        </header>

        <section className="summary">
          <div className="summary-card">
            <span>Total balance</span>
            <h2>+ 245.50 PLN</h2>
            <small>Overall balance across all groups</small>
          </div>

          <div className="summary-card">
            <span>You owe</span>
            <h2 className="negative">120.00 PLN</h2>
            <small>Across all groups</small>
          </div>

          <div className="summary-card">
            <span>You are owed</span>
            <h2 className="positive">365.50 PLN</h2>
            <small>Across all groups</small>
          </div>
        </section>

        <section className="content-grid">
          <div className="panel">
            <div className="panel-header">
              <h3>My groups</h3>
              <button className="text-button">View all</button>
            </div>

            <div className="group-item">
              <div className="group-icon">🏠</div>
              <div className="item-info">
                <strong>Apartment</strong>
                <small>4 members · 12 expenses</small>
              </div>
              <strong className="positive">+120 PLN</strong>
            </div>

            <div className="group-item">
              <div className="group-icon">✈️</div>
              <div className="item-info">
                <strong>Italy trip</strong>
                <small>3 members · 8 expenses</small>
              </div>
              <strong className="negative">-50 PLN</strong>
            </div>

            <div className="group-item">
              <div className="group-icon">🚴</div>
              <div className="item-info">
                <strong>Cycling weekend</strong>
                <small>5 members · 6 expenses</small>
              </div>
              <strong className="positive">+175.50 PLN</strong>
            </div>
          </div>

          <div className="panel">
            <div className="panel-header">
              <h3>Recent expenses</h3>
              <button className="text-button">View all</button>
            </div>

            <div className="expense-item">
              <div className="expense-icon"><Wallet size={20} /></div>
              <div className="item-info">
                <strong>Groceries</strong>
                <small>Apartment · Today</small>
              </div>
              <strong>120.00 PLN</strong>
            </div>

            <div className="expense-item">
              <div className="expense-icon"><ArrowUpRight size={20} /></div>
              <div className="item-info">
                <strong>Hotel booking</strong>
                <small>Italy trip · Yesterday</small>
              </div>
              <strong>450.00 PLN</strong>
            </div>

            <div className="expense-item">
              <div className="expense-icon"><ArrowDownLeft size={20} /></div>
              <div className="item-info">
                <strong>Restaurant</strong>
                <small>Cycling weekend · 2 days ago</small>
              </div>
              <strong>85.00 PLN</strong>
            </div>
          </div>
        </section>
      </main>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
