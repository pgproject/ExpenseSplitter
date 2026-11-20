
import { Shield, Mail, Lock, ArrowLeft } from "lucide-react";
import { Link } from "react-router-dom";
import { GoogleLogin } from "@react-oauth/google";

import { useAuth } from "../context/AuthContext";
import { useLoginSecurity } from "../hooks/useLoginSecurity";

function LoginSecurityPage() {
  const { user } = useAuth();

  const {
    error,
    success,
    isLinking,
    handleGoogleSuccess,
    handleGoogleError,
  } = useLoginSecurity();

  return (
    <main className="main">
      <header className="header">
        <div>
          <Link to="/settings" className="back-link">
            <ArrowLeft size={16} /> Back to Settings
          </Link>
          <h2>Login & Security</h2>
          <p>Manage your login methods and account security.</p>
        </div>
      </header>

      <section className="settings-section">
        <div className="panel">
          <div className="panel-header">
            <h3>
              <Mail size={20} /> Email
            </h3>
          </div>
          <p>{user?.email}</p>
          <span className="status-active">Account email</span>
        </div>

        <div className="panel">
          <div className="panel-header">
            <h3>
              <Lock size={20} /> Password
            </h3>
          </div>
          <p>Password authentication</p>
          <span className="status-active">Available</span>
        </div>

        <div className="panel">
          <div className="panel-header">
            <h3>
              <Shield size={20} /> Google account
            </h3>
          </div>

          <p>
            Connect your Google account to use it as an additional login method.
          </p>

          {user?.googleConnected ? (
            <span className="status-active">Connected</span>
          ) : (
            <>
              <span className="status-inactive">Not connected</span>

              <div className="google-connect-button">
                <GoogleLogin
                  onSuccess={handleGoogleSuccess}
                  onError={handleGoogleError}
                  width="320"
                  text="continue_with"
                />
              </div>
            </>
          )}

          {isLinking && <p>Connecting Google account...</p>}

          {success && (
            <p className="status-active">
              Google account connected successfully.
            </p>
          )}

          {error && <p className="status-error">{error}</p>}
        </div>
      </section>
    </main>
  );
}

export default LoginSecurityPage;
