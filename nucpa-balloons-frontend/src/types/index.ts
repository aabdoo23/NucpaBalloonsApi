export interface BalloonRequestDTO {
  id: string;
  submissionId: number;
  teamId: string;
  teamName: string;
  problemIndex: string;
  balloonColor: string;
  status: string;
  timestamp: string;
  statusChangedAt?: string;
  statusChangedBy?: string;
}

export interface BalloonStatusUpdateRequest {
  id: string;
  status: 'Pending' | 'PickedUp' | 'Delivered';
  statusChangedBy?: string;
}

export interface AdminSettingsResponse {
  $id: string;
  $values: AdminSettings[];
}

export interface AdminSettings {
  id: string;
  adminUsername: string;
  contestId: string;
  codeforcesApiKey?: string;
  codeforcesApiSecret?: string;
  isEnabled: boolean;
  teams: Team[];
  rooms: Room[];
  problemBalloonMaps: ProblemBalloonMap[];
}

export interface Room {
  id: string;
  capacity?: number;
  isAvailable?: boolean;
  adminSettingsId: string;
}

export interface Team {
  id: string;
  codeforcesHandle: string;
  roomId: string;
  adminSettingsId: string;
}

export interface ProblemBalloonMap {
  id: string;
  adminSettingsId: string;
  problemIndex: string;
  balloonColor: string;
}

export interface LoginRequest {
  username: string;
  password: string;
} 