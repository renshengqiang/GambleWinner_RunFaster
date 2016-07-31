#include "tcp_socket.h"
#include <iostream>

TcpSocket::~TcpSocket()
{
    Close();
}

int TcpSocket::Listen(int port)
{
    struct sockaddr_in	our_addr;
    int reuse_addr = 1;

    sock_fd_ = socket(PF_INET, SOCK_STREAM, 0);
    if (sock_fd_ == -1) {
        perror("Socket: ");
        return -1;
    }

    memset(&our_addr, 0, sizeof(struct sockaddr_in));
    our_addr.sin_family = AF_INET;
    our_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    our_addr.sin_port = htons(port);

    /* 
     * A piece of magic here: Before we bind the fd to a port, setup
     * the fd to reuse the address. If we move this line to after the
     * bind call, then things don't work out.  That is, we bind the fd
     * to a port; we panic; on restart, bind will fail with the address
     * in use (i.e., need to wait 2MSL for TCP's time-wait).  By tagging
     * the fd to reuse an , everything is happy.
     */
    if (setsockopt(sock_fd_, SOL_SOCKET, SO_REUSEADDR, 
                   (char *) &reuse_addr, sizeof(reuse_addr)) < 0) {
        perror("Setsockopt: ");
    }

    if (bind(sock_fd_, (struct sockaddr *) &our_addr, sizeof(our_addr)) < 0) {
        perror("Bind: ");
        close(sock_fd_);
        sock_fd_ = -1;
        return -1;
    }
    
    if (listen(sock_fd_, 1024) < 0) {
        perror("listen: ");
    }


    return 0;
    
}

TcpSocket* TcpSocket::Accept()
{
    int fd;
    struct sockaddr_in	cli_addr;    
    TcpSocket *acc_socket;
    socklen_t cli_addr_len = sizeof(cli_addr);

    if ((fd = accept(sock_fd_, (struct sockaddr *) &cli_addr, &cli_addr_len)) < 0) {
        perror("Accept: ");
        return NULL;
    }
    acc_socket = new TcpSocket(fd);

    acc_socket->SetupSocket();


    return acc_socket;
}

int TcpSocket::Connect(const ServerLocation &location, bool non_blocking_connect)
{
    struct sockaddr_in remote_addr = { 0 };

    if (! inet_aton(location.hostname.c_str(), &remote_addr.sin_addr)) {
        // do the conversion if we weren't handed an IP address
        struct hostent * const hostInfo = gethostbyname(location.hostname.c_str());
        if (hostInfo == NULL || hostInfo->h_addrtype != AF_INET ||
                hostInfo->h_length < (int)sizeof(remote_addr.sin_addr)) {
            std::cout <<
                "connect: "  << location.ToString() <<
                " hostent: " << (const void*)hostInfo <<
                " type: "    << (hostInfo ? hostInfo->h_addrtype : -1) <<
                " size: "    << (hostInfo ? hostInfo->h_length   : -1) <<
            std::endl;
            return -1;
        }
        memcpy(&remote_addr.sin_addr, hostInfo->h_addr, sizeof(remote_addr.sin_addr));
    }
    remote_addr.sin_port = htons(location.port);
    remote_addr.sin_family = AF_INET;
    return Connect(&remote_addr, non_blocking_connect);
}
int TcpSocket::Connect(const struct sockaddr_in *remote_addr, bool non_blocking_connect)
{
    int res = 0;

    Close();

    sock_fd_ = socket(PF_INET, SOCK_STREAM, 0);
    if (sock_fd_ < 0) {
        return (errno > 0 ? -errno : sock_fd_);
    }

    if (non_blocking_connect) {
        // when we do a non-blocking connect, we mark the socket
        // non-blocking; then call connect and it wil return
        // EINPROGRESS; the fd is added to the select loop to check
        // for completion
        fcntl(sock_fd_, F_SETFL, O_NONBLOCK);
    }

    res = connect(sock_fd_, (struct sockaddr *) remote_addr, sizeof(struct sockaddr_in));
	if(res != 0)
	{	
		std::cout << "TcpSocket connection error result:" << res <<std::endl;
	}
    if ((res < 0) && (errno != EINPROGRESS)) {
        res = errno > 0 ? -errno : res;
        perror("Connect: ");
        close(sock_fd_);
        sock_fd_ = -1;
        return res;
    }

    if ((res < 0) && non_blocking_connect)
        res = -errno;
    SetupSocket();

    return res;
}
void TcpSocket::SetupSocket()
{
    int bufSize = 65536;
    int flag = 1;

    // get big send/recv buffers and setup the socket for non-blocking I/O
    if (setsockopt(sock_fd_, SOL_SOCKET, SO_SNDBUF, (char *) &bufSize, sizeof(bufSize)) < 0) {
        perror("Setsockopt: ");
    }
    if (setsockopt(sock_fd_, SOL_SOCKET, SO_RCVBUF, (char *) &bufSize, sizeof(bufSize)) < 0) {
        perror("Setsockopt: ");
    }
    // enable keep alive so we can socket errors due to detect network partitions
    if (setsockopt(sock_fd_, SOL_SOCKET, SO_KEEPALIVE, (char *) &flag, sizeof(flag)) < 0) {
        perror("Disabling NAGLE: ");
    }

    fcntl(sock_fd_, F_SETFL, O_NONBLOCK);
    // turn off NAGLE
    if (setsockopt(sock_fd_, IPPROTO_TCP, TCP_NODELAY, (char *) &flag, sizeof(flag)) < 0) {
        perror("Disabling NAGLE: ");
    }

}
int TcpSocket::GetPeerName(struct sockaddr *peerAddr, int len) const
{
    socklen_t peerLen = len;

    if (getpeername(sock_fd_, peerAddr, &peerLen) < 0) {
        perror("getpeername: ");
        return -1;
    }
    return 0;
}

