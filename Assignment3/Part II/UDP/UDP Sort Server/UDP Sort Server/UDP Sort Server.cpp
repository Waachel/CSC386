// UDP Sort Server.cpp 
#include "NetworkManager.h"
#include "LotsOfData.h"

void AdjustWindow()
{
	HWND hwndFound = GetConsoleWindow();
	RECT rc;
	GetWindowRect(GetDesktopWindow(), &rc);  // screen dimension in right and bottom
	MoveWindow(hwndFound, rc.right / 2, 0, rc.right / 2, rc.bottom / 2, true);
}

int main()
{
	AdjustWindow(); // Positioning console window for convenience

	NetworkManager networkMan;
	networkMan.SetDuplicationProb(80);

	SOCKADDR_IN TargetAddr;
	TargetAddr.sin_family = AF_INET;

	SOCKADDR_IN serverAddr;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_addr.s_addr = INADDR_ANY;
	serverAddr.sin_port = htons(8888);

	networkMan.Bind(serverAddr);
	printf("Listening for incoming connections...\n");

	sockaddr_in SenderAddr = TargetAddr;			// used for recvFrom parameters...

	LotsOfData* testData;
	int size = 0;

	while (true) // Maybe try shutting down based on a received message?
	{
		//get size of list
		size = networkMan.RcvListSize(SenderAddr);
		testData = new LotsOfData[size + 1];

		//get list
		networkMan.RcvListWithACK(SenderAddr, testData, size);
		
		//reverse and send list
		networkMan.SendListWithACK(SenderAddr, testData, size);

		break;

	}


	//delete new testData
	delete[] testData;

	//Disconnect
	networkMan.CleanUp();


	//----------------------------------------------------
	// print and exit
	//----------------------------------------------------

	Trace::out("\n");
	Trace::out("Server: Done\n");
	Trace::out("\n");

	// Uncomment for development
	system("PAUSE");
	return 0;
}