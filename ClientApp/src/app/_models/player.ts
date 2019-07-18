import { User } from './user';

export interface Player {
  user: User;
  leftGame: boolean;
  numberOfCards: number;
  roundsWonCount: number;
}
