import { CardColor, CardValue } from './enums';

export interface Card {
  id:string;
  imageUrl: string;
  color: CardColor;
  value: CardValue;
}
