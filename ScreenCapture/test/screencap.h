#pragma once

#ifndef SCREENCAP_
#define SCREENCAP_

#include <winsock2.h>
#include <windows.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>

class Connection 
{
private:
	SOCKET listenSocket;
public:
	SOCKET clientSocket;

	Connection();
	void WaitForConnection();
};

#endif