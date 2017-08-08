// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.EncryptedKeyHashIdentifierClause
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Globalization;
using System.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Security
{
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  internal sealed class EncryptedKeyHashIdentifierClause : BinaryKeyIdentifierClause
  {
    public EncryptedKeyHashIdentifierClause(byte[] encryptedKeyHash)
      : this(encryptedKeyHash, true)
    {
    }

    internal EncryptedKeyHashIdentifierClause(byte[] encryptedKeyHash, bool cloneBuffer)
      : this(encryptedKeyHash, cloneBuffer, (byte[]) null, 0)
    {
    }

    internal EncryptedKeyHashIdentifierClause(byte[] encryptedKeyHash, bool cloneBuffer, byte[] derivationNonce, int derivationLength)
      : base((string) null, encryptedKeyHash, cloneBuffer, derivationNonce, derivationLength)
    {
    }

    public byte[] GetEncryptedKeyHash()
    {
      return this.GetBuffer();
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EncryptedKeyHashIdentifierClause(Hash = {0})", new object[1]{ (object) Convert.ToBase64String(this.GetRawBuffer()) });
    }
  }
}

