// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.SslSecurityTokenParameters
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
  /// <summary>Represents the parameters for an SSL security token that is obtained when doing the SOAP-level SSL protocol with the server.</summary>
  public class SslSecurityTokenParameters : SecurityTokenParameters
  {
    internal const bool defaultRequireClientCertificate = false;
    internal const bool defaultRequireCancellation = false;
    private bool requireCancellation;
    private bool requireClientCertificate;
    private BindingContext issuerBindingContext;

    /// <summary>Gets a value that indicates whether the token has an asymmetric key.</summary>
    /// <returns>true if the token has an asymmetric key; otherwise, false.</returns>
    protected internal override bool HasAsymmetricKey
    {
      get
      {
        return false;
      }
    }

    /// <summary>Gets or sets a value that indicates whether cancellation is required.</summary>
    /// <returns>true if cancellation is required; otherwise, false. The default is false.</returns>
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

    /// <summary>Gets or sets a value that indicates whether a client certificate is required.</summary>
    /// <returns>true if a client certificate is required; otherwise, false. The default is false.</returns>
    public bool RequireClientCertificate
    {
      get
      {
        return this.requireClientCertificate;
      }
      set
      {
        this.requireClientCertificate = value;
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
    /// <returns>true if the token supports client authentication; otherwise, false.</returns>
    protected internal override bool SupportsClientAuthentication
    {
      get
      {
        return this.requireClientCertificate;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports server authentication.</summary>
    /// <returns>true if the token supports server authentication; otherwise, false.</returns>
    protected internal override bool SupportsServerAuthentication
    {
      get
      {
        return true;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports a Windows identity for authentication.</summary>
    /// <returns>true if the token supports a Windows identity for authentication; otherwise, false.</returns>
    protected internal override bool SupportsClientWindowsIdentity
    {
      get
      {
        return this.requireClientCertificate;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SslSecurityTokenParameters" /> class from another instance.</summary>
    /// <param name="other">The other instance of this class.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="other" /> is null.</exception>
    protected SslSecurityTokenParameters(SslSecurityTokenParameters other)
      : base((SecurityTokenParameters) other)
    {
      this.requireClientCertificate = other.requireClientCertificate;
      this.requireCancellation = other.requireCancellation;
      if (other.issuerBindingContext == null)
        return;
      this.issuerBindingContext = other.issuerBindingContext.Clone();
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SslSecurityTokenParameters" /> class.</summary>
    public SslSecurityTokenParameters()
      : this(false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SslSecurityTokenParameters" /> class.</summary>
    /// <param name="requireClientCertificate">true to require client certificate; otherwise, false.</param>
    public SslSecurityTokenParameters(bool requireClientCertificate)
      : this(requireClientCertificate, false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.SslSecurityTokenParameters" /> class.</summary>
    /// <param name="requireClientCertificate">true to require client certificate; otherwise, false.</param>
    /// <param name="requireCancellation">true to require cancellation; otherwise, false.</param>
    public SslSecurityTokenParameters(bool requireClientCertificate, bool requireCancellation)
    {
      this.requireClientCertificate = requireClientCertificate;
      this.requireCancellation = requireCancellation;
    }

    /// <summary>Clones another instance of this instance of the class.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenParameters" /> instance that represents the copy.</returns>
    protected override SecurityTokenParameters CloneCore()
    {
      return (SecurityTokenParameters) new SslSecurityTokenParameters(this);
    }

    /// <summary>Creates a key identifier clause for a token.</summary>
    /// <param name="token">The token.</param>
    /// <param name="referenceStyle">The reference style of the security token.</param>
    /// <returns>The key identifier clause for a token.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="token" /> is null.</exception>
    /// <exception cref="T:System.NotSupportedException">
    /// <paramref name="referenceStyle" /> is not External or Internal.</exception>
#if FEATURE_CORECLR
    protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
#else
    protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
#endif
    {
      if (token is GenericXmlSecurityToken)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SslSecurityTokenParameters.CreateGenericXmlTokenKeyIdentifierClause is not supported in .NET Core");
#else
        return this.CreateGenericXmlTokenKeyIdentifierClause(token, referenceStyle);
#endif
      }
#if FEATURE_CORECLR
      throw new NotImplementedException("CreateKeyIdentifierClause<> is not supported in .NET Core");
#else
      return this.CreateKeyIdentifierClause<SecurityContextKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
#endif
    }

    /// <summary>Initializes a security token requirement.</summary>
    /// <param name="requirement">The requirement of the security token.</param>
#if FEATURE_CORECLR
    public override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#else
    protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#endif
    {
      requirement.TokenType = this.RequireClientCertificate ? ServiceModelSecurityTokenTypes.MutualSslnego : ServiceModelSecurityTokenTypes.AnonymousSslnego;
      requirement.RequireCryptographicToken = true;
      requirement.KeyType = SecurityKeyType.SymmetricKey;
      requirement.Properties[ServiceModelSecurityTokenRequirement.SupportSecurityContextCancellationProperty] = (object) this.RequireCancellation;
      if (this.IssuerBindingContext != null)
        requirement.Properties[ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty] = (object) this.IssuerBindingContext.Clone();
      requirement.Properties[ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty] = (object) this.Clone();
    }

    /// <summary>Displays a text representation of this instance of the class.</summary>
    /// <returns>A text representation of this instance of the class.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(base.ToString());
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RequireCancellation: {0}", new object[1]
      {
        (object) this.RequireCancellation.ToString()
      }));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RequireClientCertificate: {0}", new object[1]
      {
        (object) this.RequireClientCertificate.ToString()
      }));
      return stringBuilder.ToString();
    }
  }
}
