export interface TournamentList {
  id: string;
  numberOfPlayers: number;
  requiredNumberOfPlayers: number;
  isPasswordProtected: boolean;
  host: string;
  tournamentStarted: boolean;
}
