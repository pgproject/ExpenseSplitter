import { Link } from "react-router-dom";
import { ArrowLeft, Plus } from "lucide-react";

interface GroupDetailsHeaderProps {
  groupName: string;
  membersCount: number;
  onAddExpense: () => void;
}

function GroupDetailsHeader({
  groupName,
  membersCount,
  onAddExpense,
}: GroupDetailsHeaderProps) {
  return (
    <>
      <div className="group-back">
        <Link to="/" className="text-button">
          <ArrowLeft size={17} /> Back to dashboard
        </Link>
      </div>

      <header className="header group-page-header">
        <div>
          <h2>{groupName}</h2>
          <p>{membersCount} members · Group details</p>
        </div>

        <button
          className="primary-button"
          onClick={onAddExpense}
        >
          <Plus size={18} /> Add expense
        </button>
      </header>
    </>
  );
}

export default GroupDetailsHeader;