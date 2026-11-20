
import { Link, Navigate, useLocation } from "react-router-dom";
import { ArrowRight } from "lucide-react";

import { useAuth } from "../context/AuthContext";
import { useLogin } from "../hooks/useLogin";
import { GoogleLogin } from "@react-oauth/google";

import "./LoginPage.css";
import AuthLayout from "../components/auth/AuthLayout";

function LoginPage() {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  const registrationSuccess =
    location.state?.registrationSuccess === true;

  const {
    email,
    setEmail,
    password,
    setPassword,
    error,
    setError,
    isSubmitting,
    handleSubmit,
    handleGoogleSuccess,
  } = useLogin();

  if (isLoading) {
    return <div className="auth-loading">Loading...</div>;
  }

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

return (
    <AuthLayout>
      <div className="auth-heading">
        <h1>Welcome back</h1>
        <p>Sign in to manage your shared expenses.</p>
      </div>

      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="form-field">
          <label htmlFor="email">Email address</label>
          <input
            id="email"
            type="email"
            placeholder="you@example.com"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            autoComplete="email"
            required
          />
        </div>

        <div className="form-field">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            placeholder="Enter your password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
            required
          />
        </div>

        {registrationSuccess && (
          <div className="auth-success" role="status">
            Account created successfully. You can now sign in.
          </div>
        )}

        {error && (
          <div className="auth-error" role="alert">
            {error}
          </div>
        )}

        <button
          type="submit"
          className="auth-submit"
          disabled={isSubmitting}
        >
          {isSubmitting ? "Signing in..." : "Sign in"}
          {!isSubmitting && <ArrowRight size={18} />}
        </button>
      </form>

      <div className="auth-divider">
        <span>OR</span>
      </div>

      <div className="google-login">
        <GoogleLogin
          onSuccess={(response) => {
            if (response.credential) {
              void handleGoogleSuccess(response.credential);
            } else {
              setError("Google did not return an ID token.");
            }
          }}
          onError={() => {
            setError("Google sign-in failed.");
          }}
          theme="outline"
          size="large"
          text="continue_with"
          shape="rectangular"
          width="100%"
        />
      </div>

      <div className="auth-footer">
        Don't have an account?{" "}
        <Link to="/register">Create account</Link>
      </div>
    </AuthLayout>
  );
}

export default LoginPage;
