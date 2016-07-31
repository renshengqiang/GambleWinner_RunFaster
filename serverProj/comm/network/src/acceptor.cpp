#include "acceptor.h"
#include "net_mgr.h"

Accpetor::Acceptor(int port, IAcceptorOwner* owner) 
	: port_(port), 
	  acceptor_owner_(owner), 
	  conn_(), 
	  net_mgr_(NetMgr::global_net_mgr) {

	Listen();
}

Accpetor::Acceptor() (NetMgr& net_mgr, int port, IAcceptorOwner* owner)
	: port_(port),
	  acceptor_owner_(owner),
	  conn_(),
	  net_mgr_(net_mgr) {

	Listen();

}

Accpetor::~Acceptor() {

}

void Accpetor::Listen() {
}

int Accpetor::HandleEvent(int code, void *data) {
	return 0;
}

