import { List } from '@mui/material';
import { BalloonRequestDTO } from '../types';
import { BalloonItem } from './BalloonItem';

interface BalloonListProps {
  balloons: BalloonRequestDTO[];
  showActions?: boolean;
  onPickup?: (balloon: BalloonRequestDTO) => void;
  onDelivery?: (balloon: BalloonRequestDTO) => void;
  onRevert?: (balloon: BalloonRequestDTO) => void;
}

export const BalloonList = ({
  balloons,
  showActions = true,
  onPickup,
  onDelivery,
  onRevert,
}: BalloonListProps) => {
  return (
    <List>
      {(Array.isArray(balloons) ? balloons : []).map((balloon) => (
        <BalloonItem
          key={balloon.id}
          balloon={balloon}
          showActions={showActions}
          onPickup={onPickup}
          onDelivery={onDelivery}
          onRevert={onRevert}
        />
      ))}
    </List>
  );
}; 