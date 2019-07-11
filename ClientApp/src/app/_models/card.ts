import { CardColor, CardValue } from './enums';

export interface Card {
  imageUrl: string;
  color: CardColor;
  value: CardValue;
}
