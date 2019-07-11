import { GameMode } from './enums';

export interface GameSetup {
  id: string;
  isPasswordProtected: boolean;
  gameMode: GameMode;
}
