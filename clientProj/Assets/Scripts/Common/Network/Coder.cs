
namespace Common.Network
{
    public class Coder
    {
        public byte[] Encode<T>(uint msgId, T msg)
        {
            return null;
        }

        public Message Decode(Buffer buffer)
        {
            Message msg = new Message();

            return msg;
        }

        public int HeadLength()
        {
            return 0;
        }

        public int GetBodyLengh(Buffer buffer, int startIndex)
        {
            return 0;
        }
    }
}

