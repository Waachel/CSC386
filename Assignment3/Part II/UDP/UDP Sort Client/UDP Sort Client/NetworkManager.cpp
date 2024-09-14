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

void NetworkManager::SendListWithACK(sockaddr_in &target, std::list<LotsOfData>& rNodeList, int count)
{
	PacketHeader pkt;

	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send

	//SendTo
	//printf("Sending a datagram to the server...\n");

	//add count to buffer
	pkt.type = LSTMSG_TYPE::LIST_SIZE;
	pkt.seqnum = -1;
	pkt.sizeOfPayload = count;
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
	if (pkt.sizeOfPayload == 1)
	{
		printf("Got ACK...\n");
		for (std::list<LotsOfData>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
		{
			//add one element of test data
			pkt.type = LSTMSG_TYPE::LIST_ITEM;
			pkt.seqnum = indexOfList;
			pkt.sizeOfPayload = 10;
			pkt.data.setX(it->getX());
			pkt.data.setY(it->getY());
			//int len = strlen(it->s);
			//pkt.data.s = new char[len + 1];
			//memcpy(pkt.data.s, it->s, sizeof(pkt.data.s));
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
			if (pkt.sizeOfPayload == 0)
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

void NetworkManager::RcvListWithACK(sockaddr_in &target, std::list<LotsOfData>& rNodeList, int count)
{
	PacketHeader pkt;
	const int BufLen = 1024;
	char MsgBuf[BufLen] = { 0 };	// What we send
	//receive reversed list
	LotsOfData* newData = new LotsOfData[count + 1];



	for (int i = 0; i < count; i++)
	{
		RcvMsg(target, MsgBuf, BufLen);

		ExtractPacket(MsgBuf, pkt);

		//memcpy(newData[i].s, pkt.data.s, sizeof(pkt.data.s));

		memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer

		//check if packet received is a duplicate
		if (pkt.seqnum == i && pkt.type == LSTMSG_TYPE::LIST_ITEM) //not a duplicate
		{
			newData[i].setX(pkt.data.getX());
			newData[i].setY(pkt.data.getY());

			//send ACK
			printf("Sending ACK to the sender...\n");
			//SendTo
			pkt.type = LSTMSG_TYPE::LIST_ACK;
			pkt.seqnum = 0;
			pkt.sizeOfPayload = 1;//ACK
			BuildPacket(MsgBuf, pkt);

			SendMsg(target, MsgBuf, BufLen);

			memset(MsgBuf, 0, sizeof(MsgBuf));  // clear buffer
		}

		else //is a duplicate
		{
			//ignore
			printf("DUPLICATE IGNORE\n");
			i--;
		}


	}

	//save new list
	int i = 0;
	for (std::list<LotsOfData>::iterator it = rNodeList.begin(); it != rNodeList.end(); ++it)
	{
		it->setX(newData[i].getX());
		it->setY(newData[i].getY());
		//memcpy(it->s, newData[i].s, sizeof(pkt.data.s));
		i++;
	}

	delete[] newData;
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
		data.clear();
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
