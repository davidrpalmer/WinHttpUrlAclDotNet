using System;
using System.Runtime.InteropServices;

namespace WinHttpServerApi
{
    class NativeHttpApi
    {
        private const string HttpApiDll = "httpapi.dll";

        [DllImport(HttpApiDll)]
        public static extern Win32ErrorCode HttpInitialize(HTTPAPI_VERSION version, HttpInitializeFlags flags, IntPtr pReserved);

        [DllImport(HttpApiDll)]
        public static extern Win32ErrorCode HttpTerminate(HttpInitializeFlags flags, IntPtr pReserved);

        [DllImport(HttpApiDll, EntryPoint = "HttpSetServiceConfiguration", ExactSpelling = true)]
        static extern Win32ErrorCode HttpSetServiceConfigurationAcl(
            IntPtr ServiceHandle,
            HTTP_SERVICE_CONFIG_ID configID,
            [In] ref HTTP_SERVICE_CONFIG_URLACL_SET configInfo,
            int configInfoLength,
            IntPtr pOverlapped);

        [DllImport(HttpApiDll, EntryPoint = "HttpDeleteServiceConfiguration", ExactSpelling = true)]
        static extern Win32ErrorCode HttpDeleteServiceConfigurationAcl(
            IntPtr ServiceHandle,
            HTTP_SERVICE_CONFIG_ID configID,
            [In] ref HTTP_SERVICE_CONFIG_URLACL_SET configInfo,
            int configInfoLength,
            IntPtr pOverlapped);

        public static Win32ErrorCode HttpSetServiceConfiguration(HTTP_SERVICE_CONFIG_URLACL_SET data)
        {
            return HttpSetServiceConfigurationAcl(IntPtr.Zero,
                HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
                ref data,
                Marshal.SizeOf(data),
                IntPtr.Zero);
        }

        public static Win32ErrorCode HttpDeleteServiceConfiguration(HTTP_SERVICE_CONFIG_URLACL_SET data)
        {
            return HttpDeleteServiceConfigurationAcl(IntPtr.Zero,
                HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
                ref data,
                Marshal.SizeOf(data),
                IntPtr.Zero);
        }


        [DllImport(HttpApiDll, ExactSpelling = true)]
        public static extern Win32ErrorCode HttpQueryServiceConfiguration(
            IntPtr ServiceIntPtr,
            HTTP_SERVICE_CONFIG_ID ConfigId,
            IntPtr pInputConfigInfo,
            int InputConfigInfoLength,
            IntPtr pOutputConfigInfo,
            int OutputConfigInfoLength,
            [Optional()]
            out int pReturnLength,
            IntPtr pOverlapped);
    }
}
