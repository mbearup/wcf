// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net.Security;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Channels
{
    public sealed class TransportSecurityBindingElement : SecurityBindingElement
    {
        public TransportSecurityBindingElement()
            : base()
        {
        }

        private TransportSecurityBindingElement(TransportSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            // empty
        }

        internal override ISecurityCapabilities GetIndividualISecurityCapabilities()
        {
            bool supportsClientAuthentication;
            bool supportsClientWindowsIdentity;
            GetSupportingTokensCapabilities(out supportsClientAuthentication, out supportsClientWindowsIdentity);
            return new SecurityCapabilities(supportsClientAuthentication, false, supportsClientWindowsIdentity,
                ProtectionLevel.None, ProtectionLevel.None);
        }

        internal override bool SessionMode
        {
            get
            {
                return false;
            }
        }

        internal override bool SupportsDuplex
        {
            get { return true; }
        }

        internal override bool SupportsRequestReply
        {
            get { return true; }
        }


        protected override IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context)
        {
            ISecurityCapabilities property = this.GetProperty<ISecurityCapabilities>(context);
            SecurityCredentialsManager credentialsManager = context.BindingParameters.Find<SecurityCredentialsManager>() ?? (SecurityCredentialsManager) ClientCredentials.CreateDefaultCredentials();
            SecureConversationSecurityTokenParameters securityTokenParameters1 = (SecureConversationSecurityTokenParameters) null;
            if (this.EndpointSupportingTokenParameters.Endorsing.Count > 0)
                securityTokenParameters1 = this.EndpointSupportingTokenParameters.Endorsing[0] as SecureConversationSecurityTokenParameters;
            bool addChannelDemuxerIfRequired = this.RequiresChannelDemuxer();
            ChannelBuilder channelBuilder = new ChannelBuilder(context, addChannelDemuxerIfRequired);
            if (addChannelDemuxerIfRequired)
            {
#if FEATURE_CORECLR
                throw new NotImplementedException("ApplyPropertiesOnDemuxer is not implemented in .NET Core");
#else
                this.ApplyPropertiesOnDemuxer(channelBuilder, context);
#endif
            }
            BindingContext bindingContext = context.Clone();
            SecurityChannelFactory<TChannel> securityChannelFactory;
            if (securityTokenParameters1 != null)
            {
                if (securityTokenParameters1.BootstrapSecurityBindingElement == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecureConversationSecurityTokenParametersRequireBootstrapBinding")));
                securityTokenParameters1.IssuerBindingContext = bindingContext;
                if (securityTokenParameters1.RequireCancellation)
                {
#if FEATURE_CORECLR
                    throw new NotImplementedException("SessionSymmetricTransportSecurityProtocolFactory is not implemented in .NET Core");
#else
                    SessionSymmetricTransportSecurityProtocolFactory securityProtocolFactory = new SessionSymmetricTransportSecurityProtocolFactory();
                    securityProtocolFactory.SecurityTokenParameters = securityTokenParameters1.Clone();
                    ((SecureConversationSecurityTokenParameters) securityProtocolFactory.SecurityTokenParameters).IssuerBindingContext = bindingContext;
                    this.EndpointSupportingTokenParameters.Endorsing.RemoveAt(0);
                    try
                    {
                        this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, false, bindingContext, (Binding) context.Binding);
                    }
                    finally
                    {
                        this.EndpointSupportingTokenParameters.Endorsing.Insert(0, (SecurityTokenParameters) securityTokenParameters1);
                    }
                    SecuritySessionClientSettings<TChannel> sessionClientSettings = new SecuritySessionClientSettings<TChannel>();
                    sessionClientSettings.ChannelBuilder = channelBuilder;
                    sessionClientSettings.KeyRenewalInterval = this.LocalClientSettings.SessionKeyRenewalInterval;
                    sessionClientSettings.KeyRolloverInterval = this.LocalClientSettings.SessionKeyRolloverInterval;
                    sessionClientSettings.TolerateTransportFailures = this.LocalClientSettings.ReconnectTransportOnFailure;
                    sessionClientSettings.CanRenewSession = securityTokenParameters1.CanRenewSession;
                    sessionClientSettings.IssuedSecurityTokenParameters = securityTokenParameters1.Clone();
                    ((SecureConversationSecurityTokenParameters) sessionClientSettings.IssuedSecurityTokenParameters).IssuerBindingContext = bindingContext;
                    sessionClientSettings.SecurityStandardsManager = securityProtocolFactory.StandardsManager;
                    sessionClientSettings.SessionProtocolFactory = (SecurityProtocolFactory) securityProtocolFactory;
                    securityChannelFactory = new SecurityChannelFactory<TChannel>(property, context, sessionClientSettings);
#endif
                }
                else
                {
                  TransportSecurityProtocolFactory securityProtocolFactory = new TransportSecurityProtocolFactory();
                  this.EndpointSupportingTokenParameters.Endorsing.RemoveAt(0);
                  try
                  {
                    this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, false, bindingContext, (Binding) context.Binding);
                    SecureConversationSecurityTokenParameters securityTokenParameters2 = (SecureConversationSecurityTokenParameters) securityTokenParameters1.Clone();
                    securityTokenParameters2.IssuerBindingContext = bindingContext;
                    securityProtocolFactory.SecurityBindingElement.EndpointSupportingTokenParameters.Endorsing.Insert(0, (SecurityTokenParameters) securityTokenParameters2);
                  }
                  finally
                  {
                    this.EndpointSupportingTokenParameters.Endorsing.Insert(0, (SecurityTokenParameters) securityTokenParameters1);
                  }
                  securityChannelFactory = new SecurityChannelFactory<TChannel>(property, context, channelBuilder, (SecurityProtocolFactory) securityProtocolFactory);
                }
            }
            else
            {
                SecurityProtocolFactory securityProtocolFactory = this.CreateSecurityProtocolFactory<TChannel>(context, credentialsManager, false, bindingContext);
                securityChannelFactory = new SecurityChannelFactory<TChannel>(property, context, channelBuilder, securityProtocolFactory);
            }
            return (IChannelFactory<TChannel>) securityChannelFactory;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                throw ExceptionHelper.PlatformNotSupported("TransportSecurityBindingElement doesn't support ChannelProtectionRequirements yet.");
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

#region fromwcf
        internal override SecurityProtocolFactory CreateSecurityProtocolFactory<TChannel>(BindingContext context, SecurityCredentialsManager credentialsManager, bool isForService, BindingContext issuerBindingContext)
        {
          if (context == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
          if (credentialsManager == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("credentialsManager");
          TransportSecurityProtocolFactory securityProtocolFactory = new TransportSecurityProtocolFactory();
          if (isForService)
            this.ApplyAuditBehaviorSettings(context, (SecurityProtocolFactory) securityProtocolFactory);
          this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, isForService, issuerBindingContext, (Binding) context.Binding);
          securityProtocolFactory.DetectReplays = false;
          return (SecurityProtocolFactory) securityProtocolFactory;
        }
#endregion

        public override BindingElement Clone()
        {
            return new TransportSecurityBindingElement(this);
        }
    }
}
