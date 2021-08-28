namespace WinHttpServerApi
{
    /// <summary>
    /// Note that other Win32 error codes may also be returned, even if they are not in this enum.
    /// </summary>
    public enum Win32ErrorCode : int
    {
        NO_ERROR = 0,

        /// <summary>
        /// For the HTTP Server API this normally means the URL ACL to delete does not exist.
        /// </summary>
        ERROR_FILE_NOT_FOUND = 2,

        /// <summary>
        /// For the HTTP Server API this normally means not running as an elevated admin.
        /// </summary>
        ERROR_ACCESS_DENIED = 5,

        /// <summary>
        /// The specified record already exists, and must be deleted in order for its value to be re-set.
        /// </summary>
        ERROR_ALREADY_EXISTS = 183,

        /// <summary>
        /// The buffer size specified in the ConfigInformationLength parameter is insufficient.
        /// </summary>
        ERROR_INSUFFICIENT_BUFFER = 122,

        /// <summary>
        /// The ServiceHandle parameter is invalid.
        /// </summary>
        ERROR_INVALID_HANDLE = 6,

        /// <summary>
        /// One or more of the supplied parameters is in an unusable form.
        /// </summary>
        ERROR_INVALID_PARAMETER = 87,

        ERROR_NO_MORE_ITEMS = 259,

        /// <summary>
        /// The SSL Certificate used is invalid. This can occur only if the HttpServiceConfigSSLCertInfo parameter is used.
        /// </summary>
        ERROR_NO_SUCH_LOGON_SESSION = 1312,
    }
}
