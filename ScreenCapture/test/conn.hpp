#pragma once

void BeginListening();
void WaitForConnection();
int SendMessage(char* msg, long size);
int EndConnection();