using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Xml;
// A shim to enable some WIF3.5 classes, specifically WSSecurityTokenSerializer, to be passed back to WCF

namespace System.ServiceModel
{
  public static class CompatibilityShim
  {
    public static SecurityTokenSerializer Serializer
    {
        get; set;
    }
    
    // Make the WIF3.5 SimpleSecurityTokenProvider Constructor available as a delegate
    public static Func<SecurityToken, SecurityTokenRequirement, SecurityTokenProvider> SimpleSecurityTokenProviderConstructor
    {
        get; set;
    }
    
    public static SecurityTokenProvider SimpleSecurityTokenProvider(SecurityToken token, SecurityTokenRequirement tokenRequirement)
    {
        return SimpleSecurityTokenProviderConstructor(token, tokenRequirement);
    }
    
    // Make the WIF3.5 IssuedSecurityTokenProvider Constructor available as a delegate
    public static Func<SafeFreeCredentials, SecurityTokenProvider> IssuedSecurityTokenProviderConstructor
    {
        get; set;
    }
    
    public static SecurityTokenProvider IssuedSecurityTokenProvider(SafeFreeCredentials creds)
    {
        return IssuedSecurityTokenProviderConstructor(creds);
    }

    public static void Print(string message, params string[] list)
    {
        Console.WriteLine(message, list);
    }

    public static void PrintMessageBuffer(byte[] buffer)
    {
        string m1 = System.Text.Encoding.UTF8.GetString(buffer).Trim('^', '@');
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(m1);
        var stringWriter = new StringWriter(new StringBuilder());
        var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
        doc.Save(xmlTextWriter);
        Print("BufferedMessage {0}", stringWriter.ToString());
    }

    public const bool ShouldSignHeader = true;
  }
}
