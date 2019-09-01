export interface GameList {
  id: string;
  numberOfPlayers: number;
  maxNumberOfPlayers: number;
  isPasswordProtected: boolean;
  host: string;
  gameStarted: boolean;
}
