// UDP Sort Client.cpp 

#include "TestData.h"
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
	networkMan.SetDuplicationProb(50);

	sockaddr_in TargetAddr;
	TargetAddr.sin_family = AF_INET;
	TargetAddr.sin_addr.s_addr = inet_addr("127.0.0.1");  // This computer see localhost https://en.wikipedia.org/wiki/Localhost
	TargetAddr.sin_port = htons(8888);

	//--------------------------------------------------------
	// Get initialize data
	//--------------------------------------------------------
	TestData  data;
	std::list<int>& rNodeList = data.getNodeList();

	//--------------------------------------------------------
	// Print the original data
	//--------------------------------------------------------
	Trace::out("\n\nClient: Original List\n\n");

	int count = 0;
	for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		Trace::out("    i:%02d %d\n", count++, *it);
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

			printf("Ready for Sorted List\n\n");
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
	// Print the sorted data
	//--------------------------------------------------------
	Trace::out("\n\nClient: Sorted List from the server\n\n");

	int k = 0;
	for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		Trace::out("    i:%02d  %d\n", k++, *it);
	}

	Trace::out("\n");
	Trace::out("Client Done\n");
	Trace::out("\n");

	// Uncomment for development
	system("PAUSE");

	return 0;
}