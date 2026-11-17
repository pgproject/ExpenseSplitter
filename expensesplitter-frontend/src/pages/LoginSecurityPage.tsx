
import { useState } from "react";
import { Shield, Mail, Lock, ArrowLeft } from "lucide-react";
import { Link } from "react-router-dom";
import { GoogleLogin } from "@react-oauth/google";
import { useAuth } from "../context/AuthContext";

function LoginSecurityPage() {
  const { user, linkGoogleAccount } = useAuth();
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [isLinking, setIsLinking] = useState(false);

  const handleGoogleSuccess = async (credentialResponse: {
    credential?: string;
  }) => {
    if (!credentialResponse.credential) {
      setError("Google did not return an ID token.");
      return;
    }

    setError(null);
    setSuccess(false);
    setIsLinking(true);

    try {
      await linkGoogleAccount(credentialResponse.credential);
      setSuccess(true);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Failed to connect Google account."
      );
    } finally {
      setIsLinking(false);
    }
  };

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
                  onError={() => setError("Google authentication failed.")}
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
