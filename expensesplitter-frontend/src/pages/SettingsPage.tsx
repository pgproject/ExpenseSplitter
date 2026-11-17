
import { User, Shield, SlidersHorizontal } from "lucide-react";
import { Link } from "react-router-dom";

function SettingsPage() {
  return (
    <main className="main">
      <header className="header">
        <div>
          <h2>Settings</h2>
          <p>Manage your account and application preferences.</p>
        </div>
      </header>

      <section className="content-grid">
        <div className="panel">
          <div className="panel-header">
            <h3>
              <User size={20} /> Profile
            </h3>
          </div>
          <p>Manage your personal information.</p>
        </div>

        <Link to="/settings/login-security" className="settings-link">
          <div className="panel">
            <div className="panel-header">
              <h3>
                <Shield size={20} /> Login & Security
              </h3>
            </div>
            <p>Manage your login methods and account security.</p>
          </div>
        </Link>

        <div className="panel">
          <div className="panel-header">
            <h3>
              <SlidersHorizontal size={20} /> Preferences
            </h3>
          </div>
          <p>Manage your application preferences.</p>
        </div>
      </section>
    </main>
  );
}

export default SettingsPage;
