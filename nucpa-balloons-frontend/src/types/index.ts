export interface BalloonRequestDTO {
  id: string;
  submissionId: number;
  teamId: string;
  teamName: string;
  problemIndex: string;
  balloonColor: string;
  status: string;
  timestamp: string;
  deliveredAt?: string;
  deliveredBy?: string;
  pickedUpAt?: string;
  pickedUpBy?: string;
}

export interface BalloonStatusUpdateRequest {
  id: string;
  status: 'Pending' | 'PickedUp' | 'Delivered';
  deliveredBy?: string;
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