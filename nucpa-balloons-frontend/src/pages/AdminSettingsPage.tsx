import { useState, useEffect } from 'react';
import {
  Container,
  Paper,
  Typography,
  TextField,
  Button,
  Switch,
  FormControlLabel,
  Box,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Chip,
} from '@mui/material';
import { Delete as DeleteIcon, Edit as EditIcon, Add as AddIcon } from '@mui/icons-material';
import {
  getAllAdminSettings,
  getActiveAdminSettings,
  updateAdminSettings,
  createAdminSettings,
  setActiveAdminSettings,
  getProblemBalloonMaps,
  createProblemBalloonMap,
  updateProblemBalloonMap,
  deleteProblemBalloonMap,
} from '../services/api';
import { AdminSettings, ProblemBalloonMap } from '../types';

export const AdminSettingsPage = () => {
  const [allSettings, setAllSettings] = useState<AdminSettings[]>([]);
  const [activeSettings, setActiveSettings] = useState<AdminSettings | null>(null);
  const [maps, setMaps] = useState<ProblemBalloonMap[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [openSettingsDialog, setOpenSettingsDialog] = useState(false);
  const [editingMap, setEditingMap] = useState<ProblemBalloonMap | null>(null);
  const [newMap, setNewMap] = useState<Omit<ProblemBalloonMap, 'id'> & { adminSettingsId: string }>({
    problemIndex: '',
    balloonColor: '',
    adminSettingsId: '',
  });
  const [newSettings, setNewSettings] = useState<Omit<AdminSettings, 'id'>>({
    adminUsername: '',
    contestId: '',
    codeforcesApiKey: '',
    codeforcesApiSecret: '',
    isEnabled: true,
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [settingsData, activeData, mapsData] = await Promise.all([
        getAllAdminSettings(),
        getActiveAdminSettings(),
        getProblemBalloonMaps(),
      ]);
      setAllSettings(Array.isArray(settingsData.$values) ? settingsData.$values : []);
      setActiveSettings(activeData);
      setMaps(Array.isArray(mapsData) ? mapsData : []);
    } catch (error) {
      console.error('Error loading data:', error);
      setAllSettings([]);
      setMaps([]);
    }
  };

  const handleSettingsChange = async (field: keyof AdminSettings, value: any) => {
    if (!activeSettings) return;

    const updatedSettings = { ...activeSettings, [field]: value };
    try {
      const response = await updateAdminSettings(updatedSettings);
      setActiveSettings(response);
      await loadData();
    } catch (error) {
      console.error('Error updating settings:', error);
    }
  };

  const handleOpenSettingsDialog = () => {
    setNewSettings({
      adminUsername: '',
      contestId: '',
      codeforcesApiKey: '',
      codeforcesApiSecret: '',
      isEnabled: true,
    });
    setOpenSettingsDialog(true);
  };

  const handleCloseSettingsDialog = () => {
    setOpenSettingsDialog(false);
    setNewSettings({
      adminUsername: '',
      contestId: '',
      codeforcesApiKey: '',
      codeforcesApiSecret: '',
      isEnabled: true,
    });
  };

  const handleCreateSettings = async () => {
    try {
      await createAdminSettings(newSettings);
      handleCloseSettingsDialog();
      await loadData();
    } catch (error) {
      console.error('Error creating settings:', error);
    }
  };

  const handleSetActive = async (id: string) => {
    try {
      await setActiveAdminSettings(id);
      await loadData();
    } catch (error) {
      console.error('Error setting active settings:', error);
    }
  };

  const handleOpenDialog = (map?: ProblemBalloonMap) => {
    if (map) {
      setEditingMap(map);
      setNewMap({ 
        problemIndex: map.problemIndex, 
        balloonColor: map.balloonColor,
        adminSettingsId: map.adminSettingsId 
      });
    } else {
      setEditingMap(null);
      setNewMap({ 
        problemIndex: '', 
        balloonColor: '',
        adminSettingsId: activeSettings?.id || ''
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingMap(null);
    setNewMap({ 
      problemIndex: '', 
      balloonColor: '',
      adminSettingsId: activeSettings?.id || ''
    });
  };

  const handleSaveMap = async () => {
    try {
      if (editingMap) {
        await updateProblemBalloonMap({ ...newMap, id: editingMap.id });
      } else {
        if (!activeSettings?.id) {
          console.error('No active settings selected');
          return;
        }
        await createProblemBalloonMap(newMap);
      }
      await loadData();
      handleCloseDialog();
    } catch (error) {
      console.error('Error saving map:', error);
    }
  };

  const handleDeleteMap = async (id: string) => {
    try {
      await deleteProblemBalloonMap(id);
      await loadData();
    } catch (error) {
      console.error('Error deleting map:', error);
    }
  };

  return (
    <Container maxWidth="md">
      <Box sx={{ mt: 4 }}>
        <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <Typography variant="h5">All Settings</Typography>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={handleOpenSettingsDialog}
            >
              Add New Settings
            </Button>
          </Box>
          <List>
            {allSettings.map((settings) => (
              <ListItem 
                key={settings.id}
                sx={{
                  bgcolor: settings.isEnabled ? 'action.selected' : 'transparent',
                  '&:hover': {
                    bgcolor: 'action.hover',
                  },
                }}
              >
                <ListItemText
                  primary={
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Typography variant="subtitle1">
                        Contest ID: {settings.contestId}
                      </Typography>
                      {settings.isEnabled && (
                        <Chip 
                          label="Active" 
                          color="success" 
                          size="small"
                          sx={{ ml: 1 }}
                        />
                      )}
                    </Box>
                  }
                  secondary={`Admin: ${settings.adminUsername}`}
                />
                <ListItemSecondaryAction>
                  {!settings.isEnabled && (
                    <Button
                      variant="outlined"
                      size="small"
                      onClick={() => handleSetActive(settings.id)}
                      sx={{ mr: 1 }}
                    >
                      Set Active
                    </Button>
                  )}
                  <IconButton
                    edge="end"
                    onClick={() => {
                      setActiveSettings(settings);
                      handleOpenDialog();
                    }}
                  >
                    <EditIcon />
                  </IconButton>
                </ListItemSecondaryAction>
              </ListItem>
            ))}
          </List>
        </Paper>

        {activeSettings && (
          <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
            <Typography variant="h5" gutterBottom>
              Active Settings
            </Typography>
            <TextField
              fullWidth
              label="Admin Username"
              value={activeSettings.adminUsername}
              onChange={(e) => handleSettingsChange('adminUsername', e.target.value)}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Contest ID"
              value={activeSettings.contestId}
              onChange={(e) => handleSettingsChange('contestId', e.target.value)}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Codeforces API Key"
              value={activeSettings.codeforcesApiKey || ''}
              onChange={(e) => handleSettingsChange('codeforcesApiKey', e.target.value)}
              margin="normal"
              type="password"
            />
            <TextField
              fullWidth
              label="Codeforces API Secret"
              value={activeSettings.codeforcesApiSecret || ''}
              onChange={(e) => handleSettingsChange('codeforcesApiSecret', e.target.value)}
              margin="normal"
              type="password"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={activeSettings.isEnabled}
                  onChange={(e) => handleSettingsChange('isEnabled', e.target.checked)}
                />
              }
              label="Enable Contest"
            />
          </Paper>
        )}

        <Paper elevation={3} sx={{ p: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
            <Typography variant="h5">Problem-Balloon Mapping</Typography>
            <Button variant="contained" onClick={() => handleOpenDialog()}>
              Add New Mapping
            </Button>
          </Box>
          <List>
            {maps.map((map) => (
              <ListItem key={map.id}>
                <ListItemText
                  primary={`Problem ${map.problemIndex}`}
                  secondary={`Balloon Color: ${map.balloonColor}`}
                />
                <ListItemSecondaryAction>
                  <IconButton
                    edge="end"
                    onClick={() => handleOpenDialog(map)}
                    sx={{ mr: 1 }}
                  >
                    <EditIcon />
                  </IconButton>
                  <IconButton
                    edge="end"
                    onClick={() => handleDeleteMap(map.id)}
                  >
                    <DeleteIcon />
                  </IconButton>
                </ListItemSecondaryAction>
              </ListItem>
            ))}
          </List>
        </Paper>
      </Box>

      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>
          {editingMap ? 'Edit Mapping' : 'Add New Mapping'}
        </DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Problem Index"
            value={newMap.problemIndex}
            onChange={(e) =>
              setNewMap({ ...newMap, problemIndex: e.target.value })
            }
            margin="normal"
          />
          <TextField
            fullWidth
            label="Balloon Color"
            value={newMap.balloonColor}
            onChange={(e) =>
              setNewMap({ ...newMap, balloonColor: e.target.value })
            }
            margin="normal"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button onClick={handleSaveMap} variant="contained">
            Save
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openSettingsDialog} onClose={handleCloseSettingsDialog}>
        <DialogTitle>Add New Settings</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Admin Username"
            value={newSettings.adminUsername}
            onChange={(e) =>
              setNewSettings({ ...newSettings, adminUsername: e.target.value })
            }
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Contest ID"
            value={newSettings.contestId}
            onChange={(e) =>
              setNewSettings({ ...newSettings, contestId: e.target.value })
            }
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Codeforces API Key"
            value={newSettings.codeforcesApiKey}
            onChange={(e) =>
              setNewSettings({ ...newSettings, codeforcesApiKey: e.target.value })
            }
            margin="normal"
            type="password"
          />
          <TextField
            fullWidth
            label="Codeforces API Secret"
            value={newSettings.codeforcesApiSecret}
            onChange={(e) =>
              setNewSettings({ ...newSettings, codeforcesApiSecret: e.target.value })
            }
            margin="normal"
            type="password"
          />
          <FormControlLabel
            control={
              <Switch
                checked={newSettings.isEnabled}
                onChange={(e) =>
                  setNewSettings({ ...newSettings, isEnabled: e.target.checked })
                }
              />
            }
            label="Enable Contest"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseSettingsDialog}>Cancel</Button>
          <Button onClick={handleCreateSettings} variant="contained">
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
}; 