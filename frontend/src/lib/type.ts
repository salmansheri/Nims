export interface Alert {
  id: number;
  title: string;
  description: string;
  severity: number;
  alertType: string; // Intrusion, System, Security
  isAcknowledged: boolean;
  acknowledgedById?: string | null;
  acknowledgedBy?: IdentityUser | null;
  acknowledgedAt?: Date | null;
  createdAt: Date;
}

export interface IntrusionDetection {
  id: number;
  sourceIP: string;
  destinationIP: string;
  sourcePort: number;
  destinationPort: number;
  protocol: string;
  attackType: string;
  payload: string;
  severity: number;
  detectedAt: Date;
  isResolved: boolean;
  resolvedById?: string;
  resolvedBy?: IdentityUser;
  resolvedAt?: Date;
}

export interface IdentityUser {
  // Define properties based on your IdentityUser class
  id: string;
  userName?: string;
  email?: string;
  // Add other properties as needed
}
