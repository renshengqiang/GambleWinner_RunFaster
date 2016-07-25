using System;
using System.Text;

namespace Common.Network
{
	public class Buffer
	{
		private int     len;            // buffer size
		private byte[]  data;           // buffer data
		private int     readerIndex;    // from which we can read data
		private int     writerIndex;    // from which we can write data

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="capacity">capacity of buffer</param>
		public Buffer (int capacity)
		{
			this.len = capacity;
			this.data = new byte[len];
			readerIndex = 0;
			writerIndex = 0;
		}

		/// <summary>
		/// get the capacity of the buffer
		/// </summary>
		/// <returns></returns>
		public int Capacity ()
		{
			return len;
		}

		/// <summary>
		/// Expand the capacity of the buffer
		/// </summary>
        /// <param name="newCapicity"></param>
		/// <returns></returns>
		public Buffer Capacity (int newCapicity)
		{
            if (newCapicity > len)
            {
				byte[] old = data;
                data = new byte[newCapicity];
				Array.Copy (old, data, len);
                len = newCapicity;
			}
			return this;
		}

        /// <summary>
        /// Clear the buffer
        /// </summary>
        /// <returns></returns>
		public Buffer Clear ()
		{
			readerIndex = 0;
			writerIndex = 0;
			return this;
		}
		
        /// <summary>
        /// Deep Copy the buffer object
        /// </summary>
        /// <returns></returns>
		public Buffer Copy()
		{
			Buffer item = new Buffer(len);
			Array.Copy (this.data, item.data, len);
			item.readerIndex = readerIndex;
			item.writerIndex = writerIndex;
			return item;
		}

		/// <summary>
		/// Get one byte value from the buffer at index
        /// Not move the read index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public byte GetByte(int index)
		{
			if (index < len)
			{
				return data[index];
			}
			return (byte)0;
		}

        /// <summary>
        /// Get one int value from the buffer at index
        /// Not move the read index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public 	int GetInt(int index)
		{
			if (index + 3 < len)
			{
				int ret = ((int) data[index]) << 24;
				ret |= ((int) data[index + 1]) << 16;
				ret |= ((int) data[index + 2]) << 8;
				ret |= ((int) data[index + 3]);
				return ret;
			}
			return 0;
		}

        /// <summary>
        /// Get one short value from the buffer at index
        /// Not move the read index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public short GetShort(int index)
		{
			if (index + 1 < len)
			{
				short r1 = (short)(data[index] << 8);
				short r2 = (short)(data[index + 1]);
				short ret = (short)(r1 | r2);
				return ret;
			}
			return 0;
		}

        /// <summary>
        /// Get one int from the net endian buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public int GetNetEndianInt()
        {
            int ret = 0;
            if (readerIndex + 3 < writerIndex)
            {
                ret = NetworkEndianUtil.ConvertIntFromBytes(GetRaw(), ReaderIndex());
            }
            return 0;
        }

        /// <summary>
        /// Read one short from the net endian buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public short GetNetEndianShort()
        {
            short ret = 0;
            if (readerIndex + 1 < writerIndex)
            {
                ret = NetworkEndianUtil.ConvertShortFromBytes(GetRaw(), ReaderIndex());
            }
            return 0;
        }

        /// <summary>
        /// Read one long from the net endian buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public int GetNetEndianLong()
        {
            long ret = 0;
            if (readerIndex + 7 < writerIndex)
            {
                ret = NetworkEndianUtil.ConvertLongFromBytes(GetRaw(), ReaderIndex());
            }
            return 0;
        }

        /// <summary>
        /// Read one byte value from buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            if (readerIndex < writerIndex)
            {
                byte ret = data[readerIndex++];
                return ret;
            }
            return (byte)0;
        }

