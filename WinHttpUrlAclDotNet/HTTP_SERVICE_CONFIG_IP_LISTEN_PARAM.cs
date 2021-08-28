using System;
using System.Runtime.InteropServices;

namespace WinHttpServerApi
{
    [StructLayout(LayoutKind.Sequential)]
    struct HTTP_SERVICE_CONFIG_IP_LISTEN_PARAM
    {
        public ushort AddrLength;
        public IntPtr pAddress;
    }
}
