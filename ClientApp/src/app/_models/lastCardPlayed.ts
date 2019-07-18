import { CardValue, CardColor } from './enums';
import { Player } from './player';

export interface LastCardPlayed {
  imageUrl: string;
  color: CardColor;
  value: CardValue;
  playerPlayed: string;
}