        /// <summary>
        /// Read one int value from buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            if (readerIndex + 3 < writerIndex)
            {
                unchecked
                {
                    int ret = (int)(((data[readerIndex++]) << 24) & 0xff000000);
                    ret |= (((data[readerIndex++]) << 16) & 0x00ff0000);
                    ret |= (((data[readerIndex++]) << 8) & 0x0000ff00);
                    ret |= (((data[readerIndex++])) & 0x000000ff);
                    return ret;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read one short value from buffer at the write index and move the index
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            if (readerIndex + 1 < writerIndex)
            {
                int h = data[readerIndex++];
                int l = data[readerIndex++] & 0x000000ff;
                int len = ((h << 8) & 0x0000ff00) | (l);
                return (short)len;
            }
            return 0;
        }

        /// <summary>
        /// Read one int from the net endian buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public int ReadNetEndianInt()
        {
            int ret = 0;
            if (readerIndex + 3 < writerIndex)
            {
                ret = NetworkEndianUtil.ConvertIntFromBytes(GetRaw(), ReaderIndex());
                readerIndex += 4;
            }
            return 0;
        }

        /// <summary>
        /// Read one short from the net endian buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public short ReadNetEndianShort()
        {
            short ret = 0;
            if (readerIndex + 1 < writerIndex)
            {
                ret = NetworkEndianUtil.ConvertShortFromBytes(GetRaw(), ReaderIndex());
                readerIndex += 2;
            }
            return 0;
        }

        /// <summary>
        /// Read one long from the net endian buffer at the reader index and move the index
        /// </summary>
        /// <returns></returns>
        public int ReadNetEndianLong()
        {
            long ret = 0;
            if (readerIndex + 7 < writerIndex)
            {
                ret = NetworkEndianUtil.ConvertLongFromBytes(GetRaw(), ReaderIndex());
                readerIndex += 8;
            }
            return 0;
        }

        /// <summary>
        /// read byte array data from buffer at
        /// </summary>
        /// <param name="length"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte[] ReadByteArray(int length)
        {
            byte[] outArray = new byte[length];
            Array.Copy(GetRaw(), outArray, length);
            readerIndex += length;
            return outArray;
        }

        /// <summary>
        /// Get the read index of the buffer
        /// </summary>
        /// <returns></returns>
		public int ReaderIndex()
		{
			return readerIndex;
		}
		
		/// <summary>
		/// set byte data at index with value
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Buffer SetByte(int index, byte value)
		{
			if (index < len)
			{
				data[index] = value;
			}
			return this;
		}

		/// <summary>
		/// set bytes data from index
		/// </summary>
		/// <param name="index">from index of the buffer</param>
		/// <param name="src">source bytes data</param>
		/// <param name="from">from index of src</param>
		/// <param name="len">copy length</param>
		/// <returns></returns>
		public Buffer SetBytes(int index, byte[] src, int from, int len)
		{
			if (index + len <= len)
			{
				Array.Copy (src, from, data, index, len);
			}
			return this;
		}

        /// <summary>
        /// set int data at index with value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public Buffer SetInt(int index, int value)
		{
			if (index + 4 <= len)
			{
				data[index++] = (byte)((value >> 24) & 0xff);
				data[index++] = (byte)((value >> 16) & 0xff);
				data[index++] = (byte)((value >> 8) & 0xff);
				data[index++] = (byte)(value & 0xff);
			}
			return this;
		}

        /// <summary>
        /// set short data at index with value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public Buffer SetShort(int index, short value)
		{
			if (index + 2 <= len)
			{
				data[index++] = (byte)((value >> 8) & 0xff);
				data[index++] = (byte)(value & 0xff);
			}
			return this;
		}

		/// <summary>
		/// skip some bytes
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public Buffer SkipBytes(int length)
		{
			if (readerIndex + length <= writerIndex)
			{
				readerIndex += length;
			}
			return this;
		}


        /// <summary>
        /// Get the writable length of the buffer
        /// </summary>
        /// <returns></returns>
        public int WritableBytes()
        {
            return len - writerIndex;
        }

        /// <summary>
        /// Get the Readable length of the buffer
        /// </summary>
        /// <returns></returns>
        public int ReadableBytes()
        {
            return writerIndex - readerIndex;
        }

        /// <summary>
        /// write byte data at write index with value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
		public Buffer WriteByte(byte value)
		{
			this.Capacity(writerIndex + 1);
			this.data[writerIndex++] = value;
			return this;
		}

        /// <summary>
        /// write int data at write index with value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public Buffer WriteInt(int value)
		{
			Capacity(writerIndex + 4);
			data[writerIndex++] = (byte)((value >> 24) & 0xff);
			data[writerIndex++] = (byte)((value >> 16) & 0xff);
			data[writerIndex++] = (byte)((value >> 8) & 0xff);
			data[writerIndex++] = (byte)(value & 0xff);
			return this;
		}

        /// <summary>
        /// write short data at write index with value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns> 
		public Buffer WriteShort(short value)
		{
			Capacity(writerIndex + 2);
			data[writerIndex++] = (byte)((value >> 8) & 0xff);
			data[writerIndex++] = (byte)(value & 0xff);
			return this;
		}

        /// <summary>
        /// write bytes data at write index
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Buffer WriteBytes(byte[] src)
        {
            int sum = src.Length;
            Capacity(writerIndex + sum);
            if (sum > 0)
            {
                Array.Copy(src, 0, data, writerIndex, sum);
                writerIndex += sum;
            }
            return this;
        }

        /// <summary>
        /// write bytes data at write index
        /// </summary>
        /// <param name="src"></param>
        /// <param name="off">from index of the source buffer</param>
        /// <param name="len">write length</param>
        /// <returns></returns>
        public Buffer WriteBytes(byte[] src, int off, int len)
        {
            int sum = len;
            if (sum > 0)
            {
                Capacity(writerIndex + sum);
                Array.Copy(src, off, data, writerIndex, sum);
                writerIndex += sum;
            }
            return this;
        }

        /// <summary>
        /// write the available bytes of src to this buffer
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
		public Buffer WriteBytes(Buffer src)
		{
			int sum = src.writerIndex - src.readerIndex;
			Capacity(writerIndex + sum);
			if (sum > 0)
			{
				Array.Copy (src.data, src.readerIndex, data, writerIndex, sum);
				writerIndex += sum;
				src.readerIndex += sum;
			}
			return this;
		}
        /// <summary>
        /// write src's data to this buffer
        /// </summary>
        /// <param name="src"></param>
        /// <param name="len">write index</param>
        /// <returns></returns>
        public Buffer WriteBytes(Buffer src, int len)
        {
            if (len > 0)
            {
                Capacity(writerIndex + len);
                Array.Copy(src.data, src.readerIndex, data, writerIndex, len);
                writerIndex += len;
                src.readerIndex += len;
            }
            return this;
        }

		/// <summary>
		/// Get the write index the buffer
		/// </summary>
		/// <returns></returns>
		public int WriterIndex()
		{
			return writerIndex;
		}

		/// <summary>
		/// Get raw data of buffer
		/// </summary>
		/// <returns></returns>
		public byte[] GetRaw()
		{
			return data;
		}

        public string ReadUTF8()
        {
            short len = ReadShort(); // 字节数
            byte[] charBuff = new byte[len]; //
            Array.Copy(data, readerIndex, charBuff, 0, len);
            readerIndex += len;
            return Encoding.UTF8.GetString(charBuff);
        }

        /**
         * 写入utf字符串
         * 
         **/
        public Buffer WriteUTF8(string value)
        {
            byte[] content = Encoding.UTF8.GetBytes(value.ToCharArray());
            int len = content.Length;
            Capacity(writerIndex + len + 2);
            WriteShort((short)len);
            Array.Copy(content, 0, data, writerIndex, len);
            writerIndex += len;
            return this;
        }
	}
}

