// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



using System.Net.Security;
using System.ServiceModel.Security;

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
            throw ExceptionHelper.PlatformNotSupported("TransportSecurityBindingElement.BuildChannelFactoryCore is not supported.");
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
#if FEATURE_CORECLR
      throw new NotImplementedException("TransportSecurityProtocolFactory not supported in .NET Core");
#else
      TransportSecurityProtocolFactory securityProtocolFactory = new TransportSecurityProtocolFactory();
      if (isForService)
        this.ApplyAuditBehaviorSettings(context, (SecurityProtocolFactory) securityProtocolFactory);
      this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, isForService, issuerBindingContext, (Binding) context.Binding);
      securityProtocolFactory.DetectReplays = false;
      return (SecurityProtocolFactory) securityProtocolFactory;
#endif
    }
#endregion

        public override BindingElement Clone()
        {
            return new TransportSecurityBindingElement(this);
        }
    }
}
