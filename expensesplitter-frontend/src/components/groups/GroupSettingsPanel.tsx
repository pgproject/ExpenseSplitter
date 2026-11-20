import type { GetGroupDetailsResponse } from "../../types/group";

interface GroupSettingsPanelProps {
  settings: GetGroupDetailsResponse["settings"];
  currencyLabel: string;
}

function GroupSettingsPanel({
  settings,
  currencyLabel,
}: GroupSettingsPanelProps) {
  return (
    <div className="panel">
      <div className="panel-header">
        <h3>Group settings</h3>
      </div>

      <div className="setting-item">
        <span>Only owner can add members</span>
        <strong>
          {settings.onlyOwnerCanAddMember ? "Yes" : "No"}
        </strong>
      </div>

      <div className="setting-item">
        <span>Only owner can remove members</span>
        <strong>
          {settings.onlyOwnerCanRemoveMember ? "Yes" : "No"}
        </strong>
      </div>

      <div className="setting-item">
        <span>Default currency</span>
        <strong>{currencyLabel}</strong>
      </div>
    </div>
  );
}

export default GroupSettingsPanel;