// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Protocols.WSTrust.FederatedClientCredentialsParameters
// Assembly: Microsoft.IdentityModel, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: F0B01310-0125-4EEF-8CFA-D5C3FE813EB2
// Assembly location: C:\WINDOWS\assembly\GAC_MSIL\Microsoft.IdentityModel\3.5.0.0__31bf3856ad364e35\Microsoft.IdentityModel.dll

using System.IdentityModel.Tokens;

namespace Microsoft.IdentityModel.Protocols.WSTrust
{
  public class FederatedClientCredentialsParameters
  {
    private SecurityToken _actAs;
    private SecurityToken _onBehalfOf;
    private SecurityToken _issuedSecurityToken;

    public SecurityToken ActAs
    {
      get
      {
        return this._actAs;
      }
      set
      {
        this._actAs = value;
      }
    }

    public SecurityToken OnBehalfOf
    {
      get
      {
        return this._onBehalfOf;
      }
      set
      {
        this._onBehalfOf = value;
      }
    }

    public SecurityToken IssuedSecurityToken
    {
      get
      {
        return this._issuedSecurityToken;
      }
      set
      {
        this._issuedSecurityToken = value;
      }
    }
  }
}
