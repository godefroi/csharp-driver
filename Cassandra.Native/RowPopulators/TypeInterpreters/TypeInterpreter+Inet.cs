﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Cassandra.Native
{

    internal partial class TypeInterpreter
    {
        public static object ConvertFromInet(TableMetadata.ColumnInfo type_info, byte[] value)
        {
            var length = value[0];
            IPAddress ip;
            int port;
            var buf = new byte[length];
            if (length == 4)
            {
                Buffer.BlockCopy(value, 1, buf, 0, 4);
                ip = new IPAddress(buf);
                port = ConversionHelper.FromBytesToInt32(buf, 1 + 4);
                return new IPEndPoint(ip, port);
            }
            else if (length == 16)
            {
                Buffer.BlockCopy(value, 1, buf, 0, 16);
                ip = new IPAddress(buf);
                port = ConversionHelper.FromBytesToInt32(buf, 1 + 16);
                return new IPEndPoint(ip, port);
            }
            throw new DriverInternalError("Invalid lenght of Inet Addr");
        }

        public static Type GetTypeFromInet(TableMetadata.ColumnInfo type_info)
        {
            return typeof(IPEndPoint);
        }

        public static byte[] InvConvertFromInet(TableMetadata.ColumnInfo type_info, object value)
        {
            checkArgument<IPEndPoint>(value);
            var addrbytes = (value as IPEndPoint).Address.GetAddressBytes();
            var port = ConversionHelper.ToBytesFromInt32((value as IPEndPoint).Port);
            var ret = new byte[addrbytes.Length + 4 + 1];
            ret[0] = (byte)addrbytes.Length;
            Buffer.BlockCopy(addrbytes, 0, ret, 1, addrbytes.Length);
            Buffer.BlockCopy(port, 0, ret, 1 + addrbytes.Length, port.Length);
            return ret;
        }
    }
}
