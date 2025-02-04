using CredentialManagement;
using System;
using System.Net;
using System.Runtime.InteropServices;

public class CredentialManager
{
    public static NetworkCredential GetCredential(string target)
    {
        var cred = new Credential { Target = target };
        return cred.Load() ? new NetworkCredential(cred.Username, cred.Password) : null;
    }
}

public class MailjetConfig
{
    public static string ApiKey => CredentialManager.GetCredential("SaliyaAutoCare/apiKey")?.Password;
    public static string ApiSecret => CredentialManager.GetCredential("SaliyaAutoCare/apiSecret")?.Password;
    public static string SenderEmail => CredentialManager.GetCredential("SaliyaAutoCare/Email")?.Password;
}
