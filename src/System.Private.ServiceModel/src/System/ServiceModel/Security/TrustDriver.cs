// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;

using System.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class TrustDriver
    {
        // issued tokens control        
        public virtual bool IsIssuedTokensSupported
        {
            get
            {
                return false;
            }
        }

        // issued tokens feature        
        public virtual string IssuedTokensHeaderName
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.TrustDriverVersionDoesNotSupportIssuedTokens));
            }
        }

        // issued tokens feature        
        public virtual string IssuedTokensHeaderNamespace
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.TrustDriverVersionDoesNotSupportIssuedTokens));
            }
        }

        // session control
        public virtual bool IsSessionSupported
        {
            get
            {
                return false;
            }
        }

        public abstract XmlDictionaryString RequestSecurityTokenAction { get; }

        public abstract XmlDictionaryString RequestSecurityTokenResponseAction { get; }

        public abstract XmlDictionaryString RequestSecurityTokenResponseFinalAction { get; }

        // session feature
        public virtual string RequestTypeClose
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.TrustDriverVersionDoesNotSupportSession));
            }
        }

        public abstract string RequestTypeIssue { get; }

        // session feature
        public virtual string RequestTypeRenew
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.TrustDriverVersionDoesNotSupportSession));
            }
        }

#if FEATURE_CORECLR
        public virtual XmlElement CreateKeyTypeElement(SecurityKeyType keyType)
        { 
            throw new NotImplementedException("TrustDriver.CreateKeyTypeElement not implemented in .NET Core");
        }

        public virtual bool TryParseKeyTypeElement(XmlElement element, out SecurityKeyType keyType)
        {
            throw new NotImplementedException("TrustDriver.TryParseKeyTypeElement not implemented in .NET Core");
        }

        public virtual XmlElement CreateRequiredClaimsElement(IEnumerable<XmlElement> claimsList)
        {
            throw new NotImplementedException("TrustDriver.CreateRequiredClaimsElement not implemented in .NET Core");
        }

        public virtual XmlElement CreateCanonicalizationAlgorithmElement(string canonicalicationAlgorithm)
        {
            throw new NotImplementedException("TrustDriver.CreateCanonicalizationAlgorithmElement not implemented in .NET Core");
        }

        public virtual XmlElement CreateEncryptWithElement(string encryptionAlgorithm)
        {
            throw new NotImplementedException("TrustDriver.CreateEncryptWithElement not implemented in .NET Core");
        }

        public virtual XmlElement CreateEncryptionAlgorithmElement(string encryptionAlgorithm)
        {
            throw new NotImplementedException("TrustDriver.CreateEncryptionAlgorithmElement not implemented in .NET Core");
        }

        public virtual XmlElement CreateKeySizeElement(int keySize)
        {
            throw new NotImplementedException("TrustDriver.CreateKeySizeElement not implemented in .NET Core");
        }

        public virtual XmlElement CreateSignWithElement(string signatureAlgorithm)
        {
            throw new NotImplementedException("TrustDriver.CreateSignWithElement not implemented in .NET Core");
        }

        public virtual XmlElement CreateTokenTypeElement(string tokenTypeUri)
        {
           throw new NotImplementedException("TrustDriver.CreateTokenTypeElement not implemented in .NET Core");
        }

        public virtual Collection<XmlElement> ProcessUnknownRequestParameters(Collection<XmlElement> unknownRequestParameters, Collection<XmlElement> originalRequestParameters)
        {
           throw new NotImplementedException("TrustDriver.ProcessUnknownRequestParameters not implemented in .NET Core");
        }

        public virtual bool TryParseKeySizeElement(XmlElement element, out int keySize)
        {
           throw new NotImplementedException("TrustDriver.ProcessUnknownRequestParameters not implemented in .NET Core");
        }

        public virtual bool TryParseRequiredClaimsElement(XmlElement element, out Collection<XmlElement> requiredClaims)
        {
           throw new NotImplementedException("TrustDriver.ProcessUnknownRequestParameters not implemented in .NET Core");
        }

        public virtual bool TryParseTokenTypeElement(XmlElement element, out string tokenType)
        {
           throw new NotImplementedException("TrustDriver.ProcessUnknownRequestParameters not implemented in .NET Core");
        }

        internal virtual bool IsCanonicalizationAlgorithmElement(XmlElement element, out string canonicalizationAlgorithm)
        {
          canonicalizationAlgorithm = (string) null;
          return false;
        }

        internal virtual bool IsEncryptWithElement(XmlElement element, out string encryptWithAlgorithm)
        {
          encryptWithAlgorithm = (string) null;
          return false;
        }

    internal virtual bool IsEncryptionAlgorithmElement(XmlElement element, out string encryptionAlgorithm)
    {
      encryptionAlgorithm = (string) null;
      return false;
    }

    internal virtual bool IsKeyWrapAlgorithmElement(XmlElement element, out string keyWrapAlgorithm)
    {
      keyWrapAlgorithm = (string) null;
      return false;
    }

    internal virtual bool IsSignWithElement(XmlElement element, out string signatureAlgorithm)
    {
      signatureAlgorithm = (string) null;
      return false;
    }
