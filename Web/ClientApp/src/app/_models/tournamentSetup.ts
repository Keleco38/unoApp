import { GameType, CardValue } from './enums';
import { Card } from "./card";

export interface TournamentSetup{
    password:string;
    numberOfPlayers:number;
    bannedCards:CardValue[];
    roundsToWin:number;
    gameType:GameType;
    drawFourDrawTwoShouldSkipTurn: boolean;
    reverseShouldSkipTurnInTwoPlayers: boolean;
    matchingCardStealsTurn:boolean;
    wildCardCanBePlayedOnlyIfNoOtherOptions:boolean;
}