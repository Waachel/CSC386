// UDP Sort Client.cpp 

#include "TestData.h"

#define _WINSOCK_DEPRECATED_NO_WARNINGS	// I know, we shouldn't use deprecating functions, 
										// but in this case, it's a bit messy...
										// See "Version 2" below.

#ifndef WIN32_LEAN_AND_MEAN			// another wonderful MS hack to keep their headers under control...
#define WIN32_LEAN_AND_MEAN
#endif
#include <windows.h>

#include <winsock2.h>	// both needed for using Winsock
#include <ws2tcpip.h>	//
#include <iphlpapi.h>	// IP Helper API

#pragma comment(lib, "Ws2_32.lib")	// https://docs.microsoft.com/en-us/windows/win32/winsock/creating-a-basic-winsock-application
									// because of course MS...

#include <stdio.h>
#include <iostream>

void AdjustWindow()
{
	HWND hwndFound = GetConsoleWindow();
	RECT rc;
	GetWindowRect(GetDesktopWindow(), &rc);  // screen dimension in right and bottom
	MoveWindow(hwndFound, 0, 0, rc.right / 2, rc.bottom / 2, true);
}
int setup(SOCKET& mySocket, sockaddr_in& TargetAddr)
{
	int iResult;

	//Init Winsock
	WSADATA wsaData;
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed: %d\n", iResult);
		return EXIT_FAILURE;
	}
	printf("Wisock2 Initialization\n");

	//Create Socket using UDP settings
	mySocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (mySocket == INVALID_SOCKET) {
		printf("Socket creation error: %d\n", WSAGetLastError());
		return EXIT_FAILURE;
	}
	printf("UDP socket created\n\n");

	//Bind - optional
	//* Version 1: Old school and deprecated
	//TargetAddr.sin_family = AF_INET;
	//TargetAddr.sin_addr.s_addr = inet_addr("24.214.177.39");  // Technically we should also test for invalid address errors
	//TargetAddr.sin_port = htons(17);	// See all the other 'host-to-network' (hton) converters
	// https://docs.microsoft.com/en-us/windows/win32/api/winsock/nf-winsock-htons
	//*/

	//Version 4: Loopback for server demo
	TargetAddr.sin_family = AF_INET;
	TargetAddr.sin_addr.s_addr = inet_addr("127.0.0.1");  // This computer see localhost https://en.wikipedia.org/wiki/Localhost
	TargetAddr.sin_port = htons(8888);
	//

	return iResult;
}

