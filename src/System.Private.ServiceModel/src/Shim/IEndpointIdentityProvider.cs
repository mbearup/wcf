// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.IEndpointIdentityProvider
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel.Selectors;

namespace System.ServiceModel.Security
{
  /// <summary>Provides the identity of an endpoint. </summary>
  public interface IEndpointIdentityProvider
  {
    /// <summary>Gets the identity of the current endpoint, based on the security token requirements passed in.</summary>
    /// <param name="tokenRequirement">The <see cref="T:System.IdentityModel.Selectors.SecurityTokenRequirement" /> that describes security token requirements.</param>
    /// <returns>The <see cref="T:System.ServiceModel.EndpointIdentity" /> of the current endpoint.</returns>
    EndpointIdentity GetIdentityOfSelf(SecurityTokenRequirement tokenRequirement);
  }
}

