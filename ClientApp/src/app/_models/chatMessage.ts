import { TypeOfMessage } from './enums';

export interface ChatMessage {
  username: string;
  text: string;
  typeOfMessage: TypeOfMessage;
}