int receiveFrom(SOCKET mySocket, char* ReplyBuf, const int BufLen, sockaddr_in SenderAddr, int SenderAddrSize)
{
	int iResult;
	printf("Waiting for ACK...\n");
	iResult = recvfrom(mySocket, ReplyBuf, BufLen, 0, (SOCKADDR*)&SenderAddr, &SenderAddrSize);
	if (iResult == SOCKET_ERROR) {
		printf("recvfrom failed with error %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}

	return 0;
}

int disconnect(SOCKET mySocket)
{
	printf("Informing server we're done.\n");
	int iResult = shutdown(mySocket, SD_BOTH);
	if (iResult == SOCKET_ERROR)
	{
		printf("shutdown failed: %d\n", WSAGetLastError());
		closesocket(mySocket);
		WSACleanup();
		return 1;
	}

	printf("Closing socket.\n");
	iResult = closesocket(mySocket);
	if (iResult == SOCKET_ERROR) {
		printf("closesocket failed with error: %d\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}

	WSACleanup();
	return 0;
}

int main()
{
	AdjustWindow(); // Positioning console window for convenience

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
	SOCKET mySocket;
	sockaddr_in TargetAddr;

	//Initialize and create
	int iResult = setup(mySocket, TargetAddr);

	

	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send
	char ReplyBuf[BufLen];			// Replies we get

	sockaddr_in SenderAddr;			// used for recvFrom parameters...
	int SenderAddrSize = sizeof(SenderAddr);

	while (MsgBuf[0] != 'N')
	{
		// User input (QOTD protocol accept any string)
		printf("Send List (Y/N): ");
		std::cin >> MsgBuf;

		if (MsgBuf[0] == 'Y') // lazy user control
		{
			//SendTo
			printf("Sending a datagram to the server...\n");

			//add count to buffer
			memcpy(MsgBuf, &count, sizeof(int));

			iResult = sendto(mySocket, MsgBuf, BufLen, 0, (SOCKADDR*)&TargetAddr, sizeof(TargetAddr));
			if (iResult == SOCKET_ERROR) {
				printf("sendto failed with error: %d\n", WSAGetLastError());
				WSACleanup();
				return 1;
			}

			// Let's find out which port number we got assigned..
			sockaddr_in myaddr;
			socklen_t myaddrlen = sizeof(myaddr);
			iResult = getsockname(mySocket, (struct sockaddr*)&myaddr, &myaddrlen);
			if (iResult != 0) {
				printf("getsockname failed: %d\n", WSAGetLastError());
				return EXIT_FAILURE;
			}
			printf("(Socket now locally bound to port #%i\n\n", ntohs(myaddr.sin_port));

			//ReceiveFrom - Check for ACK
			printf("Waiting for ACK...\n");
			iResult = recvfrom(mySocket, ReplyBuf, BufLen, 0, (SOCKADDR*)&SenderAddr, &SenderAddrSize);
			if (iResult == SOCKET_ERROR) {
				printf("recvfrom failed with error %d\n", WSAGetLastError());
				WSACleanup();
				return 1;
			}


			int checkForACK;
			memcpy(&checkForACK, ReplyBuf, sizeof(int));

			//received ACK for sending count
			if (checkForACK == 1)
			{
				for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
				{
					//add one element of test data
					memcpy(MsgBuf, &*it, sizeof(int));

					iResult = sendto(mySocket, MsgBuf, BufLen, 0, (SOCKADDR*)&TargetAddr, sizeof(TargetAddr));
					if (iResult == SOCKET_ERROR) {
						printf("sendto failed with error: %d\n", WSAGetLastError());
						WSACleanup();
						return 1;
					}

					//make sure we get ACK before going on
					printf("Waiting for ACK...\n");
					iResult = recvfrom(mySocket, ReplyBuf, BufLen, 0, (SOCKADDR*)&SenderAddr, &SenderAddrSize);
					if (iResult == SOCKET_ERROR) {
						printf("recvfrom failed with error %d\n", WSAGetLastError());
						WSACleanup();
						return 1;
					}

					checkForACK = 0;
					memcpy(&checkForACK, ReplyBuf, sizeof(int));

					//ACK not received
					if (checkForACK == 0)
					{
						break;
					}

				}
			}

			printf("Ready for Sorted List\n\n");
			memset(ReplyBuf, 0, sizeof(ReplyBuf));  // clear reply buffer
		
			//receive sorted list
			int* newData = new int[count + 1];

			for (int i = 0; i < count; i++)
			{
				iResult = recvfrom(mySocket, MsgBuf, BufLen, 0, (SOCKADDR*)&SenderAddr, &SenderAddrSize);
				if (iResult == SOCKET_ERROR) {
					printf("recvfrom failed with error %d\n", WSAGetLastError());
					WSACleanup();
					return 1;
				}

				memcpy(&newData[i], MsgBuf, sizeof(int));

				//send ACK
				printf("Sending ACK to the sender...\n");
				//SendTo
				int ACK = 1;
				memcpy(ReplyBuf, &ACK, sizeof(int));
				iResult = sendto(mySocket, ReplyBuf, BufLen, 0, (SOCKADDR*)&SenderAddr, SenderAddrSize);
				if (iResult == SOCKET_ERROR) {
					printf("sendto failed with error: %d\n", WSAGetLastError());
					WSACleanup();
					return 1;
				}
			}

			Trace::out("    NEW DATA\n");
			for (int i = 0; i < count; i++)
			{
				Trace::out("    i:%02d %d\n", i, newData[i]);
			}

			//save new list
			int i = 0;
			for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
			{
				memcpy(&*it, &newData[i], sizeof(int));
				i++;
			}

			delete[] newData;
		}

		else if (MsgBuf[0] == 'N')
		{
			//SendTo
			printf("Sending a datagram to the server...\n");
			iResult = sendto(mySocket, MsgBuf, BufLen, 0, (SOCKADDR*)&TargetAddr, sizeof(TargetAddr));
			if (iResult == SOCKET_ERROR) {
				printf("sendto failed with error: %d\n", WSAGetLastError());
				WSACleanup();
				return 1;
			}
		}

		else
		{
			//do nothing
		}
	}

	//Disconnect
	disconnect(mySocket);



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