// Capture The Flag
// AB 4/21

#include <iostream>
#include "CaptureTheFlag.h"
#include "NetworkManager.h"

void AdjustWindow()
{
	HWND hwndFound = GetConsoleWindow();
	RECT rc;
	GetWindowRect(GetDesktopWindow(), &rc);  // screen dimension in right and bottom
	
	// left (client) postion
	//MoveWindow(hwndFound, 0, 0, rc.right / 2, rc.bottom / 2, true);

	// Right (server) position
	MoveWindow(hwndFound, rc.right / 2, 0, rc.right / 2, rc.bottom / 2, true);
}

int main()
{
	// Uncomment to adjust window
	AdjustWindow(); // Positioning console window for convenience

	NetworkManager networkMan;
	networkMan.SetDuplicationProb(20);

	SOCKADDR_IN TargetAddr;
	TargetAddr.sin_family = AF_INET;

	SOCKADDR_IN serverAddr;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_addr.s_addr = INADDR_ANY;
	serverAddr.sin_port = htons(8888);

	networkMan.Bind(serverAddr);
	printf("Listening for incoming connections...\n");

	sockaddr_in SenderAddr = TargetAddr;			// used for recvFrom parameters...

	srand((unsigned int) GetTickCount64());

	CaptureTheFlag game;
	CaptureTheFlag::MoveData DataP1;
	CaptureTheFlag::MoveData DataP2;
	char winner = CTFField::NoOneMarker;

	while (winner == CTFField::NoOneMarker) // this is the "game loop"
	{
		// Update phase
		// Update part 1: process user inputs for P1
			//Player 1
			//DataP1.player = CaptureTheFlag::Player::Player1;
			//DataP1.dir = (CaptureTheFlag::Direction)(rand() % 2);
			//DataP1.val = (2 * (rand() % 2)) - 1; // either -1 or 1



		// Network
		//coin flip
		DataP2.player = CaptureTheFlag::Player::Player2;
		DataP2.dir = CaptureTheFlag::Direction::None;
		DataP2.val = game.coinFlip(); //flip
		networkMan.RcvDataWithACK(TargetAddr, DataP1); // receive P1 data
		networkMan.SendDataWithACK(TargetAddr, DataP2); // send P2 data

		if (abs(DataP1.val - DataP2.val) == 1) //different
		{
			//Player 2
			DataP2.player = CaptureTheFlag::Player::Player2;
			DataP2.dir = (CaptureTheFlag::Direction)(rand() % 2);
			DataP2.val = (2 * (rand() % 2)) - 1; // either -1 or 1
			networkMan.SendDataWithACK(TargetAddr, DataP2); // send P2 data

			game.Action(DataP2); //Move P2

			printf("Player 2 Move\n");
		}

		else //same
		{
			networkMan.RcvDataWithACK(TargetAddr, DataP1); // receive P1 data

			game.Action(DataP1); //Move P1

		}

		// Update part 2: we can now update the game state
			game.UpdateField();       // Adjust markers on field 
			winner = game.TestForWinner();

		// Draw phase
		system("cls");
		game.Display();

		Sleep(100);			
		//system("pause");	// For development/lecture demo/Manual operation
	}
	printf("\tPlayer %c wins!!!\n\n", winner);

networkMan.CleanUp();

	//system("pause");

	return 0;
}
