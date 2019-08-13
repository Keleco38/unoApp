import { User } from './user';

export interface Player {
  id:string;
  user: User;
  leftGame: boolean;
  numberOfCards: number;
  roundsWonCount: number;
}
