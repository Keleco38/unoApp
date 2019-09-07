import { CardValue, GameType } from './enums';

export interface GameSetup {
  password: string;
  bannedCards: CardValue[];
  roundsToWin: number;
  gameType: GameType;
  drawFourDrawTwoShouldSkipTurn: boolean;
  reverseShouldSkipTurnInTwoPlayers: boolean;
  matchingCardStealsTurn:boolean;
  wildCardCanBePlayedOnlyIfNoOtherOptions:boolean;
  maxNumberOfPlayers: number;
}
