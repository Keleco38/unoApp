import {  CardValue } from './enums';

export interface GameSetup {
  isPasswordProtected: boolean;
  password:string;
  bannedCards: CardValue[];
  roundsToWin: number;
}
