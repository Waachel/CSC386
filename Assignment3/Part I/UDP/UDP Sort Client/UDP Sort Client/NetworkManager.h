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

	struct ListPacket
	{
		LSTMSG_TYPE type;
		int num;
		int data;

		void serialize(char* const buffer);
		void deserialize(char* const buffer);
	};

	NetworkManager();
	~NetworkManager();
	NetworkManager(const NetworkManager&) = delete;
	NetworkManager& operator=(const NetworkManager&) = delete;

	void SendMsg(sockaddr_in& target, const char* buffer, int len, int flags = 0);
	void SendListWithACK(sockaddr_in& target, std::list<int>& rNodeList, int count);
	void RcvMsg(sockaddr_in& sender, char* buffer, int len, int flags = 0);
	void RcvListWithACK(sockaddr_in& target, std::list<int>& rNodeList, int count);
	void SetDuplicationProb(int n);

	void BuildPacket(char* buffer, ListPacket& pkt);
	void ExtractPacket(char* buffer, ListPacket& pkt);

	void CleanUp();

private:
	SOCKET mySocket;
	int DuplicationProb;
};


#endif _NetworkManager