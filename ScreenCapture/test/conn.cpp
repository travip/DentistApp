#undef UNICODE

#include "conn.hpp"

#include <winsock2.h>
#include <windows.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>
#pragma comment (lib, "Ws2_32.lib")

#define DEFAULT_BUFLEN 512
#define DEFAULT_PORT "54321"

SOCKET listenSocket = INVALID_SOCKET;
SOCKET clientSocket = INVALID_SOCKET;

void BeginListening()
{
	WSADATA wsaData;
	int iResult;

	struct addrinfo *result = NULL;
	struct addrinfo hints;

	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) 
	{
		printf("WSAStartup failed with error: %d\n", iResult);
		exit(1);
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;

	// Resolve the server address and port
	iResult = getaddrinfo(NULL, DEFAULT_PORT, &hints, &result);
	if (iResult != 0) 
	{
		printf("getaddrinfo failed with error: %d\n", iResult);
		WSACleanup();
		exit(1);
	}

	// Create a SOCKET for connecting to server
	listenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
	if (listenSocket == INVALID_SOCKET) 
	{
		printf("socket failed with error: %ld\n", WSAGetLastError());
		freeaddrinfo(result);
		WSACleanup();
		exit(1);
	}

	// Setup the TCP listening socket
	iResult = bind(listenSocket, result->ai_addr, (int)result->ai_addrlen);
	if (iResult == SOCKET_ERROR) 
	{
		printf("bind failed with error: %d\n", WSAGetLastError());
		freeaddrinfo(result);
		closesocket(listenSocket);
		WSACleanup();
		exit(1);
	}

	freeaddrinfo(result);

	iResult = listen(listenSocket, SOMAXCONN);
	if (iResult == SOCKET_ERROR) 
	{
		printf("listen failed with error: %d\n", WSAGetLastError());
		closesocket(listenSocket);
		WSACleanup();
		exit(1);
	}
}
void WaitForConnection()
{
	// Accept a client socket
	clientSocket = accept(listenSocket, NULL, NULL);
	printf("Accepted a connection\n");
	if (clientSocket == INVALID_SOCKET) 
	{
		printf("accept failed with error: %d\n", WSAGetLastError());
		closesocket(listenSocket);
		WSACleanup();
		exit(1);
	}
	printf("Client connection accepted\n");
}

int SendMessage(char* msg, long size) 
{
	int iResult;

	char* len = new char[4];

	// size of picture as byte array
	// Big endian
	len[3] = size & 0x000000ff;
	len[2] = (size & 0x0000ff00) >> 8;
	len[1] = (size & 0x00ff0000) >> 16;
	len[0] = (size & 0xff000000) >> 24;

	iResult = send(clientSocket, len, 4, 0);
	printf("Send header:  %i bytes\n", iResult);
	iResult = send(clientSocket, msg, size, 0);
	printf("Sent body:    %i bytes\n", iResult);
	if (iResult < size) 
	{
		printf("Failed to send entire message");
		return 1;
	}
	return 0;
}

int EndConnection() 
{
	closesocket(listenSocket);
	closesocket(clientSocket);
	WSACleanup();
	return 0;
}
