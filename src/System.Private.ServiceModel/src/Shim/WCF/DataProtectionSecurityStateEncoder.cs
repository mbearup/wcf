// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.DataProtectionSecurityStateEncoder
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;
using System.Security.Cryptography;
using System.Text;

namespace System.ServiceModel.Security
{
  /// <summary>Provides encoding and decoding mechanisms for the security state using the Windows DataProtection API functionality. </summary>
  public class DataProtectionSecurityStateEncoder : SecurityStateEncoder
  {
    private byte[] entropy;
    private bool useCurrentUserProtectionScope;

    /// <summary>Gets a value that indicates whether to use the current user protection scope. </summary>
    /// <returns>true if the current user protection scope will be used; otherwise, false. </returns>
    public bool UseCurrentUserProtectionScope
    {
      get
      {
        return this.useCurrentUserProtectionScope;
      }
    }

    /// <summary>Initializes a new instance of this class. </summary>
    public DataProtectionSecurityStateEncoder()
      : this(true)
    {
    }

    /// <summary>Initializes a new instance of this class.  </summary>
    /// <param name="useCurrentUserProtectionScope">Indicates whether to use the current user protection scope.</param>
    public DataProtectionSecurityStateEncoder(bool useCurrentUserProtectionScope)
      : this(useCurrentUserProtectionScope, (byte[]) null)
    {
    }

    /// <summary>Initializes a new instance of this class.  </summary>
    /// <param name="useCurrentUserProtectionScope">Indicates whether to use the current user protection scope.</param>
    /// <param name="entropy">A byte array that specifies the entropy, which indicates additional randomness that the encoder could use to encode the security state.</param>
    public DataProtectionSecurityStateEncoder(bool useCurrentUserProtectionScope, byte[] entropy)
    {
      this.useCurrentUserProtectionScope = useCurrentUserProtectionScope;
      if (entropy == null)
      {
        this.entropy = (byte[]) null;
      }
      else
      {
        this.entropy = Fx.AllocateByteArray(entropy.Length);
        Buffer.BlockCopy((Array) entropy, 0, (Array) this.entropy, 0, entropy.Length);
      }
    }

    /// <summary>Indicates the randomness of this encoder.</summary>
    /// <returns>An array of type <see cref="T:System.Byte" />.</returns>
    public byte[] GetEntropy()
    {
      byte[] numArray = (byte[]) null;
      if (this.entropy != null)
      {
        numArray = Fx.AllocateByteArray(this.entropy.Length);
        Buffer.BlockCopy((Array) this.entropy, 0, (Array) numArray, 0, this.entropy.Length);
      }
      return numArray;
    }

    /// <summary>Returns a string that represents the current <see cref="T:System.ServiceModel.Security.DataProtectionSecurityStateEncoder" /> instance.</summary>
    /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:System.ServiceModel.Security.DataProtectionSecurityStateEncoder" /> instance.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.GetType().ToString());
      stringBuilder.AppendFormat("{0}  UseCurrentUserProtectionScope={1}", (object) Environment.NewLine, (object) this.useCurrentUserProtectionScope);
      stringBuilder.AppendFormat("{0}  Entropy Length={1}", (object) Environment.NewLine, (object) (this.entropy == null ? 0 : this.entropy.Length));
      return stringBuilder.ToString();
    }

    /// <summary>Decodes the security state.</summary>
    /// <param name="data">A byte array that represents the encoded security state.</param>
    /// <returns>A byte array that represents the decoded security state.</returns>
    protected internal override byte[] DecodeSecurityState(byte[] data)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("ProtectedData and DataProtectionScope are not implemented in .NET Core");
#else
      try
      {
        return ProtectedData.Unprotect(data, this.entropy, this.useCurrentUserProtectionScope ? DataProtectionScope.CurrentUser : DataProtectionScope.LocalMachine);
      }
      catch (CryptographicException ex)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("SecurityStateEncoderDecodingFailure"), (Exception) ex));
      }
#endif
    }

    /// <summary>Encodes the security state.</summary>
    /// <param name="data">A byte array representing the decoded security state.</param>
    /// <returns>A byte array that represents the encoded security state.</returns>
    protected internal override byte[] EncodeSecurityState(byte[] data)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("ProtectedData and DataProtectionScope are not implemented in .NET Core");
#else
      try
      {
        return ProtectedData.Protect(data, this.entropy, this.useCurrentUserProtectionScope ? DataProtectionScope.CurrentUser : DataProtectionScope.LocalMachine);
      }
      catch (CryptographicException ex)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("SecurityStateEncoderEncodingFailure"), (Exception) ex));
      }
#endif
    }
  }
}
