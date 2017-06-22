#pragma once

#include <winsock2.h>
#include <windows.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>
#pragma comment (lib, "Ws2_32.lib")

#define UDP_PORT "52524"
#define TCP_PORT "56567"

class Server
{
public:
	Server();
	~Server();

	sockaddr_in servAddr, clientAddr;

	int UDPSend(const char *msg, long size);
	int TCPSend(const char* msg, long size, char packetType);
	int WaitForConnection();
	int EstablishTCPConnection();
	int EndConnection();
	static int CALLBACK CheckValidConnection(LPWSABUF lpCallerId,
		LPWSABUF lpCallerData,
		LPQOS lpSQOS,
		LPQOS lpGQOS,
		LPWSABUF lpCalleeId,
		LPWSABUF lpCalleeData,
		GROUP FAR *g,
		DWORD_PTR dwCallbackData);

private:
	SOCKET udpSocket;
	SOCKET tcpListenSocket, tcpClientSocket;
	int sLen = sizeof(clientAddr);
	WSADATA wsaData;
	bool connected;
};

typedef char PACKETTYPE;

#define DISCOVERY_PACKET 0x01
#define DISCONNECT_PACKET 0x02
#define IMAGE_PACKET 0x05