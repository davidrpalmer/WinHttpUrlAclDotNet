using System;
using System.Security.Principal;

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

        /// <summary>
        /// Allow the user to register URLs. Default: true
        /// </summary>
        public bool Listen { get; set; } = true;

        /// <summary>
        /// Allow the user to delegate URLs. Default: false
        /// </summary>
        public bool Delegate { get; set; }

        public UrlPermissions()
        {
        }

        public UrlPermissions(NTAccount account)
        {
            SetSid(account);
        }

        public UrlPermissions(WellKnownSidType sid)
        {
            SetSid(sid);
        }

        public UrlPermissions(SecurityIdentifier sid)
        {
            SetSid(sid);
        }

        public void SetSid(NTAccount account) => SetSid((SecurityIdentifier)account?.Translate(typeof(SecurityIdentifier)));

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
    }
}
