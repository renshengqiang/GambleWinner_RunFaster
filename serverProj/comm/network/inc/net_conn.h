#ifndef NETWORK_NET_CONN_H_
#define NETWORK_NET_CONN_H_

#include <boost/shared_ptr.hpp>

#include "tcp_socket.h"
#include "callbackobj.h"

namespace network 
{

/// A network connection uses tcp socket for doing I/O.
class NetConn
{
public:
	typedef boost::shared_ptr<NetConn> NetConnPtr;
public:
	 NetConn(TcpSocket* sock, Callbackobj* c, bool listen_only = false, bool own_socket = true);
	~ NetConn();
	
};

typedef NetConn::NetConnPtr NetConnPtr;

}

#endif //NETWORK_NET_CONN_H_