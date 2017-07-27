// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ISecureConversationSession
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel.Security
{
  /// <summary>Represents a secure conversation security session. The communicating parties secure all messages on the session using a SecurityContextToken that is issued by the server as part of session establishment.</summary>
  public interface ISecureConversationSession : ISecuritySession, ISession
  {
    /// <summary>Tries to write the SecurityKeyIdentifierClause corresponding to the security session's token. This method is useful for sessions like WS-RM sessions that build on top of the security session and refer to the security session token's identifier as part of their session establishment protocol.</summary>
    /// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter" /> with which to try to write the token.</param>
    void WriteSessionTokenIdentifier(XmlDictionaryWriter writer);

    /// <summary>Tries to read the session token identifier pointed to by the XML reader.</summary>
    /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> with which to try to read the token.</param>
    /// <returns>true if the XML pointed to by the XML reader corresponds to a SecurityKeyIdentifierClause that matches the security session's token; otherwise, false.</returns>
    bool TryReadSessionTokenIdentifier(XmlReader reader);
  }
}
