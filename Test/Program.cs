using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var api = new WinHttpServerApi.UrlAclManager())
            {
                /*
                 * The below should produce:
                 *   Add new...
                 *      URL: http://+:1234/
                 *     SDDL: D:(A;;GX;;;AU)
                 *   Overwrite...
                 *      URL: http://+:1234/
                 *     SDDL: D:(A;;GX;;;WD)
                 */

                const ushort port = 1234;
                string url = $"http://+:{port}/";

                var checkBefore = api.QueryUrls();
                if (checkBefore.Any(x => x.Url.Contains($":{port}/")))
                {
                    throw new Exception("An entry already exists with this port.");
                }

                Console.WriteLine("Add new...");
                api.AddUrl(url, System.Security.Principal.WellKnownSidType.AuthenticatedUserSid, null, false);
                try
                {
                    api.AddUrl(url, System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null, false);
                    throw new Exception("SHOULD NOT GET HERE! The above should have failed because it's a duplicate.");
                }
                catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == (int)WinHttpServerApi.Win32ErrorCode.ERROR_ALREADY_EXISTS)
                {
                }
                foreach (var entry in api.QueryUrls())
                {
                    if (entry.Url.Contains($":{port}/"))
                    {
                        Console.WriteLine("   URL: " + entry.Url);
                        Console.WriteLine("  SDDL: " + entry.Sddl);
                    }
                }

                Console.WriteLine("Overwrite...");
                api.AddUrl(url, System.Security.Principal.WellKnownSidType.WorldSid, null, true);
                foreach (var entry in api.QueryUrls())
                {
                    if (entry.Url.Contains($":{port}/"))
                    {
                        Console.WriteLine("   URL: " + entry.Url);
                        Console.WriteLine("  SDDL: " + entry.Sddl);
                    }
                }

                api.DeleteUrl(url);
            }
        }
    }
}
