// ServerForAndroidClient.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <winsock.h>
#include <string.h>
#include <vector>


#include "FundusSegment.h"

//只连接一次，改为可重复连接，多个客户端同时运行，

#pragma comment(lib,"ws2_32.lib")

#define PORT 5037
#define BACKLOG 10
#define TRUE 1


int sendImageToClient(int socket, char *ImageForSend, int size);
char* receiveImageFromClient(int socket, int *GetSize);
void* FundusSegmentServer(char* receiveImage, size_t length, int *sendImageSize);


void main()
{
	int iServerSock;
	int iClientSock;

	struct sockaddr_in ServerAddr;
	struct sockaddr_in ClientAddr;


	int sin_size;

	WSADATA WSAData;

	if (WSAStartup(MAKEWORD(1, 1), &WSAData))
	{

		printf("initializationing error!\n");
		WSACleanup();
		exit(0);
	}
	printf("Initialization\n");

	if ((iServerSock = socket(AF_INET, SOCK_STREAM, 0)) == INVALID_SOCKET)
	{
		printf("Fail to create socket！\n");
		WSACleanup();
		exit(0);
	}
	printf("Create Socket!\n");

	ServerAddr.sin_family = AF_INET;
	//ServerAddr.sin_addr.S_un.S_addr = htonl(INADDR_ANY);
	ServerAddr.sin_addr.s_addr = inet_addr("172.27.35.1");
	ServerAddr.sin_port = htons((u_short)PORT);
	//ServerAddr.sin_port = 0;

	memset(&(ServerAddr.sin_zero), 0, sizeof(ServerAddr.sin_zero));

	if (bind(iServerSock, (struct sockaddr *)&ServerAddr, sizeof(struct sockaddr)) == -1)
	{
		printf("bind fail！\n");
		WSACleanup();
		exit(0);
	}
	printf("Bind success!\n");

	if (listen(iServerSock, BACKLOG) == -1)
	{
		printf("listen fail！\n");
		WSACleanup();
		exit(0);
	}
	printf("Listen Success!\n");

	for (;;)
	{
		sin_size = sizeof(struct sockaddr_in);
		iClientSock = accept(iServerSock, (struct sockaddr *)&ClientAddr, &sin_size);

		if (iClientSock == -1)
		{
			printf("accept调用失败！\n");
			exit(0);
		}
		printf("Accept Success\n");

		printf("Server is connected to%s\n", inet_ntoa(ClientAddr.sin_addr));


		//接受来自客户端的数据存入recvques
		//char recvques[100];//用户选择的文件号
		//memset(recvques, 0, sizeof(recvques));
		//recv(iClientSock, recvques, 100, 0);
		//printf("Receive %c from Client\n", recvques[0]);
		//

		int getSize;

		char* recvImage = receiveImageFromClient(iClientSock, &getSize);

		size_t length = getSize;

		if (recvImage == NULL)
		{
			printf("从client接收失败！\n");
			WSACleanup();
			exit(0);
		}
		else
		{
			printf("Successfully receive from Client！\n");
		}


		//第一次传输结束
		printf("\n");
		printf("opencv running。。。。\n");
		printf("\n");
		printf("\n");

		int stat;
		char buffer[50] = "Opencv is processing the Image...";
		const char* opencvSignal = buffer;
		//Send our verification signal
		do{
			stat = send(iClientSock, opencvSignal, strlen(opencvSignal) + 1, 0);
		} while (stat<0);


		int imageSendSize;
		printf("start image processing\n");
		printf("\n");
		printf("\n");

		void *sendImageVoid = FundusSegmentServer(recvImage, getSize, &imageSendSize);
		char *sendImage = (char *)sendImageVoid;


		char buffer2[100] = "Image processing is finished, start Transmit image to Client!";
		const char* opencvfinishSignal = buffer2;
		//Send our verification signal
		do{
			stat = send(iClientSock, opencvfinishSignal, strlen(opencvfinishSignal) + 1, 0);
		} while (stat<0);

		//第二次传输开始
		printf("Start send the segmentation Image to Client!\n");


		int sendStatus = sendImageToClient(iClientSock, sendImage, imageSendSize);
		if (sendStatus != 1)
		{
			printf("Send to Client失败！\n");
			WSACleanup();
			exit(0);
		}
		if (sendStatus == 1)
		{
			printf("Send to Client success！\n");
		}

		freeImage(sendImage);


		closesocket(iClientSock);

	}
}

