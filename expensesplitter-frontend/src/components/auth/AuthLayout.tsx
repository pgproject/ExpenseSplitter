
import type { ReactNode } from "react";
import { Wallet } from "lucide-react";

interface AuthLayoutProps {
  children: ReactNode;
}

function AuthLayout({ children }: AuthLayoutProps) {
  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-brand">
          <div className="auth-logo">
            <Wallet size={25} />
          </div>
          <span>ExpenseSplitter</span>
        </div>

        {children}
      </div>

      <div className="auth-bottom-text">
        ExpenseSplitter · Manage expenses together
      </div>
    </div>
  );
}

export default AuthLayout;
