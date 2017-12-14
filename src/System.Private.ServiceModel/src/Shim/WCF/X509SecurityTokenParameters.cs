// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.X509SecurityTokenParameters
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Text;

namespace System.ServiceModel.Security.Tokens
{
  /// <summary>Represents the parameters for an X.509 security token.</summary>
  public class X509SecurityTokenParameters : SecurityTokenParameters
  {
    internal const X509KeyIdentifierClauseType defaultX509ReferenceStyle = X509KeyIdentifierClauseType.Any;
    private X509KeyIdentifierClauseType x509ReferenceStyle;

    /// <summary>Gets a value that indicates whether the token has an asymmetric key.</summary>
    /// <returns>True if the token has an asymmetric key; otherwise, false. Always returns false.</returns>
    protected override internal bool HasAsymmetricKey
    {
      get
      {
        return true;
      }
    }

    /// <summary>Gets and sets the X.509 reference style.</summary>
    /// <returns>An <see cref="T:System.ServiceModel.Security.Tokens.X509KeyIdentifierClauseType" />.</returns>
    public X509KeyIdentifierClauseType X509ReferenceStyle
    {
      get
      {
        return this.x509ReferenceStyle;
      }
      set
      {
        X509SecurityTokenReferenceStyleHelper.Validate(value);
        this.x509ReferenceStyle = value;
      }
    }

    /// <summary>Gets a value that indicates whether the token supports client authentication.</summary>
    /// <returns>True if the token supports client authentication; otherwise, false. Always returns true.</returns>
    protected override internal bool SupportsClientAuthentication
    {
      get
      {
        return true;
      }
    }

    /// <summary>Gets a value that indicates whether the token supports server authentication.</summary>
    /// <returns>True if the token supports server authentication; otherwise, false. Always returns true.</returns>
    protected override internal bool SupportsServerAuthentication
    {
      get
      {
        return true;
      }
    }

    /// <summary>Gets a value that indicates whether the token supports a Windows identity for authentication.</summary>
    /// <returns>True if the token supports a Windows identity for authentication; otherwise, false. Always returns true.</returns>
    protected override internal bool SupportsClientWindowsIdentity
    {
      get
      {
        return true;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.X509SecurityTokenParameters" /> class.</summary>
    /// <param name="other">The other instance of this class. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="other" /> is null.</exception>
    protected X509SecurityTokenParameters(X509SecurityTokenParameters other)
      : base((SecurityTokenParameters) other)
    {
      this.x509ReferenceStyle = other.x509ReferenceStyle;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.X509SecurityTokenParameters" /> class.</summary>
    public X509SecurityTokenParameters()
      : this(X509KeyIdentifierClauseType.Any, SecurityTokenInclusionMode.AlwaysToRecipient)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.X509SecurityTokenParameters" /> class.</summary>
    /// <param name="x509ReferenceStyle">An <see cref="T:System.ServiceModel.Security.Tokens.X509KeyIdentifierClauseType" />.</param>
    public X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle)
      : this(x509ReferenceStyle, SecurityTokenInclusionMode.AlwaysToRecipient)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.X509SecurityTokenParameters" /> class.</summary>
    /// <param name="x509ReferenceStyle">An <see cref="T:System.ServiceModel.Security.Tokens.X509KeyIdentifierClauseType" />.</param>
    /// <param name="inclusionMode">A <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenInclusionMode" />.</param>
    public X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle, SecurityTokenInclusionMode inclusionMode)
      : this(x509ReferenceStyle, inclusionMode, true)
    {
    }

    internal X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle, SecurityTokenInclusionMode inclusionMode, bool requireDerivedKeys)
    {
      this.X509ReferenceStyle = x509ReferenceStyle;
      this.InclusionMode = inclusionMode;
      this.RequireDerivedKeys = requireDerivedKeys;
    }

    /// <summary>Clones another instance of this instance of the class.</summary>
    /// <returns>A new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenParameters" />.</returns>
#if FEATURE_CORECLR
    protected override SecurityTokenParameters CloneCore()
#else
    protected SecurityTokenParameters CloneCore()
#endif
    {
      return (SecurityTokenParameters) new X509SecurityTokenParameters(this);
    }

    /// <summary>Creates a key identifier clause for a token.</summary>
    /// <param name="token">The token.</param>
    /// <param name="referenceStyle">The <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenReferenceStyle" />.</param>
    /// <returns>The security key identifier clause.</returns>
    protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
    {
      SecurityKeyIdentifierClause identifierClause = (SecurityKeyIdentifierClause) null;
      switch (this.x509ReferenceStyle)
      {
        case X509KeyIdentifierClauseType.Thumbprint:
          identifierClause = this.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
          break;
#if FEATURE_CORECLR
        default:
          throw new NotImplementedException("X509KeyIdentifierClauseType " + this.x509ReferenceStyle + " is unsupported");
#else
        case X509KeyIdentifierClauseType.IssuerSerial:
          identifierClause = this.CreateKeyIdentifierClause<X509IssuerSerialKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
          break;
        case X509KeyIdentifierClauseType.SubjectKeyIdentifier:
          identifierClause = this.CreateKeyIdentifierClause<X509SubjectKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
          break;
        case X509KeyIdentifierClauseType.RawDataKeyIdentifier:
          identifierClause = this.CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
          break;
        default:
          if (referenceStyle == SecurityTokenReferenceStyle.External)
          {
            X509SecurityToken x509SecurityToken = token as X509SecurityToken;
            if (x509SecurityToken != null)
            {
              X509SubjectKeyIdentifierClause keyIdentifierClause;
              if (X509SubjectKeyIdentifierClause.TryCreateFrom(x509SecurityToken.Certificate, out keyIdentifierClause))
                identifierClause = (SecurityKeyIdentifierClause) keyIdentifierClause;
            }
            else
            {
              X509WindowsSecurityToken windowsSecurityToken = token as X509WindowsSecurityToken;
              X509SubjectKeyIdentifierClause keyIdentifierClause;
              if (windowsSecurityToken != null && X509SubjectKeyIdentifierClause.TryCreateFrom(windowsSecurityToken.Certificate, out keyIdentifierClause))
                identifierClause = (SecurityKeyIdentifierClause) keyIdentifierClause;
            }
            if (identifierClause == null)
              identifierClause = (SecurityKeyIdentifierClause) token.CreateKeyIdentifierClause<X509IssuerSerialKeyIdentifierClause>();
            if (identifierClause == null)
            {
              identifierClause = (SecurityKeyIdentifierClause) token.CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause>();
              break;
            }
            break;
          }
          identifierClause = (SecurityKeyIdentifierClause) token.CreateKeyIdentifierClause<LocalIdKeyIdentifierClause>();
          break;
#endif
      }
      return identifierClause;
    }

    /// <summary>Initializes a security token requirement.</summary>
    /// <param name="requirement">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenRequirement" />.</param>
    public override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
    {
      requirement.TokenType = SecurityTokenTypes.X509Certificate;
      requirement.RequireCryptographicToken = true;
      requirement.KeyType = SecurityKeyType.AsymmetricKey;
    }

    /// <summary>Displays a text representation of this instance of the class.</summary>
    /// <returns>A text representation of this instance of this class.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(base.ToString());
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X509ReferenceStyle: {0}", new object[1]
      {
        (object) this.x509ReferenceStyle.ToString()
      }));
      return stringBuilder.ToString();
    }
  }
}
