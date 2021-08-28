using System;

namespace WinHttpServerApi
{
    public class UrlPermissions
    {
        // Note: Currently only using this class when adding an ACL. Got no code to read an SDDL back into this.

        public bool Listen { get; set; } = true;

        public bool Delegate { get; set; }
    }
}
