// A shim to allow IssuedSecurityTokenProvider from WIF3.5 to be passed "back" to WCF
// Defines but does not implement the necessary methods

using Microsoft.IdentityModel.Protocols.WSTrust;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.IdentityModel.Selectors
{
    public abstract class IssuedSecurityTokenProviderShim : SecurityTokenProvider
    {
        public virtual SecurityTokenHandlerCollectionManager1 TokenHandlerCollectionManager
        { get; set; }
        
        public virtual EndpointAddress TargetAddress
        { get; set; }
        
        public virtual bool CacheIssuedTokens
        { get; set; }
        
        public virtual IdentityVerifier IdentityVerifier
        { get; set; }
        
        public virtual EndpointAddress IssuerAddress
        { get; set; }
        
        public virtual Binding IssuerBinding
        { get; set; }
        
        public virtual SecurityKeyEntropyMode KeyEntropyMode
        { get; set; }
        
        public virtual TimeSpan MaxIssuedTokenCachingTime
        { get; set; }
        
        public virtual SecurityAlgorithmSuite SecurityAlgorithmSuite
        { get; set; }
        
        public virtual MessageSecurityVersion MessageSecurityVersion
        { get; set; }
        
        public virtual SecurityTokenSerializer SecurityTokenSerializer
        { get; set; }
        
        public virtual int IssuedTokenRenewalThresholdPercentage
        { get; set; }
        
        public virtual ChannelParameterCollection ChannelParameters
        { get; set; }
        
        public virtual Collection<XmlElement> TokenRequestParameters
        { get; set; }
        
        public virtual void SetupActAsOnBehalfOfParameters(FederatedClientCredentialsParameters actAsOnBehalfOfParameters)
        { 
            throw new NotImplementedException("Inheriting class must implement SetupActAsOnBehalfOfParameters");
        }
    }
}