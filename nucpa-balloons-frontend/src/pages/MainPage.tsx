import { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  Box,
  Tabs,
  Tab,
  CircularProgress,
  IconButton,
  Tooltip,
  Chip,
  Button,
} from '@mui/material';
import { Settings as SettingsIcon } from '@mui/icons-material';
import { signalRService } from '../services/signalR';
import { BalloonRequestDTO } from '../types';
import { getPendingBalloons, getPickedUpBalloons, getDeliveredBalloons, updateBalloonStatus } from '../services/api';
import { BalloonList } from '../components/BalloonList';
import { SettingsDialog } from '../components/SettingsDialog';

export const MainPage = () => {
  const [pendingBalloons, setPendingBalloons] = useState<BalloonRequestDTO[]>([]);
  const [pickedUpBalloons, setPickedUpBalloons] = useState<BalloonRequestDTO[]>([]);
  const [deliveredBalloons, setDeliveredBalloons] = useState<BalloonRequestDTO[]>([]);
  const [activeTab, setActiveTab] = useState(0);
  const [settingsDialogOpen, setSettingsDialogOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [userName, setUserName] = useState(() => localStorage.getItem('userName') || '');
  const [isSignalRConnected, setIsSignalRConnected] = useState(false);

  useEffect(() => {
    let mounted = true;

    const handleBalloonStatusChange = (updates: { Pending: BalloonRequestDTO[], PickedUp: BalloonRequestDTO[], Delivered: BalloonRequestDTO[] }) => {
      console.log('Received balloon status change:', updates);
      if (!mounted) return;

      if (Array.isArray(updates.Pending)) setPendingBalloons(updates.Pending);
      if (Array.isArray(updates.PickedUp)) setPickedUpBalloons(updates.PickedUp);
      if (Array.isArray(updates.Delivered)) setDeliveredBalloons(updates.Delivered);
    };

    const initializeSignalR = async () => {
      try {
        await signalRService.startConnection();
        if (mounted) {
          setIsSignalRConnected(true);
          signalRService.onBalloonStatusChanged(handleBalloonStatusChange);
        }
      } catch (error) {
        console.error('Failed to initialize SignalR:', error);
        if (mounted) {
          setError('Failed to establish real-time connection. Updates may be delayed.');
        }
      }
    };

    const loadInitialData = async () => {
      if (!mounted) return;
      setIsLoading(true);
      setError(null);
      try {
        const [pending, pickedUp, delivered] = await Promise.all([
          getPendingBalloons(),
          getPickedUpBalloons(),
          getDeliveredBalloons(),
        ]);
        if (!mounted) return;
        setPendingBalloons(Array.isArray(pending) ? pending : []);
        setPickedUpBalloons(Array.isArray(pickedUp) ? pickedUp : []);
        setDeliveredBalloons(Array.isArray(delivered) ? delivered : []);
      } catch (error) {
        console.error('Error loading initial data:', error);
        if (mounted) {
          setError('Failed to load balloon data. Please make sure the backend server is running.');
          setPendingBalloons([]);
          setPickedUpBalloons([]);
          setDeliveredBalloons([]);
        }
      } finally {
        if (mounted) {
          setIsLoading(false);
        }
      }
    };

    loadInitialData();
    initializeSignalR();

    return () => {
      mounted = false;
      if (signalRService.isConnected()) {
        signalRService.offBalloonStatusChanged(handleBalloonStatusChange);
        signalRService.stopConnection();
      }
    };
  }, []);

  const refreshData = async () => {
    const [pending, pickedUp, delivered] = await Promise.all([
      getPendingBalloons(),
      getPickedUpBalloons(),
      getDeliveredBalloons(),
    ]);
    setPendingBalloons(Array.isArray(pending) ? pending : []);
    setPickedUpBalloons(Array.isArray(pickedUp) ? pickedUp : []);
    setDeliveredBalloons(Array.isArray(delivered) ? delivered : []);
  };

  const handlePickup = async (balloon: BalloonRequestDTO) => {
    try {
      await updateBalloonStatus(balloon.id, {
        id: balloon.id,
        status: 'PickedUp',
        statusChangedBy: userName
      });
      // The UI will be updated through SignalR
    } catch (error) {
      console.error('Error picking up balloon:', error);
      await refreshData(); // Fallback to manual refresh on error
    }
  };

  const handleDelivery = async (balloon: BalloonRequestDTO) => {
    try {
      await updateBalloonStatus(balloon.id, {
        id: balloon.id,
        status: 'Delivered',
        statusChangedBy: userName
      });
      // The UI will be updated through SignalR
    } catch (error) {
      console.error('Error delivering balloon:', error);
      await refreshData(); // Fallback to manual refresh on error
    }
  };

  const handleRevert = async (balloon: BalloonRequestDTO) => {
    try {
      await updateBalloonStatus(balloon.id, {
        id: balloon.id,
        status: 'PickedUp',
        statusChangedBy: userName
      });
      // The UI will be updated through SignalR
    } catch (error) {
      console.error('Error reverting balloon:', error);
      await refreshData(); // Fallback to manual refresh on error
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

              {activeTab === 0 && (
                <BalloonList
                  balloons={pendingBalloons}
                  onPickup={handlePickup}
                />
              )}
              {activeTab === 1 && (
                <BalloonList
                  balloons={pickedUpBalloons}
                  onDelivery={handleDelivery}
                />
              )}
              {activeTab === 2 && (
                <BalloonList
                  balloons={deliveredBalloons}
                  onRevert={handleRevert}
                />
              )}
            </>
          )}
        </Paper>
      </Box>

      <SettingsDialog
        open={settingsDialogOpen}
        userName={userName}
        onClose={handleCloseSettingsDialog}
        onSave={handleSaveSettings}
        onUserNameChange={setUserName}
      />
    </Container>
  );
};