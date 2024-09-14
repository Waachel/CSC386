#include "NetworkManager.h"

NetworkManager::NetworkManager()
{
	DuplicationProb = 0;
	seqServerIndex = 0;
	seqClientIndex = 0;
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

void NetworkManager::SendDataWithACK(sockaddr_in &target, CaptureTheFlag::MoveData& gameData)
{
	PacketHeader pkt;

	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send

	//SendTo
	//printf("Sending a datagram to the server...\n");

	//add to buffer
	pkt.type = LSTMSG_TYPE::LIST_ITEM;
	pkt.seqnum = seqClientIndex;
	seqClientIndex++;
	pkt.sizeOfPayload = sizeof(gameData);
	pkt.data.player = gameData.player;
	pkt.data.dir = gameData.dir;
	pkt.data.val = gameData.val;
	BuildPacket(MsgBuf, pkt);

	SendMsg(target, MsgBuf, BufLen);

	memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

	//ReceiveFrom - Check for ACK
	printf("Waiting for ACK...\n");
	RcvMsg(target, MsgBuf, BufLen);

	ExtractPacket(MsgBuf, pkt);
	memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

	//Check if packet matches
	while (pkt.type != LSTMSG_TYPE::LIST_ACK)
	{
		printf("Waiting for ACK...\n");
		RcvMsg(target, MsgBuf, BufLen);

		ExtractPacket(MsgBuf, pkt);
		memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer
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

void NetworkManager::RcvDataWithACK(sockaddr_in &target, CaptureTheFlag::MoveData& gameData)
{
	PacketHeader pkt;
	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send

	//receive data
	
	RcvMsg(target, MsgBuf, BufLen);

	ExtractPacket(MsgBuf, pkt);

	memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer
	if (pkt.seqnum == seqServerIndex && pkt.type == LSTMSG_TYPE::LIST_ITEM)
	{
		printf("Sequence Number: %d\n", pkt.seqnum);
		seqServerIndex++;
		gameData.player = pkt.data.player;
		gameData.dir = pkt.data.dir;
		gameData.val = pkt.data.val;

		//send ACK
		pkt.type = LSTMSG_TYPE::LIST_ACK;
		//pkt.seqnum = 0;
		//pkt.sizeOfPayload = 1;//ACK
		BuildPacket(MsgBuf, pkt);

		SendMsg(target, MsgBuf, BufLen);

		memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer
	}

	else //duplicate
	{
		printf("Duplicate Packet!!\n");
		RcvDataWithACK(target, gameData);
	}

}


void NetworkManager::SetDuplicationProb(int n)
{
	DuplicationProb = n;
}

void NetworkManager::BuildPacket(char* buffer, PacketHeader& pkt)
{
	pkt.serialize(buffer);
}

void NetworkManager::ExtractPacket(char* buffer, PacketHeader& pkt)
{
	pkt.deserialize(buffer);
}

NetworkManager::~NetworkManager()
{

}

void NetworkManager::CleanUp()
{
	int iResult = shutdown(mySocket, SD_BOTH);
	if (iResult == SOCKET_ERROR)
	{
		printf("shutdown failed: %d\n", WSAGetLastError());
		closesocket(mySocket);
		WSACleanup();
		assert(false);
	}

	closesocket(this->mySocket);

	WSACleanup();
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
		data.serialize(p);
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
		data.deserialize(p);
	}
}
