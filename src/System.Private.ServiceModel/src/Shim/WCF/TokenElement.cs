// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.TokenElement
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class TokenElement : ISecurityElement
  {
    private SecurityStandardsManager standardsManager;
    private SecurityToken token;

    public bool HasId
    {
      get
      {
        return true;
      }
    }

    public string Id
    {
      get
      {
        return this.token.Id;
      }
    }

    public SecurityToken Token
    {
      get
      {
        return this.token;
      }
    }

    public TokenElement(SecurityToken token, SecurityStandardsManager standardsManager)
    {
      this.token = token;
      this.standardsManager = standardsManager;
    }

    public override bool Equals(object item)
    {
      TokenElement tokenElement = item as TokenElement;
      if (tokenElement != null && this.token == tokenElement.token)
        return this.standardsManager == tokenElement.standardsManager;
      return false;
    }

    public override int GetHashCode()
    {
      return this.token.GetHashCode() ^ this.standardsManager.GetHashCode();
    }

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      this.standardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, this.token);
    }
  }
}
