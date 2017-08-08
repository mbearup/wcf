// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ISspiNegotiation
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Security
{
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  public interface ISspiNegotiation : IDisposable
  {
    DateTime ExpirationTimeUtc { get; }

    bool IsCompleted { get; }

    bool IsValidContext { get; }

    string KeyEncryptionAlgorithm { get; }

    byte[] Decrypt(byte[] encryptedData);

    byte[] Encrypt(byte[] data);

    byte[] GetOutgoingBlob(byte[] incomingBlob, ChannelBinding channelbinding, ExtendedProtectionPolicy protectionPolicy);

    string GetRemoteIdentityName();
  }
}
