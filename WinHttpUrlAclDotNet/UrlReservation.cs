using System;

namespace WinHttpServerApi
{
    [System.Diagnostics.DebuggerDisplay("{Url}")]
    public class UrlReservation
    {
        public UrlReservation(string url, string sddl)
        {
            Url = url;
            Sddl = sddl;
        }

        public string Url { get; }

        public string Sddl { get; }
    }
}
