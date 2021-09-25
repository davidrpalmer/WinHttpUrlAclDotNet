using System;
using System.Linq;

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

#if !NET451
        /// <summary>
        /// Parse the <see cref="Sddl"/>.
        /// </summary>
        public UrlPermissions[] GetPermissions()
        {
            var parsedSddl = new Sddl.Parser.Sddl(Sddl);
            ThrowSddlErrorsIfInvalid(parsedSddl);
            UrlPermissions[] result = new UrlPermissions[parsedSddl.Dacl.Aces.Length];
            for (int i = 0; i < parsedSddl.Dacl.Aces.Length; i++)
            {
                var ace = parsedSddl.Dacl.Aces[i];
                ThrowSddlErrorsIfInvalid(ace);
                result[i] = new UrlPermissions()
                {
                    Sid = ace.AceSid.Raw,

                    // Init the permissions to false (some default to true)...
                    Delegate = false,
                    Listen = false
                };

                if (ace.AceType == "ACCESS_ALLOWED")
                {
                    foreach (string right in ace.Rights)
                    {
                        switch (right)
                        {
                            case "GENERIC_ALL":
                                result[i].Delegate = true;
                                result[i].Listen = true;
                                break;
                            case "GENERIC_EXECUTE":
                                result[i].Listen = true;
                                break;
                            case "GENERIC_WRITE":
                                result[i].Delegate = true;
                                break;
                        }
                    }
                }
            }
            return result;
        }

        private void ThrowSddlErrorsIfInvalid(Sddl.Parser.Acm acm)
        {
            if (!acm.IsValid)
            {
                ThrowSddlErrors(acm.Errors);
            }
        }

        private void ThrowSddlErrors(System.Collections.Generic.IList<Sddl.Parser.Error> errors)
        {
            throw new FormatException(string.Join(", ", errors.Select(x => x.Description)));
        }
#endif
    }
}
