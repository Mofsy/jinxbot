using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.MBNCSUtil;

namespace JinxBot.Plugins.McpHandler
{
    public class McpPacket : DataBuffer
    {
        public McpPacket(byte id)
        {
            InsertInt16(0);
            InsertByte(id);
        }

        public override byte[] UnderlyingBuffer
        {
            get
            {
                byte[] buffer = base.UnderlyingBuffer;
                ushort length = unchecked((ushort)(Count - 3));
                byte[] len = BitConverter.GetBytes(length);
                buffer[0] = len[0];
                buffer[1] = len[1];
                return buffer;
            }
        }
    }
}
