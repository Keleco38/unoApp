import {  CardValue } from './enums';

export interface GameSetup {
  isPasswordProtected: boolean;
  bannedCards: CardValue[];
  roundsToWin: number;
}
