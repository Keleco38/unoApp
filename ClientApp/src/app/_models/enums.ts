export enum TypeOfMessage {
  chat = 1,
  server = 2,
  spectators = 3
}

export enum CardColor {
  blue = 1,
  green = 2,
  red = 3,
  yellow = 4,
  wild = 5
}

export enum CardValue {
  zero = 0,
  one = 1,
  two = 2,
  three = 3,
  four = 4,
  five = 5,
  six = 6,
  seven = 7,
  eight = 8,
  nine = 9,
  reverse = 10,
  skip = 11,
  drawTwo = 12,
  drawFour = 13,
  changeColor = 14,
  blackHole = 15,
  discardAllWildCards = 16,
  stealTurn = 17,
  swapHands = 18

}

export enum Direction {
  right = 1,
  left = 2
}

export enum GameMode {
  normal = 1,
  specialCards = 2,
  specialCardsAndAvalonCards = 3
}
