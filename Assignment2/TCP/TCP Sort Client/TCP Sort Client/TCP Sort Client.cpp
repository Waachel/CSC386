// TCP Sort Client.cpp 

#include "TestData.h"

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
#include <string>

#pragma comment(lib, "Ws2_32.lib")


void AdjustWindow()
{
	HWND hwndFound = GetConsoleWindow();
	RECT rc;
	GetWindowRect(GetDesktopWindow(), &rc);  // screen dimension in right and bottom
	MoveWindow(hwndFound, 0, 0, rc.right / 2, rc.bottom / 2, true);
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

int setup(SOCKET &MySocket, std::string &request)
{
	WSADATA wsaData;
	int iResult;

	//Init Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed: %d\n", iResult);
		return 1;
	}

	//Create Socket
	MySocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (MySocket == INVALID_SOCKET) {
		printf("socket failed with error: %ld\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}

	//Connect to Server
	SOCKADDR_IN TargetAddr;
	TargetAddr.sin_family = AF_INET;

	//For testing just the client side
	//TargetAddr.sin_addr.s_addr = inet_addr("132.163.97.3");
	//TargetAddr.sin_port = htons(13);

	TargetAddr.sin_addr.s_addr = inet_addr("127.0.0.1");
	TargetAddr.sin_port = htons(8888); // loopback server demo
	request = "fsdlfsdfj"; // The server is dumb and will always send be the same HTML page anyway...


	if (connect(MySocket, (SOCKADDR*)&TargetAddr, sizeof(TargetAddr)) == SOCKET_ERROR)
	{
		printf("connect function failed with error: %ld\n", WSAGetLastError());
		iResult = closesocket(MySocket);
		if (iResult == SOCKET_ERROR)
			printf("closesocket function failed with error: %ld\n", WSAGetLastError());
		WSACleanup();
		return 1;
	}
	std::cout << "Connected to MySocket!" << std::endl;
	printf("(Currently bound to port #%d)\n\n", GetPortFromSocket(MySocket));

	return iResult;
}

int sendData(SOCKET& MySocket, char* const buff, const int bufsize, char* &buffer)
{
	//strncpy_s(buffer, bufsize, request.c_str(), request.length() + 1);
	memcpy(buffer, buff, bufsize);

	if (send(MySocket, buffer, bufsize, 0) == SOCKET_ERROR) {
		printf("send failed with error: %d\n", WSAGetLastError());
		closesocket(MySocket);
		WSACleanup();
		return 1;
	}
	std::cout << "Message sent!" << std::endl;

	return 0;
}

// Copy from a buffer
void deserialize(const char* const buffer, int count)
{
	const char* p = buffer;
	for (int i = 0; i < count; i++)
	{
		memcpy(&initData[i], p, sizeof(int));
		p += sizeof(int);
	}

	Trace::out("    i:%02d  %d\n", initData[1]);

};

void receiveData(SOCKET& MySocket, const int bufsize, char*& buffer)
{
	recv(MySocket, buffer, bufsize, 0);
}

void disconnect(SOCKET MySocket,char* buffer)
{
	delete[] buffer;

	std::cout << "Client disconnected." << std::endl;

	shutdown(MySocket, SD_BOTH); // closesocket(MySocket);
	WSACleanup();
	std::cout << "Socket closed." << std::endl << std::endl;
}

//not actually needed because we print out the list
int numOfItems()
{
	TestData  data;
	std::list<int>& rNodeList = data.getNodeList();

	int count = 0;
	for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		count++;
	}

	return count;
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

	SOCKET MySocket;
	std::string request;
	//Initialize Winsock, create socket, and connect
	int iResult = setup(MySocket, request);


	//Send data
	const int bufsize = 1024;
	char* buffer = new char[bufsize];

	//add count to buffer
	char buff[1024];
	char* p = buff;

	memcpy(p, &count, sizeof(int));
	p += sizeof(int);

	//send count
	iResult = sendData(MySocket, buff, bufsize, buffer);

	rNodeList = data.getNodeList();
	for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		memcpy(buff, &*it, sizeof(int));
		iResult = sendData(MySocket, buff, bufsize, buffer);
	}

	//buffer = buff;
	//memcpy(buffer, &buff, sizeof(int) * count + 1);
	
	int* newData = new int[count + 1];
	//Receive data
	for (int i = 0; i < count; i++)
	{
		receiveData(MySocket, bufsize, buffer);

		memcpy(&newData[i], buffer, sizeof(int));

		printf("Server says: [%d]\n", newData[i]);
	}

	int i = 0;
	for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		memcpy(&*it, &newData[i], sizeof(int));
		i++;
	}

	delete[] newData;

	
	//Disconnect
	disconnect(MySocket, buffer);

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
