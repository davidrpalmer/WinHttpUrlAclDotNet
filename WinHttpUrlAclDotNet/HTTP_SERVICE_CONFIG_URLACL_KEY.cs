using System;
using System.Runtime.InteropServices;

namespace WinHttpServerApi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct HTTP_SERVICE_CONFIG_URLACL_KEY
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pUrlPrefix;

        public HTTP_SERVICE_CONFIG_URLACL_KEY(string urlPrefix)
        {
            pUrlPrefix = urlPrefix;
        }
    }
}
