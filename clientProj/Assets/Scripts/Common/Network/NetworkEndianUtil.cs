using System;
using System.Net;
using System.IO;

namespace Common.Network
{
    public class NetworkEndianUtil
    {
        public static byte[] ConvertToBytes(int value)
        {
            UInt32 bigEndianValue = (uint)IPAddress.HostToNetworkOrder(value);
            byte[] arr = BitConverter.GetBytes(bigEndianValue);
            return arr;
        }

        public static byte[] ConvertToBytes(short value)
        {
            UInt32 bigEndianValue = (uint)IPAddress.HostToNetworkOrder(value);
            byte[] arr = BitConverter.GetBytes(bigEndianValue);
            return arr;
        }

        public static byte[] ConvertToBytes(long value)
        {
            UInt32 bigEndianValue = (uint)IPAddress.HostToNetworkOrder(value);
            byte[] arr = BitConverter.GetBytes(bigEndianValue);
            return arr;
        }

        public static int ConvertIntFromBytes(byte[] source, int offset = 0)
        {
            using (MemoryStream instanceSource = new MemoryStream(source, offset, 4))
            using (BinaryReader instanceReader = new BinaryReader(instanceSource))
            {
                return IPAddress.NetworkToHostOrder(instanceReader.ReadInt32());
            }
        }

        public static short ConvertShortFromBytes(byte[] source, int offset = 0)
        {
            using (MemoryStream instanceSource = new MemoryStream(source, offset, 2))
            using (BinaryReader instanceReader = new BinaryReader(instanceSource))
            {
                return IPAddress.NetworkToHostOrder(instanceReader.ReadInt16());
            }
        }

        public static long ConvertLongFromBytes(byte[] source, int offset = 8)
        {
            using (MemoryStream instanceSource = new MemoryStream(source, offset, 8))
            using (BinaryReader instanceReader = new BinaryReader(instanceSource))
            {
                return IPAddress.NetworkToHostOrder(instanceReader.ReadInt64());
            }
        }
    }
}

