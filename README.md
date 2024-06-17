# Monopoly3D

Created Monopoly (the classic game) in Unity but implemented the 3D version (impressive animations) using a Barbie inspired theme by a Barbie inspired team: [Caraiman Ana](https://github.com/AnaCaraiman), [Caramaliu Nicoleta](https://github.com/NicoletaCaramaliu), [Manolache Diana](https://github.com/DianaManolache), [Radu Raluca](https://github.com/RaduRalucag) and [Vatau Lorena](https://github.com/vataulorena).         

### Demo
You can check out our demo [here](https://www.youtube.com/).
## Requirements 
### User stories and backlog creation

* #### User stories
  
Our team aims to develop a game that combines the features of a classic Monopoly game with the twist of some awesome animations.
The user stories that we managed to come up with are:
1.	The game allows multiple players: humans and bots, you decide the number of each.
2.	All players can roll the dice and move the characters around the board.
3.	The bank manages all transactions, money and properties, and can never run out of money.
4.	A player can purchase a piece of property when they land on an un-owned parcel. The price depends upon the neighborhood.
5.	Players must pay rent to the owner when they land on a piece of property that is already owned. The cost depends on the property and the number/type of improvements.
6.	When a player rounds the board, they receive 500 ron from the bank.
7.	If a player owes more than he can pay, he is bankrupt.
8.	The game ends  when only one player is left.
9.	Players land in jail when they land on the "Go To Jail" space, they draw a "Go to Jail" card, or they roll three doubles in a row.
10.	Players get can get out of jail by throwing doubles on any of the next three turns, using the "Get out of Jail Free" card if they have it, purchasing the "Get out of Jail Free" card from another player, or paying the $50 fine on either of the next 2 turns. If, on the third turn, the player doesn't roll doubles, she pays the $50 fine and moves forward the number of spaces rolled.
11.	When a player lands on "Go" by direct count, they receive 500 ron.
12.	When a player lands on the "Free Parking" space, they collect all the tax money, jail money, etc. that were collected until that moment.
13.	Players go to jail when they land on the "go to jail" space.
14.	Players draw a card from the chest pile when they land on the corresponding square.
15. Players can invoke trades between properties and negociate.
16. Players can aplly mortgage on their properties and take back the mortgage during their turns.
17. Players can buy houses and hotels to their properties. Houses have to be bought evenly (the maximum difference between the number of houses on properties must be 1).
18. In the first round, everybody has 1500 RON.




* #### Backlog - this is how we scheduled our work

![uml](https://github.com/AnaCaraiman/Unopoly/assets/116754655/8130acca-2649-4124-9001-9e952eb992f2)
-----------------------
### Diagrams

* The Workflow Diagram - it explains the steps involved from the beginning of the game to the end so the user can understand it better

  
![WorkFlow](https://github.com/AnaCaraiman/Unopoly/assets/116754655/90e17235-fa4a-4d83-abe3-8eefbc527787)



* The UML Diagram

![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/cafc884e-62d1-404e-a32c-6b9e1f3ec9b5)


-----------------------


### Source control using git

We used GitHub to synchronize our code. Everyone has its own branch (we also added a few more to stash some work to avoid conflicts) and you can check them out [here](https://github.com/AnaCaraiman/Unopoly/branches).
You can also view the [pull requests and the commits](https://github.com/AnaCaraiman/Unopoly/commits/main/).

-----------------------
### Bugs and pull requests

You can check out some of the bugs and how we solved some of them [here](https://github.com/AnaCaraiman/Unopoly/issues).
Also, a lot of problems were solved when we applied merge pull request from one branch to another and you can see some examples [here](https://github.com/AnaCaraiman/Unopoly/commits/diana/).

-----------------------
### Refactoring

Example of refactoring and [link](https://github.com/AnaCaraiman/Unopoly/commit/de224d6090442b860c86ed9a84c4c8757d578bfa) where you can find them.

![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/36f2d6c3-31d5-4106-a9d5-411f3bc7a86e)



-----------------------
### Comments

While coding, we inserted comments to know what to complete later and it to make it easier for the rest of the team to understand the code.
![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/d466aa0f-509b-4b6e-81f5-8a2785492833)

You can find comments in whatever class you open from the [source](https://github.com/AnaCaraiman/Unopoly/tree/main/Unopoly/Assets/Scripts).

-----------------------
### Design patterns

We used some design patterns such as:
* singleton (for the MonopolyBoard class)
![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/ab5d2232-e693-44a8-b042-d76f920afb4a)

* observer (UpdateMoneyText, UpdateSystemMessage and ManageUI - they act as observers to update the user interface when the state of the game is changing)
![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/38d4816f-2306-4b35-80be-e1093d335914)
![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/4be8d016-9369-4bf1-a6c9-d7133e17f05f)
![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/dfd253a9-6808-47bb-84aa-495eef953774)




-----------------------
### GitHub Copilot
Used the GitHub Copilot when coding in Visual Studio to help us avoiding mistakes and for suggestions (even though sometimes it messed up our code).
![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/df718d9d-1117-457a-9fe5-34e4bbdd7197)
![image](https://github.com/AnaCaraiman/Unopoly/assets/116754655/ae144081-35c9-4e3a-bd07-a5a0ef26d084)

## Please don't mind if you lose, it's just a game! :D
