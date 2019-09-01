export interface GameList {
  id: string;
  numberOfPlayers: number;
  isPasswordProtected: boolean;
  host: string;
  gameStarted: boolean;
}
