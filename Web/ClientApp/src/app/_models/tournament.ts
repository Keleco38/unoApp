import { Contestant } from './contestant';
import { TournamentRound } from './tournamentRound';
import { TournamentSetup } from './tournamentSetup';
import { User } from './user';
export interface Tournament {
  id: string;
  tournamentStarted: boolean;
  tournamentEnded: boolean;
  contestants: Contestant[];
  tournamentSetup: TournamentSetup;
  spectators: User[];
  tournamentRounds: TournamentRound[];
}