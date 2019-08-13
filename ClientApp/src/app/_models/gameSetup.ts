import { GameMode } from './enums';

export interface GameSetup {
  isPasswordProtected: boolean;
  gameMode: GameMode;
  roundsToWin: number;
}
