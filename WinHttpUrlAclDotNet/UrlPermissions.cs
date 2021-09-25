using System;
#if NETFRAMEWORK //TODO If .NET CORE then use the NuGet package with these types in instead.
using System.Security.Principal;
#endif

namespace WinHttpServerApi
{
    public class UrlPermissions
    {
        /// <summary>
        /// The account SID to grant access to.
        /// <para>
        /// Examples:<br/>
        ///   * "S-1-5-32-544"<br/>
        ///   * "BA"  (SDDL short form)<br/>
        /// </para>
        /// </summary>
        public string Sid { get; set; }

        public bool Listen { get; set; } = true;

        public bool Delegate { get; set; }

#if NETFRAMEWORK
        public void SetSid(NTAccount account) => SetSid((SecurityIdentifier)account.Translate(typeof(SecurityIdentifier)));

        public void SetSid(WellKnownSidType sid) => SetSid(new SecurityIdentifier(sid, null));

        public void SetSid(SecurityIdentifier sid)
        {
            Sid = sid?.Value;
        }

        public SecurityIdentifier GetSidObject()
        {
            if (string.IsNullOrEmpty(Sid))
            {
                return null;
            }
            return new SecurityIdentifier(Sid);
        }

        public string GetSidName()
        {
            if (string.IsNullOrEmpty(Sid))
            {
                return null;
            }
            return GetSidObject().Translate(typeof(NTAccount)).Value;
        }
#endif
    }
}
