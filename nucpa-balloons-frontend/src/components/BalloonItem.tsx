import {
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Button,
  Stack,
  Typography,
  Chip,
} from '@mui/material';
import { BalloonRequestDTO } from '../types';

interface BalloonItemProps {
  balloon: BalloonRequestDTO;
  showActions?: boolean;
  onPickup?: (balloon: BalloonRequestDTO) => void;
  onDelivery?: (balloon: BalloonRequestDTO) => void;
  onRevert?: (balloon: BalloonRequestDTO) => void;
}

export const BalloonItem = ({
  balloon,
  showActions = true,
  onPickup,
  onDelivery,
  onRevert,
}: BalloonItemProps) => {
  return (
    <ListItem
      sx={{
        mb: 2,
        border: '1px solid #e0e0e0',
        borderRadius: 1,
        '&:hover': {
          backgroundColor: '#f5f5f5',
        },
      }}
    >
      <ListItemText
        primary={
          <Stack direction="row" spacing={1} alignItems="center">
            <Typography variant="h6" component="span">
              {balloon.teamId} - {balloon.teamName}
            </Typography>
            <Chip
              label={`Problem ${balloon.problemIndex}`}
              color="primary"
              size="small"
            />
            <Chip
              label={balloon.balloonColor}
              sx={{
                backgroundColor: balloon.balloonColor,
                color: 'white',
              }}
              size="small"
            />
          </Stack>
        }
        secondary={
          <Stack spacing={0.5}>
            <Typography variant="body2" color="text.secondary" component="span">
              Submitted at: {new Date(balloon.timestamp).toLocaleString()}
            </Typography>
            {balloon.statusChangedAt && (
              <Typography variant="body2" color="text.secondary" component="span">
                Picked up by: {balloon.statusChangedBy} at {new Date(balloon.statusChangedAt).toLocaleString()}
              </Typography>
            )}
            {balloon.statusChangedAt && (
              <Typography variant="body2" color="text.secondary" component="span">
                Delivered by: {balloon.statusChangedBy} at {new Date(balloon.statusChangedAt).toLocaleString()}
              </Typography>
            )}
          </Stack>
        }
      />
      {showActions && (
        <ListItemSecondaryAction>
          {balloon.status === 'Pending' && onPickup && (
            <Button
              variant="contained"
              color="primary"
              onClick={() => onPickup(balloon)}
              sx={{ mr: 1 }}
            >
              Pick Up
            </Button>
          )}
          {balloon.status === 'PickedUp' && onDelivery && (
            <Button
              variant="contained"
              color="success"
              onClick={() => onDelivery(balloon)}
            >
              Deliver
            </Button>
          )}
          {balloon.status === 'Delivered' && onRevert && (
            <Button
              variant="contained"
              color="warning"
              onClick={() => onRevert(balloon)}
            >
              Revert to Picked Up
            </Button>
          )}
        </ListItemSecondaryAction>
      )}
    </ListItem>
  );
}; 