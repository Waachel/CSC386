#include "NetworkManager.h"

#include <fstream>
#include <stdio.h>
#include <iostream>

NetworkManager::NetworkManager()
{
	DuplicationProb = 0;
	WSADATA wsaData;
	int iResult;

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

void NetworkManager::Bind(const SOCKADDR_IN& Target)
{
	bind(this->mySocket, (SOCKADDR*)&Target, sizeof(Target));
}

void NetworkManager::SendMsg(sockaddr_in& target, const char* buffer, int len, int flags)
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

void NetworkManager::reversed(int numOfItems, LotsOfData* testData)
	{
		//reverse list
		LotsOfData* reversed = new LotsOfData[numOfItems];
		for (int i = 0; i < numOfItems; i++) 
		{
			reversed[i].setX(testData[numOfItems - i - 1].getX());
			reversed[i].setY(testData[numOfItems - i - 1].getY());
			//reversed[i].setS(testData[numOfItems - i - 1].getS());
		}

		//copy list to original array
		for (int i = 0; i < numOfItems; i++) 
		{
			testData[i].setX(reversed[i].getX());
			testData[i].setY(reversed[i].getY());
			//testData[i].setS(reversed[i].getS());
		}

		//delete temporary array
		delete[] reversed;
	}

void NetworkManager::SendListWithACK(sockaddr_in& target, LotsOfData* testData, int size)
{
	PacketHeader pkt;
	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send

	//reverse list
	reversed(size, testData);

	//Send reversed list one item at a time
	for (int i = 0; i < size; i++)
	{
		pkt.type = LSTMSG_TYPE::LIST_ITEM;
		pkt.seqnum = i;
		pkt.sizeOfPayload = 10;
		pkt.data.setX(testData[i].getX());
		pkt.data.setY(testData[i].getY());
		//pkt.data.setS(testData[i].getS());
		BuildPacket(MsgBuf, pkt);

		SendMsg(target, MsgBuf, BufLen);

		//make sure we get ACK before going on
		printf("Waiting for ACK of new list...\n");

		RcvMsg(target, MsgBuf, BufLen);

		ExtractPacket(MsgBuf, pkt);

		//ACK not received
		if (pkt.sizeOfPayload == 0)
		{
			return;
		}
		//print list
		Trace::out("    i:%02d %d, %d\n", i, testData[i].getX(), testData[i].getY());
					//testData[i].getS());
	}
}

void NetworkManager::RcvMsg(sockaddr_in& sender, char* buffer, int len, int flags)
{
	int SenderAddrSize = sizeof(sender);
	int iResult = recvfrom(this->mySocket, buffer, len, flags, (SOCKADDR*)&sender, &SenderAddrSize);
	if (iResult == SOCKET_ERROR) {
		printf("recvfrom failed with error %d\n", WSAGetLastError());
		WSACleanup();
		assert(false);
	}
}


int NetworkManager::RcvListSize(sockaddr_in& target)
{
	PacketHeader pkt;
	int size = 0;
	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send


	printf("Waiting for a request...\n");
	//ReceiveFrom
	// Note: The difference between 'recv' and 'recvfrom' is whether we get the address of the sender
	// As a server is expected to send UDP replies, we need this address, so recvfrom it is.
	RcvMsg(target, MsgBuf, BufLen);

	ExtractPacket(MsgBuf, pkt);

	if (MsgBuf[0] == '//N')
	{
		return size;//size = 0
	}

	printf("Request received from %s:%i\n", inet_ntoa(target.sin_addr), ntohs(target.sin_port));

	//check if packet received is the count - index will equal -1
	if (pkt.seqnum == -1)
	{
		//set size
		size = pkt.sizeOfPayload;

		//Send ACK to count
		memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

		//SendTo
		pkt.type = LSTMSG_TYPE::LIST_ACK;
		pkt.seqnum = 0;
		pkt.sizeOfPayload = 1;
		BuildPacket(MsgBuf, pkt);

		SendMsg(target, MsgBuf, BufLen);
	}

	else
	{
		//didn't get count first
		printf("Did not receive size of list\n");
		return size;//size = 0
	}

	printf("Got size of list\n");
	return size;
}
void NetworkManager::RcvListWithACK(sockaddr_in& target, LotsOfData* testData, int size)
{
	PacketHeader pkt;
	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send

	for (int i = 0; i < size; i++)
	{
		//get list
		RcvMsg(target, MsgBuf, BufLen);

		ExtractPacket(MsgBuf, pkt);

		//check if packet received is a duplicate
		if (pkt.seqnum == i && pkt.type == LSTMSG_TYPE::LIST_ITEM) //not a duplicate
		{
			//save list
			testData[i].setX(pkt.data.getX());
			testData[i].setY(pkt.data.getY());
			//testData[i].setS(pkt.data.getS());

			memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

			// Slowing things down to demo/study
			//Sleep(3000);

			printf("Sending ACK to the sender...\n");
			//SendTo
			pkt.type = LSTMSG_TYPE::LIST_ACK;
			pkt.seqnum = 0;
			pkt.sizeOfPayload = 1;//ACK
			BuildPacket(MsgBuf, pkt);
			SendMsg(target, MsgBuf, BufLen);
		}

		else //is a duplicate
		{
			//ignore
			printf("DUPLICATE IGNORE\n");
			i--;
		}

	}

}

void NetworkManager::SetDuplicationProb(int n)
{
	DuplicationProb = n;
}

NetworkManager::~NetworkManager()
{

}

void NetworkManager::CleanUp()
{
	shutdown(this->mySocket, SD_BOTH);
	WSACleanup();
}

void NetworkManager::BuildPacket(char* buffer, PacketHeader& pkt)
{
	pkt.serialize(buffer);
}

void NetworkManager::ExtractPacket(char* buffer, PacketHeader& pkt)
{
	pkt.deserialize(buffer);
}

void NetworkManager::PacketHeader::serialize(char* const buffer)
{
	char* p = buffer;
	//send type
	memcpy(p, &type, sizeof(LSTMSG_TYPE));

	p += sizeof(LSTMSG_TYPE);

	//send index
	memcpy(p, &seqnum, sizeof(int));

	p += sizeof(int);

	//send size
	memcpy(p, &sizeOfPayload, sizeof(int));

	p += sizeof(int);

	if (type == LSTMSG_TYPE::LIST_ITEM)
	{
		//send data
		int len = sizeof(data);
		data.serialize(p, len);
	}
}

void NetworkManager::PacketHeader::deserialize(char* const buffer)
{
	char* p = buffer;
	//send type
	memcpy(&type, p, sizeof(LSTMSG_TYPE));

	p += sizeof(LSTMSG_TYPE);

	//send index
	memcpy(&seqnum, p, sizeof(int));

	p += sizeof(int);

	//send value
	memcpy(&sizeOfPayload, p, sizeof(int));

	p += sizeof(int);

	if (type == LSTMSG_TYPE::LIST_ITEM)
	{
		//send data
		int len = sizeof(data);
		data.deserialize(p, len);
	}
}
