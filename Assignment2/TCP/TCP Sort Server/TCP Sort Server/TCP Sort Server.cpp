// TCP Sort Server.cpp 

#define _WINSOCK_DEPRECATED_NO_WARNINGS
#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <iphlpapi.h>
#include <stdio.h>

#include <iostream>
#include <fstream>

#pragma comment(lib, "Ws2_32.lib")

void AdjustWindow()
{
	HWND hwndFound = GetConsoleWindow();
	RECT rc;
	GetWindowRect(GetDesktopWindow(), &rc);  // screen dimension in right and bottom
	MoveWindow(hwndFound, rc.right / 2, 0, rc.right / 2, rc.bottom / 2, true);
}

u_short GetPortFromSocket(SOCKET& sk)
{
	int iResult;
	sockaddr_in myaddr;
	socklen_t myaddrlen = sizeof(myaddr);
	iResult = getsockname(sk, (struct sockaddr*)&myaddr, &myaddrlen);
	if (iResult != 0) {
		printf("getsockname failed: %d\n", WSAGetLastError());
		assert(false);
	}
	return ntohs(myaddr.sin_port);
}

int setup(SOCKET &server)
{
	WSADATA WSAData;

	int iResult;

	//Init Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &WSAData);
	if (iResult != 0) {
		printf("WSAStartup failed: %d\n", WSAGetLastError());
		return 1;
	}

	//Create Socket
	server = socket(AF_INET, SOCK_STREAM, 0);

	//Bind Socket
	SOCKADDR_IN serverAddr;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_addr.s_addr = INADDR_ANY;
	serverAddr.sin_port = htons(8888);

	bind(server, (SOCKADDR*)&serverAddr, sizeof(serverAddr));
	printf("(Currently bound to port #%d)\n\n", GetPortFromSocket(server));

	//Return Socket
	return iResult;
}

int receiveData(SOCKET& client, const int BufSize, char*& buffer)
{
	recv(client, buffer, BufSize, 0);
	

	//get size of list
	int size = 0;
	memcpy(&size, buffer, sizeof(int));

	printf("Client says: [%d]\n", size);

	//int* testData = new int[size + 1];

	//return BufSize
	return size;
}


int sendData(SOCKET& client, const int BufSize, char*& buffer)
{
	printf("Sending list...\n");
	if (send(client, buffer, BufSize, 0) == SOCKET_ERROR) {
		printf("send failed with error: %d\n", WSAGetLastError());
		closesocket(client);
		WSACleanup();
		return 1;
	}
	int size = 0;
	memcpy(&size, buffer, sizeof(int));
	printf("Client says: [%d]\n", size);
	return 0;
}


// Copy from a buffer
int deserialize(char* buffer)
{
	int value = 0;
	memcpy(&value, buffer, sizeof(int));
	return value;

};

// Write object to a buffer
void serialize(char* const buffer, int data)
{
	memcpy(buffer, &data, sizeof(int));
}

void sort(int numOfItems, int* testData)
{
	for (int i = 0; i < numOfItems; i++)
	{
		for (int j = i + 1; j < numOfItems; j++) {
			if (testData[i] > testData[j])
			{
				int temp = testData[i];
				testData[i] = testData[j];
				testData[j] = temp;
			}
		}
	}
}

int listenAndConnect(SOCKET server, const int BufSize, char*& buffer)
{
	//Listen on the socket for client
	listen(server, 0);
	std::cout << "Listening for incoming connections..." << std::endl;

	//Accept a connect from client
	SOCKET client;
	SOCKADDR_IN clientAddr;

	memset(buffer, 0, BufSize);
	int iResult = 0;

	int clientAddrSize = sizeof(clientAddr);
	if ((client = accept(server, (SOCKADDR*)&clientAddr, &clientAddrSize)) != INVALID_SOCKET)
	{
		printf("Client connected: %s:%i\n", inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));

		//Receive Data
		int bufferSize = receiveData(client, BufSize, buffer);
		
		//get size of list
		int size = 0;
		memcpy(&size, buffer, sizeof(int));

		int* testData = new int[size + 1];

		for (int i = 0; i < size; i++)
		{
			int value = receiveData(client, BufSize, buffer);

			//create new list
			testData[i] = value;
		}


		//sort list
		sort(size, testData);

		//print list
		for (int i = 0; i < size; i++)
		{
			Trace::out("    i: %d\n", testData[i]);
		}

		for (int i = 0; i < size; i++)
		{
			memcpy(buffer, &testData[i], sizeof(int));
			iResult = sendData(client, bufferSize, buffer);
		}
		//Send Data
		
		delete[] testData;
		shutdown(client, SD_SEND); //closesocket(client);
		std::cout << "Client disconnected." << std::endl;

	}

	return 0;
}

void disconnect(SOCKET server, char* buffer)
{
	delete[] buffer;

	closesocket(server);

	WSACleanup();
	std::cout << "Socket closed." << std::endl << std::endl;
}

int main()
{
	AdjustWindow(); // Positioning console window for convenience

	//----------------------------------------------------
	// Add your magic here
	//----------------------------------------------------

	//Initialize, create, bind
	SOCKET server;

	int iResult	= setup(server);

	const int BufSize = 1024;
	char* buffer = new char[BufSize];
	
	iResult = listenAndConnect(server, BufSize, buffer);

	//Disconnect
	disconnect(server, buffer);

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
