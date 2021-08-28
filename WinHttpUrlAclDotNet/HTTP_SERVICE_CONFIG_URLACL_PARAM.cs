using System;
using System.Runtime.InteropServices;

namespace WinHttpServerApi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct HTTP_SERVICE_CONFIG_URLACL_PARAM
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pStringSecurityDescriptor;

        public HTTP_SERVICE_CONFIG_URLACL_PARAM(string securityDescriptor)
        {
            pStringSecurityDescriptor = securityDescriptor;
        }
    }
}