char* receiveImageFromClient(int socket, int *GetSize)
{
	int buffersize = 0, recv_size = 0, packet_index = 1, stat;
	char sizeBuffer[15];
	memset(sizeBuffer, 0, sizeof(sizeBuffer));

	char imagearray[10240];


	printf("Start Receive Size data from Client.\n");

	//Find the size of the image
	do{
		stat = recv(socket, sizeBuffer, 15, 0);
	} while (stat<0);

	int size = atoi(&sizeBuffer[0]);

	printf("Packet received.\n");
	printf("Packet size: %d\n", stat);
	printf("Image size: %d\n", size);
	printf(" \n");

	char buffer[50] = "Got the size data, now start transmit imgae!";
	const char* ConfirmDataSend = buffer;
	//Send our verification signal
	do{
		stat = send(socket, ConfirmDataSend, strlen(ConfirmDataSend) + 1, 0);
	} while (stat<0);

	printf("Reply sent\n");
	printf(" \n");
	printf(" \n");
	printf(" \n");



	char* recvImage;
	recvImage = (char *)malloc(size);

	int totalWritten = 0;

	do{
		recv_size = recv(socket, imagearray, sizeof(imagearray), 0);

		if (recv_size < 0)
		{
			printf("Cannot read from socket!\n");

			exit(0);
		}

		if (recv_size == 0)
			break;

		printf("receive data packet #%i from Client.\n", packet_index);
		printf("Receive %i data from the Client.\n", recv_size);

		int off = 0;
		/*do{
		int written = fwrite(&imagearray[off], 1, recv_size - off, image);
		if (written < 1)
		{
		printf("Cannot write to the image\n");
		fclose(image);
		exit(0);
		}

		printf("Written %i data into image\n", written);
		off += written;
		totalWritten += written;
		} while (off < recv_size);*/


		do
		{
			memcpy(&recvImage[totalWritten], &imagearray[off], recv_size - off);

			totalWritten += recv_size - off;
			off += recv_size - off;

		} while (off < recv_size);



		printf("This transmission write %i data into image from data packet %i.\n", totalWritten, packet_index);
		packet_index++;

		printf("\n");
		printf("\n");
		printf("\n");

	} while (totalWritten<size);


	printf("This transmission write %i data into image from Client\n", totalWritten);

	*GetSize = size;

	return recvImage;
}

int sendImageToClient(int socket, char *ImageForSend, int size){


	int read_size, sent, stat, packet_index;
	char send_buffer[10240], read_buffer[256];
	packet_index = 1;





	printf("Getting Picture Size\n");

	printf("Total send Picture size: %i\n", size);

	char sizeSendBuffer[10];
	_itoa_s(size, sizeSendBuffer, 10);
	const char* SizeSend = sizeSendBuffer;
	//Send Picture Size
	printf("Sending Picture Size\n");
	send(socket, SizeSend, strlen(SizeSend) + 1, 0);

	//Send Picture as Byte Array
	printf("Sending Picture as Byte Array\n");




	do { //Read while we get errors that are due to signals.
		stat = recv(socket, read_buffer, sizeof(read_buffer), 0);//read the comfirm signal from the client
		if (stat < 1)
		{
			printf("Cannot receive the confirm signal from the client.\n");
			exit(0);
		}
	} while (stat < 0);

	printf("Received comfirm data in socket from client\n");
	printf("\n");
	printf("Confirm Socket data: %s\n", read_buffer);
	printf("\n");
	printf("\n");
	printf("\n");

	int totalSent = 0;

	while (totalSent<size) {


		if (size<10240)
		{
			memcpy(&send_buffer[0], &ImageForSend[totalSent], size);

			int off = 0;

			//send data through our socket 
			do{
				sent = send(socket, &send_buffer[off], size - off, 0);

				if (sent < 1)
				{
					printf("Cannot write to socket!\n");

					exit(0);
				}

				printf("Sent %i data to client\n", sent);
				off += sent;

				totalSent += sent;

			} while (off<size);
		}
		else
		{
			if ((size - totalSent) > 10240)
			{
				memcpy(&send_buffer[0], &ImageForSend[totalSent], sizeof(send_buffer));

				int off = 0;

				//send data through our socket 
				do{
					sent = send(socket, &send_buffer[off], sizeof(send_buffer)-off, 0);

					if (sent < 1)
					{
						printf("Cannot write to socket!\n");

						exit(0);
					}

					printf("Sent %i data to client\n", sent);
					off += sent;

					totalSent += sent;

				} while (off<sizeof(send_buffer));
			}


			else if ((size - totalSent) < 10240)
			{
				memcpy(&send_buffer[0], &ImageForSend[totalSent], size - totalSent);

				int off = 0;

				//send data through our socket 
				do{
					sent = send(socket, &send_buffer[off], size - totalSent - off, 0);

					if (sent < 1)
					{
						printf("Cannot write to socket!\n");

						exit(0);
					}

					printf("Sent %i data to client\n", sent);
					off += sent;

					totalSent += sent;

				} while (off<(size - totalSent));
			}
		}

		printf("packet number: %i\n", packet_index);

		printf(" \n");
		printf(" \n");


		packet_index++;

		//zero out our send buffer
		memset(send_buffer, 0, sizeof(send_buffer));
	}

	printf("Total data sent to the client is %i\n", totalSent);


	return 1;
}

void* FundusSegmentServer(char* receiveImage, size_t length, int *sendImageSize)
{
	//std::string directory("d:/");

	std::string directory("Data//");

	printf("Start startup func.....\n");

	startup(directory.c_str());

	printf("success startup\n");


	printf("Start segmentImage func.....\n");
	void *outputImage; size_t bytes;
	auto error = segmentImage(receiveImage, length, &outputImage, &bytes);
	if (error) return NULL;
	printf("success segmentImage\n");

	*sendImageSize = bytes;

	delete[] receiveImage;

	//shutdown1();

	return outputImage;
}



