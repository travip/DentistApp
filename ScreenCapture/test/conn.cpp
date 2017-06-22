#undef UNICODE

#include "conn.hpp"
#include <string>

#pragma comment (lib, "Ws2_32.lib")

#define DEFAULT_PORT 52524
#define RECV_BUFLEN 512

// Callback function to check if connecting client is the same from the UDP Broadcast
int CALLBACK Server::CheckValidConnection(LPWSABUF lpCallerId,
	LPWSABUF lpCallerData,
	LPQOS lpSQOS,
	LPQOS lpGQOS,
	LPWSABUF lpCalleeId,
	LPWSABUF lpCalleeData,
	GROUP FAR *g,
	DWORD_PTR dwCallbackData)
{
	Server* callingServer = (Server *) dwCallbackData;
	sockaddr_in* connectingAddr = (sockaddr_in *)lpCallerId->buf;
	if ( strcmp(inet_ntoa(connectingAddr->sin_addr), inet_ntoa(callingServer->clientAddr.sin_addr)) == 0) {
		return CF_ACCEPT;
	}
	return CF_REJECT;
}

Server::Server()
{
	int iResult;

	addrinfo *result = NULL;
	addrinfo hints;

	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0)
	{
		printf("WSAStartup failed with error: %d\n", iResult);
		exit(EXIT_FAILURE);
	}

	udpSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (udpSocket == INVALID_SOCKET)
	{
		printf("socket failure with error: %d \n", WSAGetLastError());
		exit(EXIT_FAILURE);
	}

	ZeroMemory(&servAddr, sizeof(servAddr));
	ZeroMemory(&clientAddr, sizeof(clientAddr));
	servAddr.sin_family = AF_INET;
	servAddr.sin_port = htons((USHORT)DEFAULT_PORT);
	servAddr.sin_addr.s_addr = htonl(INADDR_ANY);

	iResult = bind(udpSocket, (struct sockaddr *)&servAddr, sizeof(servAddr));
	if (iResult != 0) 
	{
		printf("Server: udp bind failed with error: %d\n", iResult);
		WSACleanup();
		exit(EXIT_FAILURE);
	}
}

// Destructor - Close the server socket and cleanup
Server::~Server() {
	if (connected) {
		TCPSend(NULL, 0, PTYPE_DISCONNECT);
	}
	closesocket(udpSocket);
	closesocket(tcpListenSocket);
	closesocket(tcpClientSocket);
	WSACleanup();
}

// Block and wait for a connection
int Server::WaitForConnection()
{
	printf("Waiting for connection...\n");
	char recvBuf[RECV_BUFLEN];
	int recvLen;

	// Try to receive some data, this is a blocking call
	recvLen = recvfrom(udpSocket, recvBuf, RECV_BUFLEN, 0, (SOCKADDR *) &clientAddr, &sLen);
	if (recvLen == SOCKET_ERROR)
	{
		printf("recvfrom failed with error: %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}
	// Received message - reply with my IP Address
	else 
	{
		printf("Received %i bytes from connection from: %s:%d\n", recvLen, inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));
		if (recvLen == 1 && recvBuf[0] == 0x01) {
			printf("Valid connection request. Replying...\n");
			UDPSend("HI", 2);
			recvLen = EstablishTCPConnection();
		}
	}

	if (recvLen == 0) {
		printf("Connection established succesfully\n");
		connected = true;
		return 0;
	}
	else {
		printf("Failed to establish connection\n");
		return 1;
	}

	return -1;
}

// Create a TCP listening Socket (stored to tcpListenSocket) and being listening
int Server::CreateTCPSocket() 
{
	int iResult;
	addrinfo *result = NULL;
	addrinfo hints;

	// Set hints for TCP socket binding
	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;

	// Get server information
	iResult = getaddrinfo(NULL, TCP_PORT, &hints, &result);
	if (iResult != 0) {
		printf("CreateTCPSocket: getaddrinfo failed with error: %d\n", iResult);
		return 1;
	}

	// Create a socket for listening
	tcpListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
	if (tcpListenSocket == INVALID_SOCKET) {
		printf("CreateTCPSocket: socket failed with error: %ld\n", WSAGetLastError());
		freeaddrinfo(result);
		return 1;
	}

	iResult = bind(tcpListenSocket, result->ai_addr, (int)result->ai_addrlen);
	if (iResult == SOCKET_ERROR) {
		printf("CreateTCPSocket: bind failed with error: %d\n", WSAGetLastError());
		freeaddrinfo(result);
		closesocket(tcpListenSocket);
		return 1;
	}

	freeaddrinfo(result);

	iResult = listen(tcpListenSocket, SOMAXCONN);
	if (iResult == SOCKET_ERROR) {
		printf("EstablishTCPConnection: listen failed with error: %d\n", WSAGetLastError());
		closesocket(tcpListenSocket);
		return 1;
	}

	DWORD nTrue = 1;
	setsockopt(tcpListenSocket, SOL_SOCKET, SO_CONDITIONAL_ACCEPT, (char*)&nTrue, sizeof(nTrue));

	return 0;
}

// Accept a pending connection request on TCP
int Server::EstablishTCPConnection() 
{
	printf("Waiting for client to connect..\n");
	tcpClientSocket = WSAAccept(tcpListenSocket, (sockaddr*)&clientAddr, &sLen, &CheckValidConnection, (DWORD_PTR)this);
	if (tcpClientSocket == INVALID_SOCKET) {
		printf("EstablishTCPConnection: accept failed with error: %d\n", WSAGetLastError());
		return 1;
	}
	printf("Connected!\n");

	// No longer need server socket
	closesocket(tcpListenSocket);
	return 0;
}

// Send a packet - MSG is the payload, a header indicating size of payload is appended to the packet
int Server::UDPSend(const char* msg, long size)
{
	int iResult;
	iResult = sendto(udpSocket, msg, size, 0, (struct sockaddr*) &clientAddr, sLen);
	if (iResult < size)
	{
		printf("UDPSend: sendto failed with error: %d", WSAGetLastError());
		return 1;
	}
	return 0;
}

// Send a packet - MSG is the payload, a header indicating size of payload is appended to the packet
int Server::TCPSend(const char* msg, long size, char packetType)
{
	if (connected) {
		int iResult;

		char head[5];

		// Type of packet
		head[0] = packetType;

		// size of picture as byte array
		head[4] = size & 0x000000ff;
		head[3] = (size & 0x0000ff00) >> 8;
		head[2] = (size & 0x00ff0000) >> 16;
		head[1] = (size & 0xff000000) >> 24;

		iResult = sendto(tcpClientSocket, head, 5, 0, (struct sockaddr*) &clientAddr, sLen);
		if (iResult < 5)
		{
			printf("TCPSend: sendto failed with error: %d", WSAGetLastError());
			return 1;
		}
		if (size > 0) {
			iResult = sendto(tcpClientSocket, msg, size, 0, (struct sockaddr*) &clientAddr, sLen);
			if (iResult < size)
			{
				printf("TCPSend: sendto failed with error: %d", WSAGetLastError());
				return 1;
			}
		}
	}
	else {
		printf("Tried to send message with no client connected\n");
		return 1;
	}

	return 0;
}

// End an active connection - Only one connection can be active at any time
int Server::EndConnection()
{
	if (connected) {
		TCPSend(NULL, 0, PTYPE_DISCONNECT);
		ZeroMemory(&clientAddr, sizeof(clientAddr));
		connected = false;
		return 0;
	}
	else {
		printf("Tried to end connection whilst no connection open\n");
		return 1;
	}
}

