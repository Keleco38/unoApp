import { TournamentRoundGame } from './tournamentRoundGame';

export interface TournamentRound {
  roundNumber: number;
  tournamentRoundGames: TournamentRoundGame[];
}
