
import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { useAuth } from "../context/AuthContext";

export function useLogin() {
  const { login, loginWithGoogle } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (
    event: React.SyntheticEvent<HTMLFormElement>
        ) => {
        event.preventDefault();
        setError("");
        setIsSubmitting(true);

        try {
        await login({ email, password });
        navigate("/", { replace: true });
        } catch (err) {
        setError(
            err instanceof Error
            ? err.message
            : "An unexpected error occurred."
        );
        } finally {
        setIsSubmitting(false);
        }
    };

  const handleGoogleSuccess = async (credential: string) => {
    setError("");
    setIsSubmitting(true);

    try {
      await loginWithGoogle(credential);
      navigate("/", { replace: true });
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "An unexpected error occurred."
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return {
    email,
    setEmail,
    password,
    setPassword,
    error,
    setError,
    isSubmitting,
    handleSubmit,
    handleGoogleSuccess,
  };
}
