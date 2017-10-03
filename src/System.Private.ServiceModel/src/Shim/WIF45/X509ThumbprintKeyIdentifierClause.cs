// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.Tokens.X509ThumbprintKeyIdentifierClause
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
  /// <summary>Represents a key identifier clause that identifies a <see cref="T:System.IdentityModel.Tokens.X509SecurityToken" /> security tokens using the X.509 certificate's thumbprint.</summary>
  public class X509ThumbprintKeyIdentifierClause : BinaryKeyIdentifierClause
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509ThumbprintKeyIdentifierClause" /> class using the specified X.509 certificate. </summary>
    /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="certificate" /> is null.</exception>
    public X509ThumbprintKeyIdentifierClause(X509Certificate2 certificate)
      : this(X509ThumbprintKeyIdentifierClause.GetHash(certificate), false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509ThumbprintKeyIdentifierClause" /> class using the specified thumbprint for an X.509 certificate. </summary>
    /// <param name="thumbprint">An array of <see cref="T:System.Byte" /> that contains the thumbprint of the X.509 certificate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="thumbprint" /> is null.-or-<paramref name="thumbprint" /> is zero length.</exception>
    public X509ThumbprintKeyIdentifierClause(byte[] thumbprint)
      : this(thumbprint, true)
    {
    }

    internal X509ThumbprintKeyIdentifierClause(byte[] thumbprint, bool cloneBuffer)
      : base((string) null, thumbprint, cloneBuffer)
    {
    }

    private static byte[] GetHash(X509Certificate2 certificate)
    {
      if (certificate == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
      return certificate.GetCertHash();
    }

    /// <summary>Returns the thumbprint for the X.509 certificate.</summary>
    /// <returns>An array of <see cref="T:System.Byte" /> that contains the thumbprint of the X.509 certificate.</returns>
    public byte[] GetX509Thumbprint()
    {
      return this.GetBuffer();
    }

    /// <summary>Returns a value that indicates whether the key identifier for this instance is equivalent to the specified X.509 certificate's thumbprint.</summary>
    /// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that contains the X.509 certificate to compare.</param>
    /// <returns>true if <paramref name="certificate" /> has the same thumbprint as the current instance; otherwise, false.</returns>
    public bool Matches(X509Certificate2 certificate)
    {
      if (certificate == null)
        return false;
      return this.Matches(X509ThumbprintKeyIdentifierClause.GetHash(certificate));
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A <see cref="T:System.String" /> that represents the current object.</returns>
    public override string ToString()
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("ToHexString is not supported in .NET Core");
#else
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X509ThumbprintKeyIdentifierClause(Hash = 0x{0})", new object[1]{ (object) this.ToHexString() });
#endif
    }
  }
}
