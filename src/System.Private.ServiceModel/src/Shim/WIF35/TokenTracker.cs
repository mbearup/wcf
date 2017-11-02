// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.TokenTracker
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security
{
  internal class TokenTracker
  {
    public SecurityToken token;
    public bool IsDerivedFrom;
    public bool IsSigned;
    public bool IsEncrypted;
    public bool IsEndorsing;
    public bool AlreadyReadEndorsingSignature;
    private bool allowFirstTokenMismatch;
    public SupportingTokenAuthenticatorSpecification spec;

    public TokenTracker(SupportingTokenAuthenticatorSpecification spec)
      : this(spec, (SecurityToken) null, false)
    {
    }

    public TokenTracker(SupportingTokenAuthenticatorSpecification spec, SecurityToken token, bool allowFirstTokenMismatch)
    {
      this.spec = spec;
      this.token = token;
      this.allowFirstTokenMismatch = allowFirstTokenMismatch;
    }

    public void RecordToken(SecurityToken token)
    {
      if (this.token == null)
        this.token = token;
      else if (this.allowFirstTokenMismatch)
      {
        if (!TokenTracker.AreTokensEqual(this.token, token))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MismatchInSecurityOperationToken")));
        this.token = token;
        this.allowFirstTokenMismatch = false;
      }
      else if (this.token != token)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MismatchInSecurityOperationToken")));
    }

    private static bool AreTokensEqual(SecurityToken outOfBandToken, SecurityToken replyToken)
    {
      if (outOfBandToken is X509SecurityToken && replyToken is X509SecurityToken)
	  {
#if FEATURE_CORECLR
		throw new NotImplementedException("CryptoHelper is not supported in .NET Core");
#else
        return CryptoHelper.IsEqual(((X509SecurityToken) outOfBandToken).Certificate.GetCertHash(), ((X509SecurityToken) replyToken).Certificate.GetCertHash());
#endif
	  }
      return false;
    }
  }
}
