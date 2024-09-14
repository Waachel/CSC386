// NetworkManager
// AB 4/21

#ifndef _NetworkManager
#define _NetworkManager

#define _WINSOCK_DEPRECATED_NO_WARNINGS

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <iphlpapi.h>

#pragma comment(lib, "Ws2_32.lib")

#include <string>
#include <list>
#include "LotsOfData.h"

class NetworkManager
{
public:
	enum class LSTMSG_TYPE
	{
		LIST_SIZE,
		LIST_ITEM,
		LIST_ACK
	};

	struct PacketHeader
	{
		LSTMSG_TYPE type;
		int seqnum;
		int sizeOfPayload;
		LotsOfData data;

		void serialize(char* const buffer);

		void deserialize(char* const buffer);
	};

	NetworkManager();
	~NetworkManager();
	NetworkManager(const NetworkManager&) = delete;
	NetworkManager& operator=(const NetworkManager&) = delete;

	void Bind(const SOCKADDR_IN& Target);

	void SendMsg(sockaddr_in& target, const char* buffer, int len, int flags = 0);
	void reversed(int numOfItems, LotsOfData* testData);
	void SendListWithACK(sockaddr_in& target, LotsOfData* testData, int size);
	void RcvMsg(sockaddr_in& sender, char* buffer, int len, int flags = 0);
	int RcvListSize(sockaddr_in& sender);
	void RcvListWithACK(sockaddr_in& sender, LotsOfData* testData, int size);
	void SetDuplicationProb(int n);

	void CleanUp();

	void BuildPacket(char* buffer, PacketHeader& pkt);

	void ExtractPacket(char* buffer, PacketHeader& pkt);

private:
	SOCKET mySocket;
	int DuplicationProb;
};


#endif _NetworkManager