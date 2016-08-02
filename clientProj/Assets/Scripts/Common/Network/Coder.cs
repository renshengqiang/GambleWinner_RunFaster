using ProtoBuf;
using System.IO;
using System.Collections.Generic;
using System;

namespace Common.Network
{
    public class Coder : IDisposable
    {
        private Buffer buffer;
        private Dictionary<uint, Type> dicTypeInfo;

        public Coder()
        {
            buffer = new Buffer(4096);
            dicTypeInfo = new Dictionary<uint, Type>();
        }

        public void RegisterMsgId(uint msgId, Type t)
        {
            dicTypeInfo[msgId] = t;
        }

        public byte[] Encode<T>(uint msgId, T msg)
        {
            buffer.Clear();
            buffer.WriteInt((int)msgId);
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, msg);
                buffer.WriteBytes(ms.ToArray());
            }
            return buffer.GetRaw();
        }

        public Message Decode(byte[] arr)
        {
            Message msg = new Message();
            buffer.Clear();
            buffer.WriteBytes(arr);
            uint msgId = (uint)buffer.ReadInt();
            byte[] msgBytes = buffer.ReadByteArray(arr.Length - sizeof(int));
            if(msgBytes != null && dicTypeInfo.ContainsKey(msgId))
            {
                using (MemoryStream ms = new MemoryStream(msgBytes))
                {
                    Type t = dicTypeInfo[msgId];
                    object obj = Serializer.Deserialize(ms, t);
                    msg.id = msgId;
                    msg.msg = obj;
                }
            }
            else
            {
                Logger.Error(string.Format("Can't decode msg of id {0}", msgId));
            }
            return msg;
        }

        public void Dispose()
        {
            buffer.Clear();
            dicTypeInfo.Clear();
        }
    }
}

