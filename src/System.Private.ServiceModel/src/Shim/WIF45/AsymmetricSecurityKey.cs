// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.Tokens.AsymmetricSecurityKey
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Security.Cryptography;

namespace System.IdentityModel.Tokens
{
  /// <summary>Base class for asymmetric keys.</summary>
  public abstract class AsymmetricSecurityKey : SecurityKey
  {
    /// <summary>When overridden in a derived class, gets the specified asymmetric cryptographic algorithm. </summary>
    /// <param name="algorithm">The asymmetric algorithm to create.</param>
    /// <param name="privateKey">true when a private key is required to create the algorithm; otherwise, false. </param>
    /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> that represents the specified asymmetric cryptographic algorithm.Typically, true is passed into the <paramref name="privateKey" /> parameter, as a private key is typically required for decryption.</returns>
    public abstract AsymmetricAlgorithm GetAsymmetricAlgorithm(string algorithm, bool privateKey);

    /// <summary>When overridden in a derived class, gets a cryptographic algorithm that generates a hash for a digital signature.</summary>
    /// <param name="algorithm">The hash algorithm.</param>
    /// <returns>A <see cref="T:System.Security.Cryptography.HashAlgorithm" /> that generates hashes for digital signatures.</returns>
    public abstract HashAlgorithm GetHashAlgorithmForSignature(string algorithm);

    /// <summary>When overridden in a derived class, gets the deformatter algorithm for the digital signature.</summary>
    /// <param name="algorithm">The deformatter algorithm for the digital signature.</param>
    /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureDeformatter" /> that represents the deformatter algorithm for the digital signature.</returns>
    public abstract AsymmetricSignatureDeformatter GetSignatureDeformatter(string algorithm);

    /// <summary>When overridden in a derived class, gets the formatter algorithm for the digital signature.</summary>
    /// <param name="algorithm">The formatter algorithm for the digital signature.</param>
    /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureFormatter" /> that represents the formatter algorithm for the digital signature.</returns>
    public abstract AsymmetricSignatureFormatter GetSignatureFormatter(string algorithm);

    /// <summary>When overridden in a derived class, gets a value that indicates whether the private key is available.</summary>
    /// <returns>true when the private key is available; otherwise, false. </returns>
    public abstract bool HasPrivateKey();
  }
}
