import { CardValue, GameType, PlayersSetup } from './enums';

export interface GameSetup {
  password: string;
  bannedCards: CardValue[];
  numberOfStandardDecks: number;
  roundsToWin: number;
  gameType: GameType;
  playersSetup: PlayersSetup;
  drawFourDrawTwoShouldSkipTurn: boolean;
  reverseShouldSkipTurnInTwoPlayers: boolean;
  matchingCardStealsTurn:boolean;
  wildCardCanBePlayedOnlyIfNoOtherOptions:boolean;
  maxNumberOfPlayers: number;
  canSeeTeammatesHandInTeamGame: boolean;
  drawAutoPlay: boolean;
  spectatorsCanViewHands: boolean;
  limitColorChangingCards: boolean;
}
