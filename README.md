# WinHttpUrlAclDotNet
A .NET wrapper for the Win32 HTTP Server URL ACL APIs found in `Http.h`. This library can be considered as a programmatic alternative to Windows command line tools `netsh http show|add|delete urlacl`.

This library wraps around native methods `HttpSetServiceConfiguration`, `HttpDeleteServiceConfiguration` and `HttpQueryServiceConfiguration`.

Get from [NuGet.org](https://www.nuget.org/packages/WinHttpUrlAclDotNet/1.0.0)

## Usage
```c#
try
{
    using (var api = new WinHttpServerApi.UrlAclManager())
    {
        api.AddUrl("http://+:1234/", WellKnownSidType.AuthenticatedUserSid, null, false);
    }
}
catch (Win32Exception ex) when (ex.NativeErrorCode == (int)WinHttpServerApi.Win32ErrorCode.ERROR_ACCESS_DENIED)
{
    Console.WriteLine("Must run as an elevated admin.");
}
catch (Win32Exception ex)
{
    ...
}
```

## Other Useful Libraries
This library only does the URL ACL. The below libraries may be used for the firewall and certificate.

### Windows Firewall
The [WindowsFirewallHelper](https://www.nuget.org/packages/WindowsFirewallHelper/) NuGet package can be used to add exceptions to the Windows firewall.
The below example shows how to use it to add a firewall rule for port 80. Because it is HTTP.SYS (not your application) that is actually listening the application is set to "System".

```C#
var profiles = FirewallProfiles.Domain | FirewallProfiles.Private | FirewallProfiles.Public;
var rule = FirewallManager.Instance.CreateApplicationRule(profiles, "Test rule for HTTP.SYS", FirewallAction.Allow, "System", FirewallProtocol.TCP);
rule.LocalPorts = new ushort[] { 80 };
FirewallManager.Instance.Rules.Add(rule);
```

### SSL Certificate Binding
The [SslCertBinding.Net](https://www.nuget.org/packages/SslCertBinding.Net/) NuGet package can be used to bind a certificate if you want to use HTTPS.
