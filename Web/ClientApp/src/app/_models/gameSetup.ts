import { CardValue, GameType, PlayersSetup } from './enums';

export interface GameSetup {
  password: string;
  bannedCards: CardValue[];
  roundsToWin: number;
  gameType: GameType;
  playersSetup: PlayersSetup;
  drawFourDrawTwoShouldSkipTurn: boolean;
  reverseShouldSkipTurnInTwoPlayers: boolean;
  matchingCardStealsTurn:boolean;
  wildCardCanBePlayedOnlyIfNoOtherOptions:boolean;
  maxNumberOfPlayers: number;
}
