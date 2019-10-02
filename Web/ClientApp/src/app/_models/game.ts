import { LastCardPlayed } from './lastCardPlayed';
import { Direction } from './enums';
import { Player } from './player';
import { Spectator } from './spectator';
import { GameSetup } from './gameSetup';
import { Deck } from './deck';
import { Card } from './card';

export interface Game {
  id: string;
  gameSetup: GameSetup;
  players: Player[];
  spectators: Spectator[];
  playerToPlay: Player;
  gameEnded: boolean;
  gameStarted: boolean;
  deck: Deck;
  direction: Direction;
  lastCardPlayed: LastCardPlayed;
  discardedPile: Card[];
  readyPlayersLeft: string[];
  isTournamentGame:boolean;
}
