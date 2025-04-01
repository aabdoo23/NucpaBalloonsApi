import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
} from '@mui/material';

interface SettingsDialogProps {
  open: boolean;
  userName: string;
  onClose: () => void;
  onSave: () => void;
  onUserNameChange: (value: string) => void;
}

export const SettingsDialog = ({
  open,
  userName,
  onClose,
  onSave,
  onUserNameChange,
}: SettingsDialogProps) => {
  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>Settings</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
          margin="dense"
          label="Your Name"
          fullWidth
          value={userName}
          onChange={(e) => onUserNameChange(e.target.value)}
          helperText="This name will be used for all status changes"
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button onClick={onSave} variant="contained">
          Save
        </Button>
      </DialogActions>
    </Dialog>
  );
}; 