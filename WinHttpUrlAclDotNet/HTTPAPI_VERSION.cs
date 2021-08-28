using System;
using System.Runtime.InteropServices;

namespace WinHttpServerApi
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    struct HTTPAPI_VERSION
    {
        public static readonly HTTPAPI_VERSION HTTPAPI_VERSION_1 = new HTTPAPI_VERSION(1, 0);
        public static readonly HTTPAPI_VERSION HTTPAPI_VERSION_2 = new HTTPAPI_VERSION(2, 0);

        public readonly ushort HttpApiMajorVersion;
        public readonly ushort HttpApiMinorVersion;

        public HTTPAPI_VERSION(ushort majorVersion, ushort minorVersion)
        {
            HttpApiMajorVersion = majorVersion;
            HttpApiMinorVersion = minorVersion;
        }
    }
}
