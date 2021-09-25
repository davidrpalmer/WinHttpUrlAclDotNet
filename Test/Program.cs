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
                        Add new...
                           URL: http://+:1234/
                           SDDL: D:(A;;GX;;;AU)
                            SID: AU
                              Name: NT AUTHORITY\Authenticated Users
                              Listen: True
                              Delegate: False
                        Overwrite...
                           URL: http://+:1234/
                           SDDL: D:(A;;GX;;;WD)
                            SID: WD
                              Name: Everyone
                              Listen: True
                              Delegate: False
                        Overwrite (multiple accounts)...
                           URL: http://+:1234/
                           SDDL: D:(A;;GX;;;WD)(A;;GW;;;BU)
                            SID: WD
                              Name: Everyone
                              Listen: True
                              Delegate: False
                            SID: BU
                              Name: BUILTIN\Users
                              Listen: False
                              Delegate: True
                 */

                const ushort port = 1234;
                string url = $"http://+:{port}/";
                var permissions = new WinHttpServerApi.UrlPermissions();

                var checkBefore = api.QueryUrls();
                if (checkBefore.Any(x => x.Url.Contains($":{port}/")))
                {
                    throw new Exception("An entry already exists with this port.");
                }

                Console.WriteLine("Add new...");
                permissions.SetSid(System.Security.Principal.WellKnownSidType.AuthenticatedUserSid);
                api.AddUrl(url, permissions, false);
                permissions.SetSid(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid);
                try
                {
                    api.AddUrl(url, permissions, false);
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
                        Console.WriteLine("   SDDL: " + entry.Sddl);

                        var decodedPermissions = entry.GetPermissions();
                        foreach (var decodedPermission in decodedPermissions)
                        {
                            Console.WriteLine("    SID: " + decodedPermission.Sid);
                            if (!decodedPermission.GetSidObject().IsWellKnown(System.Security.Principal.WellKnownSidType.AuthenticatedUserSid))
                            {
                                Console.WriteLine("      Error, SID is not the expected SID!");
                            }
                            Console.WriteLine("      Name: " + decodedPermission.GetSidName());
                            Console.WriteLine("      Listen: " + decodedPermission.Listen);
                            Console.WriteLine("      Delegate: " + decodedPermission.Delegate);
                        }
                    }
                }

                Console.WriteLine("Overwrite...");
                permissions.SetSid(System.Security.Principal.WellKnownSidType.WorldSid);
                api.AddUrl(url, permissions, true);
                foreach (var entry in api.QueryUrls())
                {
                    if (entry.Url.Contains($":{port}/"))
                    {
                        Console.WriteLine("   URL: " + entry.Url);
                        Console.WriteLine("   SDDL: " + entry.Sddl);

                        var decodedPermissions = entry.GetPermissions();
                        foreach (var decodedPermission in decodedPermissions)
                        {
                            Console.WriteLine("    SID: " + decodedPermission.Sid);
                            if (!decodedPermission.GetSidObject().IsWellKnown(System.Security.Principal.WellKnownSidType.WorldSid))
                            {
                                Console.WriteLine("      Error, SID is not the expected SID!");
                            }
                            Console.WriteLine("      Name: " + decodedPermission.GetSidName());
                            Console.WriteLine("      Listen: " + decodedPermission.Listen);
                            Console.WriteLine("      Delegate: " + decodedPermission.Delegate);
                        }
                    }
                }

                Console.WriteLine("Overwrite (multiple accounts)...");
                var permissionsList = new WinHttpServerApi.UrlPermissions[]
                {
                    permissions,
                    new WinHttpServerApi.UrlPermissions()
                    {
                         Delegate = true,
                         Listen = false
                    }
                };
                permissionsList[1].SetSid(System.Security.Principal.WellKnownSidType.BuiltinUsersSid);
                api.AddUrl(url, permissionsList, true);
                foreach (var entry in api.QueryUrls())
                {
                    if (entry.Url.Contains($":{port}/"))
                    {
                        Console.WriteLine("   URL: " + entry.Url);
                        Console.WriteLine("   SDDL: " + entry.Sddl);

                        var decodedPermissions = entry.GetPermissions();
                        foreach (var decodedPermission in decodedPermissions)
                        {
                            Console.WriteLine("    SID: " + decodedPermission.Sid);
                            Console.WriteLine("      Name: " + decodedPermission.GetSidName());
                            Console.WriteLine("      Listen: " + decodedPermission.Listen);
                            Console.WriteLine("      Delegate: " + decodedPermission.Delegate);
                        }
                    }
                }

                api.DeleteUrl(url);
            }
        }
    }
}
