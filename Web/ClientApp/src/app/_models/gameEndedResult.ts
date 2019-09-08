import { HallOfFame } from './hallOfFame';
export interface GameEndedResult{
    playersWon:string[];
    pointsWon:number;
    hallOfFameStats:HallOfFame[]
}