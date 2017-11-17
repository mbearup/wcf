using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
// A shim to enable some WIF3.5 classes, specifically WSSecurityTokenSerializer, to be passed back to WCF

namespace System.ServiceModel
{
  public static class CompatibilityShim
  {
    private static bool debugMode = false;

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
    public static Func<SafeFreeCredentials, IssuedSecurityTokenProviderShim> IssuedSecurityTokenProviderConstructor
    {
        get; set;
    }
    
    public static IssuedSecurityTokenProviderShim IssuedSecurityTokenProvider(SafeFreeCredentials creds)
    {
        return IssuedSecurityTokenProviderConstructor(creds);
    }

    public static bool DebugMode
    {
        get
        {
            return debugMode;
        }
    }

    public static void Log(string message, params string[] list)
    {
        if (DebugMode)
        {
            Console.WriteLine(message, list);
        }
    }

    public static void LogMessageBuffer(byte[] buffer, int start, int size)
    {
        if (!DebugMode)
        {
            return;
        }
        
        byte[] messageBuffer = new byte[size];
        Array.Copy(buffer, start, messageBuffer, 0, size);
        string m1 = System.Text.Encoding.UTF8.GetString(messageBuffer);
        if (m1.StartsWith("<"))
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(m1);
                var stringWriter = new StringWriter(new StringBuilder());
                var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
                doc.Save(xmlTextWriter);
                m1 = stringWriter.ToString();
            }
            catch (Exception)
            {}
        }
        Log("BufferedMessage {0}", m1);
    }

    // This should be set elsewhere. 
    public const bool ShouldSignHeader = true;
    
    // Make the IssuerAddress and IssuerBinding available. 
    // These should be set by the in the IssuedTokenParameters by the ChannelFectory, but that doesn't happen...
    public static EndpointAddress IssuerAddress
    {
        get; set;
    }
    
    public static Binding IssuerBinding
    {
        get; set;
    }
  }
}
