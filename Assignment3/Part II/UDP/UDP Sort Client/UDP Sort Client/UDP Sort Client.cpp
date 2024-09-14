// UDP Sort Client.cpp 

#include "TestData.h"
#include "LotsOfData.h"
#include "NetworkManager.h"
#include <stdio.h>
#include <iostream>

void AdjustWindow()
{
	HWND hwndFound = GetConsoleWindow();
	RECT rc;
	GetWindowRect(GetDesktopWindow(), &rc);  // screen dimension in right and bottom
	MoveWindow(hwndFound, 0, 0, rc.right / 2, rc.bottom / 2, true);
}

int main()
{
	AdjustWindow(); // Positioning console window for convenience

	NetworkManager networkMan;
	networkMan.SetDuplicationProb(10);

	sockaddr_in TargetAddr;
	TargetAddr.sin_family = AF_INET;
	TargetAddr.sin_addr.s_addr = inet_addr("127.0.0.1");  // This computer see localhost https://en.wikipedia.org/wiki/Localhost
	TargetAddr.sin_port = htons(8888);

	//--------------------------------------------------------
	// Get initialize data
	//--------------------------------------------------------

	std::list<LotsOfData> rNodeList;

	LotsOfData  dataA(1, 2);
	rNodeList.push_back(dataA);
	LotsOfData  dataB(3, 4);
	rNodeList.push_back(dataB);
	LotsOfData  dataC(5, 6);
	rNodeList.push_back(dataC);
	LotsOfData  dataD(7, 8);
	rNodeList.push_back(dataD);
	LotsOfData  dataE(9, 10);
	rNodeList.push_back(dataE);

	//--------------------------------------------------------
	// Print the original data
	//--------------------------------------------------------
	Trace::out("\n\nClient: Original List\n\n");

	int count = 0;
	for (std::list<LotsOfData>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		Trace::out("    i:%02d %d, %d\n", count++, it->getX(), it->getY());// , it->getS());

	}

	//--------------------------------------------------------
	// Add your code and hooks here:
	//--------------------------------------------------------
	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send

	while (MsgBuf[0] != 'N')
	{
		// User input (QOTD protocol accept any string)
		printf("Send List (Y/N): ");
		std::cin >> MsgBuf;

		if (MsgBuf[0] == 'Y') // lazy user control
		{
			
			//send list
			networkMan.SendListWithACK(TargetAddr, rNodeList, count);
			printf("Ready for Reversed List\n\n");
			memset(MsgBuf, 0, sizeof(MsgBuf));  // clear reply buffer
		
			//receive list
			networkMan.RcvListWithACK(TargetAddr, rNodeList, count);
		}

		else if (MsgBuf[0] == 'N')
		{
			//SendTo
			printf("Sending a datagram to the server...\n");
			networkMan.SendMsg(TargetAddr, MsgBuf, BufLen);
		}

		else
		{
			//do nothing
		}
	}

	//Disconnect
	networkMan.CleanUp();

	//--------------------------------------------------------
	// Print the reversed data
	//--------------------------------------------------------
	Trace::out("\n\nClient: Reversed List from the server\n\n");

	int k = 0;
	for (std::list<LotsOfData>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		Trace::out("    i:%02d %d, %d\n", k++, it->getX(), it->getY());// , it->getS());
	}

	Trace::out("\n");
	Trace::out("Client Done\n");
	Trace::out("\n");

	// Uncomment for development
	system("PAUSE");
	
	return 0;
}