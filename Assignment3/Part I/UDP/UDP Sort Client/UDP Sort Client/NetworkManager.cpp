#include "NetworkManager.h"

NetworkManager::NetworkManager()
{
	DuplicationProb = 0;
	WSADATA wsaData;
	int iResult;

	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed: %d\n", iResult);
		assert(false);
	}

	mySocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (this->mySocket == INVALID_SOCKET) {
		printf("socket failed with error: %ld\n", WSAGetLastError());
		assert(false);
	}

	printf("Socket ready\n");
}

void NetworkManager::SendMsg(sockaddr_in &target, const char* buffer, int len, int flags)
{
	int iResult = sendto(this->mySocket, buffer, len, flags, (SOCKADDR*)&target, sizeof(target));
	if (iResult == SOCKET_ERROR) {
		printf("sendto failed with error: %d\n", WSAGetLastError());
		WSACleanup();
		assert(false);
	}

	//simulate duplicated messages
	if (rand() % 100 < DuplicationProb)
	{
		iResult = sendto(this->mySocket, buffer, len, flags, (SOCKADDR*)&target, sizeof(target));
		if (iResult == SOCKET_ERROR) {
			printf("sendto failed with error: %d\n", WSAGetLastError());
			WSACleanup();
			assert(false);
		}
	}

}

void NetworkManager::SendListWithACK(sockaddr_in &target, std::list<int>& rNodeList, int count)
{
	ListPacket pkt;

	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send

	//SendTo
	//printf("Sending a datagram to the server...\n");

	//add count to buffer
	pkt.type = LSTMSG_TYPE::LIST_SIZE;
	pkt.num = -1;
	pkt.data = count;
	BuildPacket(MsgBuf, pkt);

	SendMsg(target, MsgBuf, BufLen);

	memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

	//ReceiveFrom - Check for ACK
	printf("Waiting for ACK...\n");
	RcvMsg(target, MsgBuf, BufLen);

	ExtractPacket(MsgBuf, pkt);
	memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

	int indexOfList = 0;
	//received ACK for sending count
	if (pkt.data == 1)
	{
		printf("Got ACK...\n");
		for (std::list<int>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
		{
			//add one element of test data
			pkt.type = LSTMSG_TYPE::LIST_ITEM;
			pkt.num = indexOfList;
			pkt.data = *it;
			BuildPacket(MsgBuf, pkt);

			SendMsg(target, MsgBuf, BufLen);

			indexOfList++;

			memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

			//make sure we get ACK before going on
			printf("Waiting for ACK...\n");
			RcvMsg(target, MsgBuf, BufLen);

			ExtractPacket(MsgBuf, pkt);

			memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

			//ACK not received
			if (pkt.data == 0)
			{
				break;
			}

		}
	}

}

void NetworkManager::RcvMsg(sockaddr_in& sender, char* buffer, int len, int flags)
{
	
	int SenderAddrSize = sizeof(sender);
	int iResult = recvfrom(this->mySocket, buffer, len, flags, (SOCKADDR*)&sender, &SenderAddrSize);
	//printf("Started to receive list...\n");
	if (iResult == SOCKET_ERROR) {
		printf("recvfrom failed with error %d\n", WSAGetLastError());
		WSACleanup();
		assert(false);
	}
}

void NetworkManager::RcvListWithACK(sockaddr_in &target, std::list<int>& rNodeList, int count)
{
	ListPacket pkt;
	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send
	//receive sorted list
	int* newData = new int[count + 1];



	for (int i = 0; i < count; i++)
	{
		RcvMsg(target, MsgBuf, BufLen);

		ExtractPacket(MsgBuf, pkt);

		newData[i] = pkt.data;

		memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

		//check if packet received is a duplicate
		if (pkt.num == i) //not a duplicate
		{
			//send ACK
			printf("Sending ACK to the sender...\n");
			//SendTo
			pkt.type = LSTMSG_TYPE::LIST_ACK;
			pkt.num = 0;
			pkt.data = 1;//ACK
			BuildPacket(MsgBuf, pkt);

			SendMsg(target, MsgBuf, BufLen);

			memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer
		}

		else //is a duplicate
		{
			//ignore
			printf("DUPLICATE IGNORE...\n");
			i--;
		}


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


void NetworkManager::SetDuplicationProb(int n)
{
	DuplicationProb = n;
}

void NetworkManager::BuildPacket(char* buffer, ListPacket& pkt)
{
	pkt.serialize(buffer);
}

void NetworkManager::ExtractPacket(char* buffer, ListPacket& pkt)
{
	pkt.deserialize(buffer);
}

void NetworkManager::ListPacket::serialize(char* const buffer)
{
	char* p = buffer;
	//send type
	memcpy(p, &type, sizeof(LSTMSG_TYPE));

	p += sizeof(LSTMSG_TYPE);

	//send index
	memcpy(p, &num, sizeof(int));

	p += sizeof(int);

	//send value
	memcpy(p, &data, sizeof(int));
}

void NetworkManager::ListPacket::deserialize(char* const buffer)
{
	char* p = buffer;
	//send type
	memcpy(&type, p, sizeof(LSTMSG_TYPE));

	p += sizeof(LSTMSG_TYPE);

	//send index
	memcpy(&num, p, sizeof(int));

	p += sizeof(int);

	//send value
	memcpy(&data, p, sizeof(int));
}


NetworkManager::~NetworkManager()
{

}

void NetworkManager::CleanUp()
{
	printf("Informing server we're done.\n");
	int iResult = shutdown(mySocket, SD_BOTH);
	if (iResult == SOCKET_ERROR)
	{
		printf("shutdown failed: %d\n", WSAGetLastError());
		closesocket(mySocket);
		WSACleanup();
		assert(false);
	}

	closesocket(this->mySocket);
	printf("Socket closed\n");

	WSACleanup();
}