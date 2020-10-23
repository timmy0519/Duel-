# Duel!

The original file would be pretty huge(600MB).

You can download the whole project here:
https://drive.google.com/drive/folders/1xFWrp96fMyeQ-mUsdGIquDf0ldRqSbd9

Click on the d.exe to play!
## Introduction

It's a board game inspired by the board game that only appeared in one of the episode of the famous anime "Yu-gi-oh!". It's called "Dungeon Dice Monsters" 

![](https://i.imgur.com/VeAWcuD.png)

From wiki:
> "Dungeon Dice monsters was made as a real game in early 2002 as Yu-Gi-Oh!: Dungeondice Monsters, with 137 figures and cards scheduled for release. However, it never really caught on and was canceled within a few months."'

My teammates and I tried to design a similar game to shout out to this unique game which lives in our childhood memory.

## How to play?

### Main Objective
Each player has three lives that appears at the top of the castles they belong to. Each player want to attack the opponent's castle by their monsters to win. Regardless of the monster's "Attack attribute", everytime a monster successfully attacks the castle, the opponent's life minus by 1 and it would show on the castle as well.

![](https://i.imgur.com/izd2hq8.png)

### Build your path to your success!
At every turn, player would want to "expand" its path to the opponent's castle. After putting the land on the board, the player can summon the monster of the card he/she has in his/her hand.

### Cards in your hand
Each turn of the drawing phase, they can draw a card that specifies which kind of monster they can summon.

On the left corner, it's the block the player can expand by summonning this monster. 
(we can rotate it just like we did in Tetris battle)

On the right corner, it shows the monster's name.

In the bottom section, there are Health Point(HP) and its attack power. They would show in green and white color respectively.

![](https://i.imgur.com/SQVisw8.png)

![](https://i.imgur.com/u18xrdT.png)



Although a player can only expand the path if the block is placing next to his/her own path(distinguished by the color) and also can summon the monster on his/her land, the monster can walk to any land placed by both players. 

![](https://i.imgur.com/s6lyQSg.jpg)

That is, monster can walk to any path as long as it was built on the board, but it can only summon on the belonger's land initially.

The picture below shows that the monster from player blue walked to the land the player red placed. 

The attack power of fireball is 5, and therefore after the attack, the monster from the red reduced to 0(shows in red). Thus, it would be destroyed.

![](https://i.imgur.com/kwOU8Ql.png)

## AI behavior

Current behavior of AI is straightfoward. It tries to place the land that is closet to our castle. Also, the main objective of summonning a monster is also attack the castle instead of attacking our monsters.

It's intuitive. Since only one monster can attack at a time, we should better attack the castle, which is our main objective. No matter how strong the monster is, it can only cause one damage to the castle. Therefore, it's a matter of speed to approach the opponent's castle.


