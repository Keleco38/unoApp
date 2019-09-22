export interface TournamentList {
  id: string;
  numberOfPlayers: number;
  requiredNumberOfPlayers: number;
  isPasswordProtected: boolean;
  name: string;
  tournamentStarted: boolean;
}