std::string TcpSocket::GetPeerName() const
{
    struct sockaddr_in saddr;
    char ipname[INET_ADDRSTRLEN + 7];

    if (GetPeerName((struct sockaddr*) &saddr, sizeof(struct sockaddr_in)) < 0)
        return "unknown";
    if (inet_ntop(AF_INET, &(saddr.sin_addr), ipname, INET_ADDRSTRLEN) == NULL)
        return "unknown";
    ipname[INET_ADDRSTRLEN] = 0;
    sprintf(ipname + strlen(ipname), ":%d", (int)htons(saddr.sin_port));
    return ipname;
}

std::string TcpSocket::GetSockName() const
{
    struct sockaddr_in saddr;
    char ipname[INET_ADDRSTRLEN + 7];

    socklen_t len(sizeof(struct sockaddr_in));
    if (getsockname(sock_fd_, (struct sockaddr*) &saddr, &len) < 0)
        return "unknown";
    if (inet_ntop(AF_INET, &(saddr.sin_addr), ipname, INET_ADDRSTRLEN) == NULL)
        return "unknown";
    ipname[INET_ADDRSTRLEN] = 0;
    sprintf(ipname + strlen(ipname), ":%d", (int)htons(saddr.sin_port));
    return ipname;
}

int TcpSocket::Send(const char *buf, int buf_len)
{
    int nwrote;

    nwrote = buf_len > 0 ? send(sock_fd_, buf, buf_len, 0) : 0;
    return nwrote;
}

int TcpSocket::Recv(char *buf, int buf_len)
{
    int nread;

    nread = buf_len > 0 ? recv(sock_fd_, buf, buf_len, 0) : 0;

    return nread;
}


bool TcpSocket::IsGood() const
{
    return (sock_fd_ >= 0);
}


void TcpSocket::Close()
{
    if (sock_fd_ < 0) {
        return;
    }
    close(sock_fd_);
    sock_fd_ = -1;
}

int TcpSocket::GetSocketError() const
{
    if (sock_fd_ < 0) {
        return -EBADF;
    }
    int       err = 0;
    socklen_t len = sizeof(err);
    if (getsockopt(sock_fd_, SOL_SOCKET, SO_ERROR, &err, &len)) {
        return (errno != 0 ? errno : -EINVAL);
    }
    assert(len == sizeof(err));
    return err;
}