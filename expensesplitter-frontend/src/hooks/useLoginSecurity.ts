
import { useState } from "react";
import { useAuth } from "../context/AuthContext";

export function useLoginSecurity() {
  const { linkGoogleAccount } = useAuth();

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

  const handleGoogleError = () => {
    setError("Google authentication failed.");
  };

  return {
    error,
    success,
    isLinking,
    handleGoogleSuccess,
    handleGoogleError,
  };
}
