namespace WinHttpServerApi
{
    enum HTTP_SERVICE_CONFIG_QUERY_TYPE
    {
        /// <summary>
        /// Returns a single record. dwToken is ignored.
        /// </summary>
        HttpServiceConfigQueryExact = 0,

        /// <summary>
        /// Returns a sequence of records in a sequence of calls, controlled by the dwToken parameter.
        /// </summary>
        HttpServiceConfigQueryNext,
    }
}
