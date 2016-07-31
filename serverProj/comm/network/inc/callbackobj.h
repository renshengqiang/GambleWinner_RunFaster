#ifndef CALLBACK_OBJ_H_
#define CALLBACK_OBJ_H_

namespace network {

enum EventCode_t
{
	EVENT_NET_CONNECT,
	EVENT_NET_DISCONNECT,
	EVENT_NET_READ,
	EVENT_NET_WROTE,
	EVENT_NET_ERROR
};

class Callbackobj
{
public:
	 virtual int HandleEvent(int code, void *data) = 0 ;
};

}

#endif //CALLBACK_OBJ_H_