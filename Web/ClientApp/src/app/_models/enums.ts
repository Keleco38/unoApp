export enum TypeOfMessage {
  chat = 1,
  server = 2,
  spectators = 3
}
export enum ChatDestination {
  all = 1,
  tournament = 2,
  game = 3
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
  discardWildCards = 16,
  stealTurn = 17,
  swapHands = 18,
  doubleEdge = 19,
  discardColor = 20,
  handOfGod = 21,
  judgement = 22,
  unitedWeFall = 23,
  paradigmShift = 24,
  deflect = 25,
  inspectHand = 26,
  graveDigger = 27,
  russianRoulette = 28,
  roulette = 29,
  duel = 30,
  keepMyHand = 31,
  charity = 32,
  blackjack = 33,
  fairPlay = 34,
  tricksOfTheTrade = 35,
  discardNumber = 36,
  theLastStand = 37,
  magneticPolarity = 38,
  fortuneTeller=39,
  doubleDraw=40,
  poison=41,
  randomColor=42,
  promiseKeeper=43,
  gambling = 44,
  copyCat = 45
}

export enum Direction {
  right = 1,
  left = 2
}
export enum GameType {
  normal = 1,
  specialWildCards = 2
}

export enum PlayersSetup {
  individual = 1,
  teams = 2
}

