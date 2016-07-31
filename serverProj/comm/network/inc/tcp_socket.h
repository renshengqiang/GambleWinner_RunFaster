#ifndef NETWORK_TCP_SOCKET_H_
#define NETWORK_TCP_SOCKET_H_

#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>

#include <string>

#include "common.h"

namespace network 
{
	
class TcpSocket {
public:
	TcpSocket() {
		sock_fd_ = -1;
	}

	TcpSocket(int fd) {
		sock_fd_ = fd;
	}
	~ TcpSocket();

	int Listen(int port);

    TcpSocket* Accept();

    int Connect(const struct sockaddr_in *remote_addr, bool non_blocking_connect = false); 
    int Connect(const ServerLocation &location, bool non_blocking_connect = false);

    int Send(const char *buf, int buf_len);

    /// Receives at-most the specified # of bytes.
    /// @retval Returns the result of calling recv().
    int Recv(char *buf, int buf_len);

    /// Close the TCP socket.
    void Close();

    /// Get and clear pending socket error: getsockopt(SO_ERROR)
    int GetSocketError() const;

    int GetPeerName(struct sockaddr *peerAddr, int len) const;

    std::string GetPeerName() const;
    std::string GetSockName() const;

    inline int GetFd() { 
    	return sock_fd_; 
    }

    /// Return true if socket is good for read/write. false otherwise.
    bool IsGood() const;
private:
	int sock_fd_;
};

}


#endif //NETWORK_TCP_SOCKET_H_