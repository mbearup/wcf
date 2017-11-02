// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecurityHeaderTokenResolver
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Globalization;
using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
  internal sealed class SecurityHeaderTokenResolver : SecurityTokenResolver, IWrappedTokenKeyResolver
  {
    private const int InitialTokenArraySize = 10;
    private int tokenCount;
    private SecurityHeaderTokenResolver.SecurityTokenEntry[] tokens;
    private SecurityToken expectedWrapper;
    private SecurityTokenParameters expectedWrapperTokenParameters;
    private ReceiveSecurityHeader securityHeader;

    public SecurityToken ExpectedWrapper
    {
      get
      {
        return this.expectedWrapper;
      }
      set
      {
        this.expectedWrapper = value;
      }
    }

    public SecurityTokenParameters ExpectedWrapperTokenParameters
    {
      get
      {
        return this.expectedWrapperTokenParameters;
      }
      set
      {
        this.expectedWrapperTokenParameters = value;
      }
    }

    public SecurityHeaderTokenResolver()
      : this((ReceiveSecurityHeader) null)
    {
    }

    public SecurityHeaderTokenResolver(ReceiveSecurityHeader securityHeader)
    {
      this.tokens = new SecurityHeaderTokenResolver.SecurityTokenEntry[10];
      this.securityHeader = securityHeader;
    }

    public void Add(SecurityToken token)
    {
      this.Add(token, SecurityTokenReferenceStyle.Internal, (SecurityTokenParameters) null);
    }

    public void Add(SecurityToken token, SecurityTokenReferenceStyle allowedReferenceStyle, SecurityTokenParameters tokenParameters)
    {
      if (token == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
      if (allowedReferenceStyle == SecurityTokenReferenceStyle.External && tokenParameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("ResolvingExternalTokensRequireSecurityTokenParameters"));
      this.EnsureCapacityToAddToken();
      SecurityHeaderTokenResolver.SecurityTokenEntry[] tokens = this.tokens;
      int tokenCount = this.tokenCount;
      this.tokenCount = tokenCount + 1;
      int index = tokenCount;
      SecurityHeaderTokenResolver.SecurityTokenEntry securityTokenEntry = new SecurityHeaderTokenResolver.SecurityTokenEntry(token, tokenParameters, allowedReferenceStyle);
      tokens[index] = securityTokenEntry;
    }

    private void EnsureCapacityToAddToken()
    {
      if (this.tokenCount != this.tokens.Length)
        return;
      SecurityHeaderTokenResolver.SecurityTokenEntry[] securityTokenEntryArray = new SecurityHeaderTokenResolver.SecurityTokenEntry[this.tokens.Length * 2];
      Array.Copy((Array) this.tokens, 0, (Array) securityTokenEntryArray, 0, this.tokenCount);
      this.tokens = securityTokenEntryArray;
    }

    public bool CheckExternalWrapperMatch(SecurityKeyIdentifier keyIdentifier)
    {
      if (this.expectedWrapper == null || this.expectedWrapperTokenParameters == null)
        return false;
#if FEATURE_CORECLR
      // TokenParameters.MatchesKeyIdentifierClause not supported
#else
      for (int index = 0; index < keyIdentifier.Count; ++index)
      {
        if (this.expectedWrapperTokenParameters.MatchesKeyIdentifierClause(this.expectedWrapper, keyIdentifier[index], SecurityTokenReferenceStyle.External))
          return true;
      }
#endif
      return false;
    }

    internal SecurityToken ResolveToken(SecurityKeyIdentifier keyIdentifier, bool matchOnlyExternalTokens, bool resolveIntrinsicKeyClause)
    {
      if (keyIdentifier == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");
      for (int index = 0; index < keyIdentifier.Count; ++index)
      {
        SecurityToken securityToken = this.ResolveToken(keyIdentifier[index], matchOnlyExternalTokens, resolveIntrinsicKeyClause);
        if (securityToken != null)
          return securityToken;
      }
      return (SecurityToken) null;
    }

    private SecurityKey ResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, bool createIntrinsicKeys)
    {
      if (keyIdentifierClause == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("keyIdentifierClause"));
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityUtils.TryCreateKeyFromIntrinsicKeyClause is not supported in .NET Core");
#else
      SecurityKey key;
      for (int index = 0; index < this.tokenCount; ++index)
      {
        key = this.tokens[index].Token.ResolveKeyIdentifierClause(keyIdentifierClause);
        if (key != null)
          return key;
      }
      if (createIntrinsicKeys && SecurityUtils.TryCreateKeyFromIntrinsicKeyClause(keyIdentifierClause, (SecurityTokenResolver) this, out key))
        return key;
      return (SecurityKey) null;
#endif
    }

    private bool MatchDirectReference(SecurityToken token, SecurityKeyIdentifierClause keyClause)
    {
      LocalIdKeyIdentifierClause identifierClause = keyClause as LocalIdKeyIdentifierClause;
      if (identifierClause == null)
        return false;
      return token.MatchesKeyIdentifierClause((SecurityKeyIdentifierClause) identifierClause);
    }

    internal SecurityToken ResolveToken(SecurityKeyIdentifierClause keyIdentifierClause, bool matchOnlyExternal, bool resolveIntrinsicKeyClause)
    {
      if (keyIdentifierClause == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
      SecurityToken securityToken = (SecurityToken) null;
      for (int index = 0; index < this.tokenCount; ++index)
      {
        if (!matchOnlyExternal || this.tokens[index].AllowedReferenceStyle == SecurityTokenReferenceStyle.External)
        {
          SecurityToken token = this.tokens[index].Token;
#if FEATURE_CORECLR
          // TokenParameters.MatchesKeyIdentifierClause not supported
#else
          if (this.tokens[index].TokenParameters != null && this.tokens[index].TokenParameters.MatchesKeyIdentifierClause(token, keyIdentifierClause, this.tokens[index].AllowedReferenceStyle))
          {
            securityToken = token;
            break;
          }
#endif
          if (this.tokens[index].TokenParameters == null && this.tokens[index].AllowedReferenceStyle == SecurityTokenReferenceStyle.Internal && this.MatchDirectReference(token, keyIdentifierClause))
          {
            securityToken = token;
            break;
          }
        }
      }
      if (securityToken == null && keyIdentifierClause is EncryptedKeyIdentifierClause)
      {
        EncryptedKeyIdentifierClause keyClause = (EncryptedKeyIdentifierClause) keyIdentifierClause;
        SecurityKeyIdentifier encryptingKeyIdentifier = keyClause.EncryptingKeyIdentifier;
        SecurityToken unwrappingToken = this.expectedWrapper == null || !this.CheckExternalWrapperMatch(encryptingKeyIdentifier) ? this.ResolveToken(encryptingKeyIdentifier, true, resolveIntrinsicKeyClause) : this.expectedWrapper;
        if (unwrappingToken != null)
		{
#if FEATURE_CORECLR
          throw new NotImplementedException("SecurityUtils.CreateTokenFromEncryptedKeyClause is not supported in .NET Core");
#else
          securityToken = (SecurityToken) SecurityUtils.CreateTokenFromEncryptedKeyClause(keyClause, unwrappingToken);
#endif
		}
      }
#if FEATURE_CORECLR
	  throw new NotImplementedException("X509RawDataKeyIdentifierClause is not supported in .NET Core");
#else
      if (((securityToken != null || !(keyIdentifierClause is X509RawDataKeyIdentifierClause) ? 0 : (!matchOnlyExternal ? 1 : 0)) & (resolveIntrinsicKeyClause ? 1 : 0)) != 0)
        securityToken = (SecurityToken) new X509SecurityToken(new X509Certificate2(((X509RawDataKeyIdentifierClause) keyIdentifierClause).GetX509RawData()));
      byte[] derivationNonce = keyIdentifierClause.GetDerivationNonce();
      if (securityToken != null && derivationNonce != null)
      {
        if (SecurityUtils.GetSecurityKey<SymmetricSecurityKey>(securityToken) == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToDeriveKeyFromKeyInfoClause", (object) keyIdentifierClause, (object) securityToken)));
        int num = keyIdentifierClause.DerivationLength == 0 ? 32 : keyIdentifierClause.DerivationLength;
        if (num > this.securityHeader.MaxDerivedKeyLength)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("DerivedKeyLengthSpecifiedInImplicitDerivedKeyClauseTooLong", (object) keyIdentifierClause.ToString(), (object) num, (object) this.securityHeader.MaxDerivedKeyLength)));
        bool flag = false;
        for (int index = 0; index < this.tokenCount; ++index)
        {
          DerivedKeySecurityToken token = this.tokens[index].Token as DerivedKeySecurityToken;
          if (token != null && token.Length == num && (CryptoHelper.IsEqual(token.Nonce, derivationNonce) && token.TokenToDerive.MatchesKeyIdentifierClause(keyIdentifierClause)))
          {
            securityToken = this.tokens[index].Token;
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(this.securityHeader.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
          securityToken = (SecurityToken) new DerivedKeySecurityToken(-1, 0, num, (string) null, derivationNonce, securityToken, keyIdentifierClause, derivationAlgorithm, SecurityUtils.GenerateId());
          ((DerivedKeySecurityToken) securityToken).InitializeDerivedKey(num);
          this.Add(securityToken, SecurityTokenReferenceStyle.Internal, (SecurityTokenParameters) null);
          this.securityHeader.EnsureDerivedKeyLimitNotReached();
        }
      }
      return securityToken;
#endif
    }

    public override string ToString()
    {
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        stringWriter.WriteLine("SecurityTokenResolver");
        stringWriter.WriteLine("    (");
        stringWriter.WriteLine("    TokenCount = {0},", (object) this.tokenCount);
        for (int index = 0; index < this.tokenCount; ++index)
          stringWriter.WriteLine("    TokenEntry[{0}] = (AllowedReferenceStyle={1}, Token={2}, Parameters={3})", (object) index, (object) this.tokens[index].AllowedReferenceStyle, (object) this.tokens[index].Token.GetType(), (object) this.tokens[index].TokenParameters);
        stringWriter.WriteLine("    )");
        return stringWriter.ToString();
      }
    }

    protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
    {
      token = this.ResolveToken(keyIdentifier, false, true);
      return token != null;
    }

    internal bool TryResolveToken(SecurityKeyIdentifier keyIdentifier, bool matchOnlyExternalTokens, bool resolveIntrinsicKeyClause, out SecurityToken token)
    {
      token = this.ResolveToken(keyIdentifier, matchOnlyExternalTokens, resolveIntrinsicKeyClause);
      return token != null;
    }

    protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
    {
      token = this.ResolveToken(keyIdentifierClause, false, true);
      return token != null;
    }

    internal bool TryResolveToken(SecurityKeyIdentifierClause keyIdentifierClause, bool matchOnlyExternalTokens, bool resolveIntrinsicKeyClause, out SecurityToken token)
    {
      token = this.ResolveToken(keyIdentifierClause, matchOnlyExternalTokens, resolveIntrinsicKeyClause);
      return token != null;
    }

    internal bool TryResolveSecurityKey(SecurityKeyIdentifierClause keyIdentifierClause, bool createIntrinsicKeys, out SecurityKey key)
    {
      if (keyIdentifierClause == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
      key = this.ResolveSecurityKeyCore(keyIdentifierClause, createIntrinsicKeys);
      return key != null;
    }

    protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
    {
      key = this.ResolveSecurityKeyCore(keyIdentifierClause, true);
      return key != null;
    }

    private struct SecurityTokenEntry
    {
      private SecurityTokenParameters tokenParameters;
      private SecurityToken token;
      private SecurityTokenReferenceStyle allowedReferenceStyle;

      public SecurityToken Token
      {
        get
        {
          return this.token;
        }
      }

      public SecurityTokenParameters TokenParameters
      {
        get
        {
          return this.tokenParameters;
        }
      }

      public SecurityTokenReferenceStyle AllowedReferenceStyle
      {
        get
        {
          return this.allowedReferenceStyle;
        }
      }

      public SecurityTokenEntry(SecurityToken token, SecurityTokenParameters tokenParameters, SecurityTokenReferenceStyle allowedReferenceStyle)
      {
        this.token = token;
        this.tokenParameters = tokenParameters;
        this.allowedReferenceStyle = allowedReferenceStyle;
      }
    }
  }
}
