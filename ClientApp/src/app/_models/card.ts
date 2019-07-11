import { CardColor, CardValue } from './enums';

export interface Card {
  imageUrl: string;
  cardColor: CardColor;
  cardValue: CardValue;
}
