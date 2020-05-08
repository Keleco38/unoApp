> Developed with: .net core 3.0 + angular8 + bootstrap 4.1

##### DEMO: http://uno-special-edition.com/

##### HELP: http://uno-special-edition.com/help

## Rules

There is some difference between this version of the game and the original. Here are the basics. (this only applies if the game mode selected is "special wild cards", if the selected mode is "normal" then the standard rules apply)

- The first player that runs out of card is a round winner. Host can change the number of rounds required to win the game.
- If the player has played a card, and as a result of that they are left with 1 card in his hand, they must call "uno", by pressing the blue button in the middle of the screen. If the player doesn't call uno, they are punished by drawing 2 cards.
- Player can only play a card if it's their turn to play (excluding steal turn card).
- Player can play colored card only if the color or the value on the card corresponds to the color or the value of the previously played card.
- Player can play steal turn card anytime if the color of the steal turn card corresponds to the previously played card.
- Player can play positive, neutral and gambling wildcards regardless of the previously played card. Wildcards allow player to pick desired color.
- Player can play negative wildcards only under certain condition (read card section below). If the condition is fulfilled then the player can pick desired color.
- Cards that are automatically played do not require action from the player (read card section below)

## Features

Basics features are explained below:

- Player can change their settings and name any time by clicking "User Settings" button in the navbar.
- Player can buzz another player by selecting the option from the dropdown next to other player's name (under online players section).
- Host can define game setup (password/max no of players/draw two-four rules/reverse skip in 2p/number of rounds to win/game type).
- Host can ban up to 10 cards from the game. Other player should give consent on the cards banned.
- Host can kick players from the game.
- In game, players can chat by opening chat window by pressing the "Chat" button on top.
- In game, players can see current game status by pressing "Game info" button on top.
- Once the game is started player can exit the game and reconnect back to it.
- In game, names on top of the screen show the players in the game. Blue colored name shows the player currently playing. If the border around the player's name is red, that means the player has exited the game.
- In game, number in the parentheses corresponds to the number of cards player has.
- In game, arrows next to the player's name corresponds to the current direction of the game.
- Player can mute spectator's and server's chat by selecting those checkboxed under the Misc. tab.
- Player can keep sidebar open and change sidebar's size by selecting those options under the Misc. tab.
- Player can top navigation bar when in game by selecting that option under Misc. tab.
- Player can mention another player by sending @name in chat where name is the name of the player.

## Cards

Deck is composed of 3 colored UNO decks (numbers, skip, draw two, reverse, ), 4 instances of +4 and pick color, 24 instances of steal turn, 8 instances of "Keep my hand" and "Deflect" cards, and 4 instance of every other special wild card.

There are many new cards added to this game. They mostly fall into 4 groups:
- Positive cards - Cards that have positive effect on the holder. Can be played anytime.
- Negative cards - Cards that have negative effect on the holder. Can be played only under special condition.
- Neutral cards - Cards that can have both positive or negative effect, depending on the setup of the game and the cards.
- Gambling cards - Cards that relay on the luck, mostly based on rolling a dice.

