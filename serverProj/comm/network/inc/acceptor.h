#ifndef ACCEPTOR_H_
#define ACCEPTOR_H_

#include "callbackobj.h"
#include "net_conn.h"

namespace network
{

class NetMgr;
class IAcceptorOwner {
public:
    virtual ~IAcceptorOwner() { };

    virtual CallbackObj *CreateCallbackObj(NetConnPtr conn) = 0;
};

class Acceptor: public Callbackobj
{
public:
	Acceptor(int port, IAcceptorOwner* owner);
	Acceptor(NetMgr& net_mgr, int port, IAcceptorOwner* owner);
	~Acceptor();

	int HandleEvent(int code, void *data); 

private:
	const int port_;
	IAcceptorOwner* const acceptor_owner_;
	NetConnPtr conn_;
	NetMgr& net_mgr_;
private:
	void Listen();

};

}

#endif //ACCEPOR_H_