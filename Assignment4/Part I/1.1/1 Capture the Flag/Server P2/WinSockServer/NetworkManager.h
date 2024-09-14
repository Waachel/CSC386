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
#include <list>
#include "CaptureTheFlag.h"

#pragma comment(lib, "Ws2_32.lib")


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
		CaptureTheFlag::MoveData data;

		void serialize(char* const buffer);

		void deserialize(char* const buffer);
	};

	NetworkManager();
	~NetworkManager();
	NetworkManager(const NetworkManager&) = delete;
	NetworkManager& operator=(const NetworkManager&) = delete;

	void Bind(const SOCKADDR_IN& Target);

	void SendMsg(sockaddr_in& target, const char* buffer, int len, int flags = 0);
	void SendDataWithACK(sockaddr_in& target, CaptureTheFlag::MoveData& gameData);
	void RcvMsg(sockaddr_in& sender, char* buffer, int len, int flags = 0);
	void RcvDataWithACK(sockaddr_in& target, CaptureTheFlag::MoveData& gameData);
	void SetDuplicationProb(int n);

	void BuildPacket(char* buffer, PacketHeader& pkt);
	void ExtractPacket(char* buffer, PacketHeader& pkt);

	void CleanUp();

private:
	SOCKET mySocket;
	int DuplicationProb;
	int seqClientIndex;
	int seqServerIndex;
};


#endif _NetworkManager