#endif

        public abstract string ComputedKeyAlgorithm { get; }

        public abstract SecurityStandardsManager StandardsManager { get; }

        public abstract XmlDictionaryString Namespace { get; }

        // RST specific method
        public abstract RequestSecurityToken CreateRequestSecurityToken(XmlReader reader);

        // RSTR specific method
        public abstract RequestSecurityTokenResponse CreateRequestSecurityTokenResponse(XmlReader reader);

        // RSTRC specific method
        public abstract RequestSecurityTokenResponseCollection CreateRequestSecurityTokenResponseCollection(XmlReader xmlReader);

        public abstract bool IsAtRequestSecurityTokenResponse(XmlReader reader);

        public abstract bool IsAtRequestSecurityTokenResponseCollection(XmlReader reader);

        public abstract bool IsRequestedSecurityTokenElement(string name, string nameSpace);

        public abstract bool IsRequestedProofTokenElement(string name, string nameSpace);

        public abstract T GetAppliesTo<T>(RequestSecurityToken rst, XmlObjectSerializer serializer);

        public abstract T GetAppliesTo<T>(RequestSecurityTokenResponse rstr, XmlObjectSerializer serializer);

        public abstract void GetAppliesToQName(RequestSecurityToken rst, out string localName, out string namespaceUri);

        public abstract void GetAppliesToQName(RequestSecurityTokenResponse rstr, out string localName, out string namespaceUri);

        public abstract bool IsAppliesTo(string localName, string namespaceUri);

        // RSTR specific method
        public abstract byte[] GetAuthenticator(RequestSecurityTokenResponse rstr);

        // RST specific method
        public abstract BinaryNegotiation GetBinaryNegotiation(RequestSecurityToken rst);

        // RSTR specific method
        public abstract BinaryNegotiation GetBinaryNegotiation(RequestSecurityTokenResponse rstr);

        // RST specific method
        public abstract SecurityToken GetEntropy(RequestSecurityToken rst, SecurityTokenResolver resolver);

        // RSTR specific method
        public abstract SecurityToken GetEntropy(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver);

        public abstract void OnRSTRorRSTRCMissingException();

        // RST specific method
        public abstract void WriteRequestSecurityToken(RequestSecurityToken rst, XmlWriter w);

        // RSTR specific method
        public abstract void WriteRequestSecurityTokenResponse(RequestSecurityTokenResponse rstr, XmlWriter w);

        // RSTR Collection method
        public abstract void WriteRequestSecurityTokenResponseCollection(RequestSecurityTokenResponseCollection rstrCollection, XmlWriter writer);

        // Federation proxy creation
        public abstract IChannelFactory<IRequestChannel> CreateFederationProxy(EndpointAddress address, Binding binding, KeyedByTypeCollection<IEndpointBehavior> channelBehaviors);
    }
}
