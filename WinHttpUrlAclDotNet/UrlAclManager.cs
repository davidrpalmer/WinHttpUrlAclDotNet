﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
#if NET451
using System.Security.Principal;
#endif

namespace WinHttpServerApi
{
    public class UrlAclManager : IDisposable
    {
        private bool _disposedValue;

        public UrlAclManager()
        {
            Win32ErrorCode result = NativeHttpApi.HttpInitialize(HTTPAPI_VERSION.HTTPAPI_VERSION_1, HttpInitializeFlags.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

            if (result != Win32ErrorCode.NO_ERROR)
            {
                throw new Win32Exception((int)result);
            }
        }

#if NET451
        /// <param name="permissions">The permissions to grant the specified account. NULL for default.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public void AddUrl(string networkURL, NTAccount account, UrlPermissions permissions, bool overwrite) => AddUrl(networkURL, (SecurityIdentifier)account?.Translate(typeof(SecurityIdentifier)), permissions, overwrite);

        /// <param name="permissions">The permissions to grant the specified SID. NULL for default.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public void AddUrl(string networkURL, WellKnownSidType sid, UrlPermissions permissions, bool overwrite) => AddUrl(networkURL, new SecurityIdentifier(sid, null), permissions, overwrite);

        /// <param name="permissions">The permissions to grant the specified SID. NULL for default.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public void AddUrl(string networkURL, SecurityIdentifier sid, UrlPermissions permissions, bool overwrite) => AddUrlBySidString(networkURL, sid.ToString(), permissions, overwrite);
#endif

        /// <param name="sid">The account SID to grant access to. Example: S-1-1-0</param>
        /// <param name="permissions">The permissions to grant the specified SID. NULL for default.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public void AddUrlBySidString(string networkURL, string sid, UrlPermissions permissions, bool overwrite)
        {
            if (string.IsNullOrWhiteSpace(sid))
            {
                throw new ArgumentNullException(nameof(sid));
            }
            permissions = permissions ?? new UrlPermissions();

            char allowDeny;
            char permission;
            if (permissions.Delegate == permissions.Listen)
            {
                allowDeny = permissions.Delegate ? 'A' : 'D';
                permission = 'A';
            }
            else
            {
                allowDeny = 'A';
                permission = permissions.Listen ? 'X' : 'W';
            }

            string sddl = $"D:({allowDeny};;G{permission};;;{sid})";

            AddUrl(networkURL, sddl, overwrite);
        }

        /// <param name="securityDescriptor">An SDDL string. Example: D:(A;;GX;;;S-1-1-0)</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public void AddUrl(string networkUrl, string securityDescriptor, bool overwrite)
        {
            AssertNotDisposed();
            if (string.IsNullOrWhiteSpace(networkUrl))
            {
                throw new ArgumentNullException(nameof(networkUrl));
            }
            if (string.IsNullOrWhiteSpace(securityDescriptor))
            {
                throw new ArgumentNullException(nameof(securityDescriptor));
            }

            Win32ErrorCode result;

            HTTP_SERVICE_CONFIG_URLACL_SET inputConfigInfoSet = new HTTP_SERVICE_CONFIG_URLACL_SET
            {
                KeyDesc = new HTTP_SERVICE_CONFIG_URLACL_KEY(networkUrl),
                ParamDesc = new HTTP_SERVICE_CONFIG_URLACL_PARAM(securityDescriptor)
            };

            result = NativeHttpApi.HttpSetServiceConfiguration(inputConfigInfoSet);

            if (result == Win32ErrorCode.ERROR_ALREADY_EXISTS && overwrite)
            {
                result = NativeHttpApi.HttpDeleteServiceConfiguration(inputConfigInfoSet);

                if (result == Win32ErrorCode.NO_ERROR)
                {
                    result = NativeHttpApi.HttpSetServiceConfiguration(inputConfigInfoSet);
                }
            }

            if (result != Win32ErrorCode.NO_ERROR)
            {
                throw new Win32Exception((int)result);
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="networkUrl"/> is null, empty or white space.</exception>
        /// <exception cref="Win32Exception"></exception>
        public void DeleteUrl(string networkUrl)
        {
            AssertNotDisposed();
            if (string.IsNullOrWhiteSpace(networkUrl))
            {
                throw new ArgumentNullException(nameof(networkUrl));
            }

            Win32ErrorCode result;

            HTTP_SERVICE_CONFIG_URLACL_SET inputConfigInfoSet = new HTTP_SERVICE_CONFIG_URLACL_SET
            {
                KeyDesc = new HTTP_SERVICE_CONFIG_URLACL_KEY(networkUrl),
                ParamDesc = new HTTP_SERVICE_CONFIG_URLACL_PARAM(null)
            };

            result = NativeHttpApi.HttpDeleteServiceConfiguration(inputConfigInfoSet);

            if (result != Win32ErrorCode.NO_ERROR)
            {
                throw new Win32Exception((int)result);
            }
        }

        /// <exception cref="Win32Exception"></exception>
        public List<UrlReservation> QueryUrls()
        {
            List<UrlReservation> urls = new List<UrlReservation>();

            int outputBufferLength = 512; // Start with a buffer that is probably big enough so we won't need to expand it.
            int requiredOutputBufferLength;
            HTTP_SERVICE_CONFIG_URLACL_QUERY query = new HTTP_SERVICE_CONFIG_URLACL_QUERY()
            {
                QueryDesc = HTTP_SERVICE_CONFIG_QUERY_TYPE.HttpServiceConfigQueryNext,
                dwToken = 0,
                KeyDesc = new HTTP_SERVICE_CONFIG_URLACL_KEY()
            };

            IntPtr pOutputConfigInfo = Marshal.AllocCoTaskMem(outputBufferLength);

            try
            {
                Win32ErrorCode queryResult;

                while (true)
                {
                    while (true)
                    {
                        IntPtr pQuery = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(HTTP_SERVICE_CONFIG_URLACL_QUERY)));
                        Marshal.StructureToPtr(query, pQuery, false);
                        try
                        {
                            queryResult = NativeHttpApi.HttpQueryServiceConfiguration(IntPtr.Zero, HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, pQuery, Marshal.SizeOf(query), pOutputConfigInfo, outputBufferLength, out requiredOutputBufferLength, IntPtr.Zero);
                        }
                        finally
                        {
                            Marshal.FreeCoTaskMem(pQuery);
                            pQuery = IntPtr.Zero;
                        }
                        if (queryResult == Win32ErrorCode.NO_ERROR || queryResult == Win32ErrorCode.ERROR_NO_MORE_ITEMS)
                        {
                            break;
                        }
                        else if (queryResult == Win32ErrorCode.ERROR_INSUFFICIENT_BUFFER)
                        {
                            outputBufferLength = requiredOutputBufferLength;
                            Marshal.FreeCoTaskMem(pOutputConfigInfo);
                            pOutputConfigInfo = IntPtr.Zero;
                            pOutputConfigInfo = Marshal.AllocCoTaskMem(outputBufferLength);
                        }
                        else
                        {
                            throw new Win32Exception((int)queryResult);
                        }
                    }

                    if (queryResult == Win32ErrorCode.NO_ERROR)
                    {
                        var data = Marshal.PtrToStructure<HTTP_SERVICE_CONFIG_URLACL_SET>(pOutputConfigInfo);
                        urls.Add(new UrlReservation(data.KeyDesc.pUrlPrefix, data.ParamDesc.pStringSecurityDescriptor));
                        query.dwToken++;
                    }
                    else
                    {
                        return urls;
                    }
                }
            }
            finally
            {
                if (pOutputConfigInfo != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pOutputConfigInfo);
                }
            }
        }

        #region IDisposable

        private void AssertNotDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(UrlAclManager));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                NativeHttpApi.HttpTerminate(HttpInitializeFlags.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
                _disposedValue = true;
            }
        }

        ~UrlAclManager()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
