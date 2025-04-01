import { HubConnectionBuilder, HubConnection, HttpTransportType } from '@microsoft/signalr';
import { BalloonRequestDTO } from '../types';

interface BalloonUpdates {
  Pending: BalloonRequestDTO[];
  PickedUp: BalloonRequestDTO[];
  Delivered: BalloonRequestDTO[];
}

class SignalRService {
  private connection: HubConnection | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;

  public async startConnection() {
    if (this.connection) {
      console.log('SignalR connection already exists');
      return;
    }

    try {
      console.log('Starting SignalR connection...');
      this.connection = new HubConnectionBuilder()
        .withUrl('/api/balloonHub', {
          withCredentials: true,
          skipNegotiation: true,
          transport: HttpTransportType.WebSockets
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .build();

      this.connection.onreconnecting((error) => {
        console.log('SignalR reconnecting...', error);
        this.reconnectAttempts++;
      });

      this.connection.onreconnected((connectionId) => {
        console.log('SignalR reconnected with connection ID:', connectionId);
        this.reconnectAttempts = 0;
      });

      this.connection.onclose((error) => {
        console.log('SignalR connection closed:', error);
        if (this.reconnectAttempts >= this.maxReconnectAttempts) {
          console.log('Max reconnection attempts reached. Stopping reconnection.');
          this.stopConnection();
        }
      });

      await this.connection.start();
      console.log('SignalR connection started successfully with ID:', this.connection.connectionId);
    } catch (error) {
      console.error('Error starting SignalR connection:', error);
      this.stopConnection();
    }
  }

  public async stopConnection() {
    if (this.connection) {
      try {
        await this.connection.stop();
        this.connection = null;
        this.reconnectAttempts = 0;
        console.log('SignalR connection stopped');
      } catch (error) {
        console.error('Error stopping SignalR connection:', error);
      }
    }
  }

  public onReceiveBalloonUpdates(callback: (updates: BalloonUpdates) => void) {
    if (!this.connection) {
      console.error('Cannot register callback: SignalR connection not established');
      return;
    }

    console.log('Registering ReceiveBalloonUpdates callback...');
    this.connection.on('ReceiveBalloonUpdates', (updates: BalloonUpdates) => {
      console.log('Received balloon updates through SignalR:', updates);
      callback(updates);
    });
    console.log('ReceiveBalloonUpdates callback registered');
  }

  public onBalloonStatusChanged(callback: (updates: BalloonUpdates) => void) {
    if (!this.connection) {
      console.error('Cannot register callback: SignalR connection not established');
      return;
    }

    console.log('Registering BalloonStatusChanged callback...');
    this.connection.on('BalloonStatusChanged', (updates: BalloonUpdates) => {
      console.log('Received balloon status change through SignalR:', updates);
      callback(updates);
    });
    console.log('BalloonStatusChanged callback registered');
  }

  public offBalloonStatusChanged(callback: (updates: BalloonUpdates) => void) {
    if (!this.connection) {
      console.error('Cannot unregister callback: SignalR connection not established');
      return;
    }

    this.connection.off('BalloonStatusChanged', callback);
    console.log('Unregistered BalloonStatusChanged callback');
  }

  public offReceiveBalloonUpdates(callback: (updates: BalloonUpdates) => void) {
    if (!this.connection) {
      console.error('Cannot unregister callback: SignalR connection not established');
      return;
    }

    this.connection.off('ReceiveBalloonUpdates', callback);
    console.log('Unregistered ReceiveBalloonUpdates callback');
  }

  public isConnected(): boolean {
    return this.connection?.state === 'Connected';
  }
}

export const signalRService = new SignalRService();