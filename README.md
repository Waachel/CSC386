# CSC486 - Real-Time Networking
## Description
CSC486 focused on real-time networking fundamentals. Topics included low-level TCP/UDP socket programming, serialization of data for network transportation, deterministic data flow and queuing in applications, dynamic configuration and session communications.
## Project Descriptions
* Simple and complex De/Serialization (Assignment 1)
  * Simple and complex de/serialization
    * char, int, float, double
    * char* and class/struct*
  * Data ordering, padding, alignment
* TCP AND UDP Socket Programming (Assignment 2 and 3)
  * TCP Client-Server application
  * UDP Client-Server application
  * Handling UDP communication with simulated network problems
* Refactored Networking System (Assignment 4)
  * Capture the Flag Game using UDP with ACK
  * Data driven message queue that supports mutiple object types
* Omega Race (Client-Server Game)
  * Data-driven input/output queues
  * Game state synchronization
  * Record and playback tool
  * Player prediction and dead reckoning prediction algorithms
 
## Instructions To Run Omega Race
* Open Omega Race/Dead Reckoning or Omega Race/Player Prediction
* Open OmegaRace Client (Player 1)
* Run OmegaRace.sln
* Open OmegaRace Server (Player 2)
* Run OmegaRace.sln
* The following scene is rendered for each player:
  ![image](https://github.com/user-attachments/assets/579fd1d7-f052-4a65-900e-24cb4362840b)
* Example of game play: </br>
  ![image](https://github.com/user-attachments/assets/20741315-b89a-4db2-b036-688fda939740)
* Player 1 Controls:
  * F to fire missiles
  * C to lay mines
  * A to turn left
  * D to turn right
  * W to accelerate
  * SPACE to play
* Player 2 Controls:
  * H to fire missiles
  * N to lay mines
  * J to turn left
  * L to turn right
  * I to accelerate
  * SPACE to play
    
**NOTE 1:** Current setup assumes Server and Client are on the same host. In this configuration in order for a player to move or fire, their window has to be active. </br>
**NOTE 2:** Order of running Client and Server does not matter until players start the game. When starting, Server (Player 2) should press SPACE first.

* Click here to view a [full walkthrough](https://www.youtube.com/watch?v=V-xvuM4aAu8) of my game
