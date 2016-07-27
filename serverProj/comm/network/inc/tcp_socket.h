#ifndef TCP_SOCKET_H_
#define TCP_SOCKET_H_

namespace network 
{
	class TcpSocket
	{
	public:
		 TcpSocket();
		~ TcpSocket();
		
		virtual int HandleEvent(int code, void *data) = 0;

	};

}


#endif //TCP_SOCKET_H_