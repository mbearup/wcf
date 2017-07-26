// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.SspiSecurityTokenParameters
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.Text;

namespace System.ServiceModel.Security.Tokens
{
  /// <summary>Represents the parameters for an SSPI security token that is obtained during the SOAP-level SPNEGO protocol with the server.</summary>
  public class SspiSecurityTokenParameters : SecurityTokenParameters
  {
    internal const bool defaultRequireCancellation = false;
    private bool requireCancellation;
    private BindingContext issuerBindingContext;

    /// <summary>Gets a value that indicates whether the token has an asymmetric key.</summary>
    /// <returns>Always returns false.</returns>
    protected internal override bool HasAsymmetricKey
    {
      get
      {
        return false;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the token requires cancellation.</summary>
    /// <returns>true if the token requires cancellation; otherwise, false. The default is false.</returns>
    public bool RequireCancellation
    {
      get
      {
        return this.requireCancellation;
      }
      set
      {
        this.requireCancellation = value;
      }
    }

    internal BindingContext IssuerBindingContext
    {
      get
      {
        return this.issuerBindingContext;
      }
      set
      {
        if (value != null)
          value = value.Clone();
        this.issuerBindingContext = value;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports client authentication.</summary>
    /// <returns>Always returns true.</returns>
    protected internal override bool SupportsClientAuthentication
    {
      get
      {
        return true;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports server authentication.</summary>
    /// <returns>Always returns true.</returns>
    protected internal override bool SupportsServerAuthentication
    {
      get
      {
        return true;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports a Windows identity for authentication.</summary>
    /// <returns>Always returns true.</returns>
    protected internal override bool SupportsClientWindowsIdentity
    {
      get
      {
        return true;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SspiSecurityTokenParameters" /> class.</summary>
    /// <param name="other">The other instance of this class. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="other" /> is null.</exception>
    protected SspiSecurityTokenParameters(SspiSecurityTokenParameters other)
      : base((SecurityTokenParameters) other)
    {
      this.requireCancellation = other.requireCancellation;
      if (other.issuerBindingContext == null)
        return;
      this.issuerBindingContext = other.issuerBindingContext.Clone();
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SspiSecurityTokenParameters" /> class.</summary>
    public SspiSecurityTokenParameters()
      : this(false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SspiSecurityTokenParameters" /> class.</summary>
    /// <param name="requireCancellation">Whether the token requires cancellation.</param>
    public SspiSecurityTokenParameters(bool requireCancellation)
    {
      this.requireCancellation = requireCancellation;
    }

    /// <summary>Clones another instance of this instance of the class.</summary>
    /// <returns>A new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenParameters" />.</returns>
    protected override SecurityTokenParameters CloneCore()
    {
      return (SecurityTokenParameters) new SspiSecurityTokenParameters(this);
    }

    /// <summary>Creates a key identifier clause for a token.</summary>
    /// <param name="token">The token.</param>
    /// <param name="referenceStyle">The <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenReferenceStyle" />.</param>
    /// <returns>A security key identifier clause.</returns>
#if FEATURE_CORECLR
    protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
#else
    protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
#endif
    {
      if (token is GenericXmlSecurityToken)
        return this.CreateGenericXmlTokenKeyIdentifierClause(token, referenceStyle);
#if FEATURE_CORECLR
      throw new NotImplementedException("Generic CreateKeyIdentifierClause function is not implemented in .NET Core");
#else
      return this.CreateKeyIdentifierClause<SecurityContextKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
#endif
    }

    /// <summary>Initializes a security token requirement.</summary>
    /// <param name="requirement">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenRequirement" />.</param>
#if FEATURE_CORECLR
    protected override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#else
    protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#endif
    {
      requirement.TokenType = ServiceModelSecurityTokenTypes.Spnego;
      requirement.RequireCryptographicToken = true;
      requirement.KeyType = SecurityKeyType.SymmetricKey;
      requirement.Properties[ServiceModelSecurityTokenRequirement.SupportSecurityContextCancellationProperty] = (object) this.RequireCancellation;
      if (this.IssuerBindingContext != null)
        requirement.Properties[ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty] = (object) this.IssuerBindingContext.Clone();
      requirement.Properties[ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty] = (object) this.Clone();
    }

    /// <summary>Displays a text representation of this instance of the class.</summary>
    /// <returns>A text representation of this instance of this class.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(base.ToString());
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RequireCancellation: {0}", new object[1]
      {
        (object) this.RequireCancellation.ToString()
      }));
      return stringBuilder.ToString();
    }
  }
}
