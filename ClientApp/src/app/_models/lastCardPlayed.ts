import { CardValue, CardColor } from './enums';

export interface LastCardPlayed {
  imageUrl: string;
  color: CardColor;
  value: CardValue;
  playerPlayed: string;
}
