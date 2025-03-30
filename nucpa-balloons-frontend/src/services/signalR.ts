import { HubConnectionBuilder, HubConnection, HttpTransportType } from '@microsoft/signalr';
import { BalloonRequestDTO } from '../types';

class SignalRService {
  private connection: HubConnection | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;

  public async startConnection() {
    if (this.connection) return;

    try {
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
      console.log('SignalR connection started successfully');
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

  public onReceiveBalloonUpdates(callback: (balloons: BalloonRequestDTO[]) => void) {
    if (!this.connection) return;

    this.connection.on('ReceiveBalloonUpdates', callback);
    console.log('Registered ReceiveBalloonUpdates callback');
  }

  public offReceiveBalloonUpdates(callback: (balloons: BalloonRequestDTO[]) => void) {
    if (!this.connection) return;

    this.connection.off('ReceiveBalloonUpdates', callback);
    console.log('Unregistered ReceiveBalloonUpdates callback');
  }
}

export const signalRService = new SignalRService(); 