import { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Button,
  Box,
  Chip,
  Tabs,
  Tab,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  CircularProgress,
  IconButton,
  Tooltip,
} from '@mui/material';
import { Settings as SettingsIcon } from '@mui/icons-material';
import { signalRService } from '../services/signalR';
import { BalloonRequestDTO } from '../types';
import { getPendingBalloons, getPickedUpBalloons, getDeliveredBalloons, updateBalloonStatus } from '../services/api';

export const MainPage = () => {
  const [pendingBalloons, setPendingBalloons] = useState<BalloonRequestDTO[]>([]);
  const [pickedUpBalloons, setPickedUpBalloons] = useState<BalloonRequestDTO[]>([]);
  const [deliveredBalloons, setDeliveredBalloons] = useState<BalloonRequestDTO[]>([]);
  const [activeTab, setActiveTab] = useState(0);
  const [settingsDialogOpen, setSettingsDialogOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [userName, setUserName] = useState(() => localStorage.getItem('userName') || '');

  useEffect(() => {
    const handleBalloonUpdates = (updates: { Pending: BalloonRequestDTO[], PickedUp: BalloonRequestDTO[], Delivered: BalloonRequestDTO[] }) => {
      setPendingBalloons(updates.Pending);
      setPickedUpBalloons(updates.PickedUp);
      setDeliveredBalloons(updates.Delivered);
    };

    const loadInitialData = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const [pending, pickedUp, delivered] = await Promise.all([
          getPendingBalloons(),
          getPickedUpBalloons(),
          getDeliveredBalloons(),
        ]);
        setPendingBalloons(Array.isArray(pending) ? pending : []);
        setPickedUpBalloons(Array.isArray(pickedUp) ? pickedUp : []);
        setDeliveredBalloons(Array.isArray(delivered) ? delivered : []);
      } catch (error) {
        console.error('Error loading initial data:', error);
        setError('Failed to load balloon data. Please make sure the backend server is running.');
        setPendingBalloons([]);
        setPickedUpBalloons([]);
        setDeliveredBalloons([]);
      } finally {
        setIsLoading(false);
      }
    };

    loadInitialData();
    signalRService.startConnection();
    signalRService.onReceiveBalloonUpdates(handleBalloonUpdates);

    return () => {
      signalRService.offReceiveBalloonUpdates(handleBalloonUpdates);
      signalRService.stopConnection();
    };
  }, []);

  const loadPickedUpBalloons = async () => {
    try {
      const balloons = await getPickedUpBalloons();
      setPickedUpBalloons(Array.isArray(balloons) ? balloons : []);
    } catch (error) {
      console.error('Error loading picked up balloons:', error);
      setPickedUpBalloons([]);
    }
  };

  const loadDeliveredBalloons = async () => {
    try {
      const balloons = await getDeliveredBalloons();
      setDeliveredBalloons(Array.isArray(balloons) ? balloons : []);
    } catch (error) {
      console.error('Error loading delivered balloons:', error);
      setDeliveredBalloons([]);
    }
  };

  const handlePickup = async (balloon: BalloonRequestDTO) => {
    try {
      await updateBalloonStatus(balloon.id, {
        id: balloon.id,
        status: 'PickedUp',
        deliveredBy: userName
      });
    } catch (error) {
      console.error('Error picking up balloon:', error);
    }
  };

  const handleDelivery = async (balloon: BalloonRequestDTO) => {
    try {
      await updateBalloonStatus(balloon.id, {
        id: balloon.id,
        status: 'Delivered',
        deliveredBy: userName
      });
    } catch (error) {
      console.error('Error delivering balloon:', error);
    }
  };

  const handleOpenSettingsDialog = () => {
    setSettingsDialogOpen(true);
  };

  const handleCloseSettingsDialog = () => {
    setSettingsDialogOpen(false);
  };

  const handleSaveSettings = () => {
    localStorage.setItem('userName', userName);
    handleCloseSettingsDialog();
  };

  const renderBalloonList = (balloons: BalloonRequestDTO[], showActions = true) => (
    <List>
      {(Array.isArray(balloons) ? balloons : []).map((balloon) => (
        <ListItem
          key={balloon.id}
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
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Typography variant="h6">
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
              </Box>
            }
            secondary={
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Submitted at: {new Date(balloon.timestamp).toLocaleString()}
                </Typography>
                {balloon.pickedUpAt && (
                  <Typography variant="body2" color="text.secondary">
                    Picked up by: {balloon.pickedUpBy} at {new Date(balloon.pickedUpAt).toLocaleString()}
                  </Typography>
                )}
                {balloon.deliveredAt && (
                  <Typography variant="body2" color="text.secondary">
                    Delivered by: {balloon.deliveredBy} at {new Date(balloon.deliveredAt).toLocaleString()}
                  </Typography>
                )}
              </Box>
            }
          />
          {showActions && (
            <ListItemSecondaryAction>
              {balloon.status === 'Pending' && (
                <Button
                  variant="contained"
                  color="primary"
                  onClick={() => handlePickup(balloon)}
                  sx={{ mr: 1 }}
                >
                  Pick Up
                </Button>
              )}
              {balloon.status === 'PickedUp' && (
                <Button
                  variant="contained"
                  color="success"
                  onClick={() => handleDelivery(balloon)}
                >
                  Deliver
                </Button>
              )}
            </ListItemSecondaryAction>
          )}
        </ListItem>
      ))}
    </List>
  );

  return (
    <Container maxWidth="md">
      <Box sx={{ mt: 4 }}>
        <Paper elevation={3} sx={{ p: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Typography variant="h4">
                Balloon Requests
              </Typography>
              {userName && (
                <Chip
                  label={`Signed in as: ${userName}`}
                  color="primary"
                  variant="outlined"
                />
              )}
            </Box>
            <Tooltip title="Change your name">
              <IconButton onClick={handleOpenSettingsDialog}>
                <SettingsIcon />
              </IconButton>
            </Tooltip>
          </Box>
          {isLoading ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
              <CircularProgress />
            </Box>
          ) : error ? (
            <Box sx={{ p: 3, bgcolor: 'error.light', color: 'error.contrastText', borderRadius: 1 }}>
              <Typography>{error}</Typography>
              <Button 
                variant="contained" 
                onClick={() => window.location.reload()}
                sx={{ mt: 2 }}
              >
                Retry
              </Button>
            </Box>
          ) : (
            <>
              <Tabs
                value={activeTab}
                onChange={(_, newValue) => setActiveTab(newValue)}
                sx={{ mb: 3 }}
              >
                <Tab label={`Pending (${pendingBalloons.length})`} />
                <Tab label={`Picked Up (${pickedUpBalloons.length})`} />
                <Tab label={`Delivered (${deliveredBalloons.length})`} />
              </Tabs>

              {activeTab === 0 && renderBalloonList(pendingBalloons)}
              {activeTab === 1 && renderBalloonList(pickedUpBalloons)}
              {activeTab === 2 && renderBalloonList(deliveredBalloons, false)}
            </>
          )}
        </Paper>
      </Box>

      <Dialog open={settingsDialogOpen} onClose={handleCloseSettingsDialog}>
        <DialogTitle>Settings</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Your Name"
            fullWidth
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            helperText="This name will be used for all status changes"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseSettingsDialog}>Cancel</Button>
          <Button onClick={handleSaveSettings} variant="contained">
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};