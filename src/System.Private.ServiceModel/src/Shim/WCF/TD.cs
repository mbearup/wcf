// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Diagnostics.Application.TD
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.Security;
using System.Threading;

namespace System.ServiceModel.Diagnostics.Application
{
  internal class TD
  {
    private static bool shouldTraceError = true;

    internal static bool ReceiveTimeoutIsEnabled()
    {
      return shouldTraceError;
    }

    internal static void ReceiveTimeout(string param0)
    {
      CompatibilityShim.Log("ReceiveTimeout: {0}", param0);
    }

    internal static bool CloseTimeoutIsEnabled()
    {
      return shouldTraceError;
    }

    internal static void CloseTimeout(string param0)
    {
      CompatibilityShim.Log("CloseTimeout: {0}", param0);
    }

    internal static bool MessageReceivedFromTransportIsEnabled()
    {
      return false;
    }

    /*private static object syncLock = new object();
    private static ResourceManager resourceManager;
    private static CultureInfo resourceCulture;
    [SecurityCritical]
    private static EventDescriptor[] eventDescriptors;
    private static volatile bool eventDescriptorsCreated;

    private static ResourceManager ResourceManager
    {
      get
      {
        if (TD.resourceManager == null)
          TD.resourceManager = new ResourceManager("System.ServiceModel.Diagnostics.Application.TD", typeof (TD).Assembly);
        return TD.resourceManager;
      }
    }

    internal static CultureInfo Culture
    {
      get
      {
        return TD.resourceCulture;
      }
      set
      {
        TD.resourceCulture = value;
      }
    }

    private TD()
    {
    }

    internal static bool ClientOperationPreparedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(0);
      return false;
    }

    internal static void ClientOperationPrepared(EventTraceActivity eventTraceActivity, string Action, string ContractName, string Destination, Guid relatedActivityId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(0))
        return;
      TD.WriteEtwTransferEvent(0, eventTraceActivity, relatedActivityId, Action, ContractName, Destination, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientMessageInspectorAfterReceiveInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(1);
      return false;
    }

    internal static void ClientMessageInspectorAfterReceiveInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(1))
        return;
      TD.WriteEtwEvent(1, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientMessageInspectorBeforeSendInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(2);
      return false;
    }

    internal static void ClientMessageInspectorBeforeSendInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(2))
        return;
      TD.WriteEtwEvent(2, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientParameterInspectorAfterCallInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(3);
      return false;
    }

    internal static void ClientParameterInspectorAfterCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(3))
        return;
      TD.WriteEtwEvent(3, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientParameterInspectorBeforeCallInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(4);
      return false;
    }

    internal static void ClientParameterInspectorBeforeCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(4))
        return;
      TD.WriteEtwEvent(4, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool OperationInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(5);
      return false;
    }

    internal static void OperationInvoked(EventTraceActivity eventTraceActivity, string MethodName, string CallerInfo)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(5))
        return;
      TD.WriteEtwEvent(5, eventTraceActivity, MethodName, CallerInfo, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ErrorHandlerInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(6);
      return false;
    }

    internal static void ErrorHandlerInvoked(string TypeName, bool Handled, string ExceptionTypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(6))
        return;
      TD.WriteEtwEvent(6, (EventTraceActivity) null, TypeName, Handled, ExceptionTypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool FaultProviderInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(7);
      return false;
    }

    internal static void FaultProviderInvoked(string TypeName, string ExceptionTypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(7))
        return;
      TD.WriteEtwEvent(7, (EventTraceActivity) null, TypeName, ExceptionTypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageInspectorAfterReceiveInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(8);
      return false;
    }

    internal static void MessageInspectorAfterReceiveInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(8))
        return;
      TD.WriteEtwEvent(8, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageInspectorBeforeSendInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(9);
      return false;
    }

    internal static void MessageInspectorBeforeSendInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(9))
        return;
      TD.WriteEtwEvent(9, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageThrottleExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(10);
      return false;
    }

    internal static void MessageThrottleExceeded(string ThrottleName, long Limit)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(10))
        return;
      TD.WriteEtwEvent(10, (EventTraceActivity) null, ThrottleName, Limit, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ParameterInspectorAfterCallInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(11);
      return false;
    }

    internal static void ParameterInspectorAfterCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(11))
        return;
      TD.WriteEtwEvent(11, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ParameterInspectorBeforeCallInvokedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(12);
      return false;
    }

    internal static void ParameterInspectorBeforeCallInvoked(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(12))
        return;
      TD.WriteEtwEvent(12, eventTraceActivity, TypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool OperationCompletedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(13);
      return false;
    }

    internal static void OperationCompleted(EventTraceActivity eventTraceActivity, string MethodName, long Duration)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(13))
        return;
      TD.WriteEtwEvent(13, eventTraceActivity, MethodName, Duration, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageReceivedByTransportIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(14);
      return false;
    }

    internal static void MessageReceivedByTransport(EventTraceActivity eventTraceActivity, string ListenAddress, Guid relatedActivityId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(14))
        return;
      TD.WriteEtwTransferEvent(14, eventTraceActivity, relatedActivityId, ListenAddress, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageSentByTransportIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(15);
      return false;
    }

    internal static void MessageSentByTransport(EventTraceActivity eventTraceActivity, string DestinationAddress)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(15))
        return;
      TD.WriteEtwEvent(15, eventTraceActivity, DestinationAddress, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageLogInfoIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(16);
      return false;
    }

    internal static bool MessageLogInfo(string param0)
    {
      bool flag = true;
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (TD.IsEtwEventEnabled(16))
        flag = TD.WriteEtwEvent(16, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
      return flag;
    }

    internal static bool MessageLogWarningIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(17);
      return false;
    }

    internal static bool MessageLogWarning(string param0)
    {
      bool flag = true;
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (TD.IsEtwEventEnabled(17))
        flag = TD.WriteEtwEvent(17, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
      return flag;
    }

    internal static bool MessageLogEventSizeExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(18);
      return false;
    }

    internal static void MessageLogEventSizeExceeded()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(18))
        return;
      TD.WriteEtwEvent(18, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ResumeSignpostEventIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(19);
      return false;
    }

    internal static void ResumeSignpostEvent(TraceRecord traceRecord)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (!TD.IsEtwEventEnabled(19))
        return;
      TD.WriteEtwEvent(19, (EventTraceActivity) null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool StartSignpostEventIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(20);
      return false;
    }

    internal static void StartSignpostEvent(TraceRecord traceRecord)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (!TD.IsEtwEventEnabled(20))
        return;
      TD.WriteEtwEvent(20, (EventTraceActivity) null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool StopSignpostEventIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(21);
      return false;
    }

    internal static void StopSignpostEvent(TraceRecord traceRecord)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (!TD.IsEtwEventEnabled(21))
        return;
      TD.WriteEtwEvent(21, (EventTraceActivity) null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SuspendSignpostEventIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(22);
      return false;
    }

    internal static void SuspendSignpostEvent(TraceRecord traceRecord)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (!TD.IsEtwEventEnabled(22))
        return;
      TD.WriteEtwEvent(22, (EventTraceActivity) null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceChannelCallStopIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(23);
      return false;
    }

    internal static void ServiceChannelCallStop(EventTraceActivity eventTraceActivity, string Action, string ContractName, string Destination)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(23))
        return;
      TD.WriteEtwEvent(23, eventTraceActivity, Action, ContractName, Destination, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceExceptionIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(24);
      return false;
    }

    internal static void ServiceException(EventTraceActivity eventTraceActivity, string ExceptionToString, string ExceptionTypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(24))
        return;
      TD.WriteEtwEvent(24, eventTraceActivity, ExceptionToString, ExceptionTypeName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool OperationFailedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(25);
      return false;
    }

    internal static void OperationFailed(EventTraceActivity eventTraceActivity, string MethodName, long Duration)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(25))
        return;
      TD.WriteEtwEvent(25, eventTraceActivity, MethodName, Duration, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool OperationFaultedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(26);
      return false;
    }

    internal static void OperationFaulted(EventTraceActivity eventTraceActivity, string MethodName, long Duration)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(26))
        return;
      TD.WriteEtwEvent(26, eventTraceActivity, MethodName, Duration, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageThrottleAtSeventyPercentIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(27);
      return false;
    }

    internal static void MessageThrottleAtSeventyPercent(string ThrottleName, long Limit)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(27))
        return;
      TD.WriteEtwEvent(27, (EventTraceActivity) null, ThrottleName, Limit, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageReceivedFromTransportIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(28);
      return false;
    }

    internal static void MessageReceivedFromTransport(EventTraceActivity eventTraceActivity, Guid CorrelationId, string reference)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(28))
        return;
      TD.WriteEtwEvent(28, eventTraceActivity, CorrelationId, reference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageSentToTransportIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(29);
      return false;
    }

    internal static void MessageSentToTransport(EventTraceActivity eventTraceActivity, Guid CorrelationId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(29))
        return;
      TD.WriteEtwEvent(29, eventTraceActivity, CorrelationId, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceHostOpenStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(30);
      return false;
    }

    internal static void ServiceHostOpenStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(30))
        return;
      TD.WriteEtwEvent(30, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceHostOpenStopIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(31);
      return false;
    }

    internal static void ServiceHostOpenStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(31))
        return;
      TD.WriteEtwEvent(31, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceChannelOpenStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(32);
      return false;
    }

    internal static void ServiceChannelOpenStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(32))
        return;
      TD.WriteEtwEvent(32, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceChannelOpenStopIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(33);
      return false;
    }

    internal static void ServiceChannelOpenStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(33))
        return;
      TD.WriteEtwEvent(33, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceChannelCallStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(34);
      return false;
    }

    internal static void ServiceChannelCallStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(34))
        return;
      TD.WriteEtwEvent(34, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceChannelBeginCallStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(35);
      return false;
    }

    internal static void ServiceChannelBeginCallStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(35))
        return;
      TD.WriteEtwEvent(35, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpSendMessageStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(36);
      return false;
    }

    internal static void HttpSendMessageStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(36))
        return;
      TD.WriteEtwEvent(36, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpSendStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(37);
      return false;
    }

    internal static void HttpSendStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(37))
        return;
      TD.WriteEtwEvent(37, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpMessageReceiveStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(38);
      return false;
    }

    internal static void HttpMessageReceiveStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(38))
        return;
      TD.WriteEtwEvent(38, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchMessageStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(39);
      return false;
    }

    internal static void DispatchMessageStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(39))
        return;
      TD.WriteEtwEvent(39, eventTraceActivity, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpContextBeforeProcessAuthenticationIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(40);
      return false;
    }

    internal static void HttpContextBeforeProcessAuthentication(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(40))
        return;
      TD.WriteEtwEvent(40, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchMessageBeforeAuthorizationIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(41);
      return false;
    }

    internal static void DispatchMessageBeforeAuthorization(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(41))
        return;
      TD.WriteEtwEvent(41, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchMessageStopIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(42);
      return false;
    }

    internal static void DispatchMessageStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(42))
        return;
      TD.WriteEtwEvent(42, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientChannelOpenStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(43);
      return false;
    }

    internal static void ClientChannelOpenStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(43))
        return;
      TD.WriteEtwEvent(43, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientChannelOpenStopIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(44);
      return false;
    }

    internal static void ClientChannelOpenStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(44))
        return;
      TD.WriteEtwEvent(44, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpSendStreamedMessageStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(45);
      return false;
    }

    internal static void HttpSendStreamedMessageStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(45))
        return;
      TD.WriteEtwEvent(45, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ReceiveContextAbandonFailedIsEnabled()
    {
      if (!FxTrace.ShouldTraceWarning)
        return false;
      if (!FxTrace.ShouldTraceWarningToTraceSource)
        return TD.IsEtwEventEnabled(46);
      return true;
    }

    internal static void ReceiveContextAbandonFailed(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (TD.IsEtwEventEnabled(46))
        TD.WriteEtwEvent(46, eventTraceActivity, TypeName, serializedPayload.AppDomainFriendlyName);
      if (!FxTrace.ShouldTraceWarningToTraceSource)
        return;
      TD.WriteTraceSource(46, string.Format((IFormatProvider) TD.Culture, TD.ResourceManager.GetString("ReceiveContextAbandonFailed", TD.Culture), new object[1]
      {
        (object) TypeName
      }), serializedPayload);
    }

    internal static bool ReceiveContextAbandonWithExceptionIsEnabled()
    {
      if (!FxTrace.ShouldTraceInformation)
        return false;
      if (!FxTrace.ShouldTraceInformationToTraceSource)
        return TD.IsEtwEventEnabled(47);
      return true;
    }

    internal static void ReceiveContextAbandonWithException(EventTraceActivity eventTraceActivity, string TypeName, string ExceptionToString)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (TD.IsEtwEventEnabled(47))
        TD.WriteEtwEvent(47, eventTraceActivity, TypeName, ExceptionToString, serializedPayload.AppDomainFriendlyName);
      if (!FxTrace.ShouldTraceInformationToTraceSource)
        return;
      TD.WriteTraceSource(47, string.Format((IFormatProvider) TD.Culture, TD.ResourceManager.GetString("ReceiveContextAbandonWithException", TD.Culture), new object[2]
      {
        (object) TypeName,
        (object) ExceptionToString
      }), serializedPayload);
    }

    internal static bool ReceiveContextCompleteFailedIsEnabled()
    {
      if (!FxTrace.ShouldTraceWarning)
        return false;
      if (!FxTrace.ShouldTraceWarningToTraceSource)
        return TD.IsEtwEventEnabled(48);
      return true;
    }

    internal static void ReceiveContextCompleteFailed(EventTraceActivity eventTraceActivity, string TypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (TD.IsEtwEventEnabled(48))
        TD.WriteEtwEvent(48, eventTraceActivity, TypeName, serializedPayload.AppDomainFriendlyName);
      if (!FxTrace.ShouldTraceWarningToTraceSource)
        return;
      TD.WriteTraceSource(48, string.Format((IFormatProvider) TD.Culture, TD.ResourceManager.GetString("ReceiveContextCompleteFailed", TD.Culture), new object[1]
      {
        (object) TypeName
      }), serializedPayload);
    }

    internal static bool ReceiveContextFaultedIsEnabled()
    {
      if (!FxTrace.ShouldTraceWarning)
        return false;
      if (!FxTrace.ShouldTraceWarningToTraceSource)
        return TD.IsEtwEventEnabled(49);
      return true;
    }

    internal static void ReceiveContextFaulted(EventTraceActivity eventTraceActivity, object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (TD.IsEtwEventEnabled(49))
        TD.WriteEtwEvent(49, eventTraceActivity, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
      if (!FxTrace.ShouldTraceWarningToTraceSource)
        return;
      TD.WriteTraceSource(49, string.Format((IFormatProvider) TD.Culture, TD.ResourceManager.GetString("ReceiveContextFaulted", TD.Culture), new object[0]), serializedPayload);
    }

    internal static bool ClientBaseCachedChannelFactoryCountIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(50);
      return false;
    }

    internal static void ClientBaseCachedChannelFactoryCount(int Count, int MaxNum, object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(50))
        return;
      TD.WriteEtwEvent(50, (EventTraceActivity) null, Count, MaxNum, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientBaseChannelFactoryAgedOutofCacheIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(51);
      return false;
    }

    internal static void ClientBaseChannelFactoryAgedOutofCache(int Count, object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(51))
        return;
      TD.WriteEtwEvent(51, (EventTraceActivity) null, Count, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientBaseChannelFactoryCacheHitIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(52);
      return false;
    }

    internal static void ClientBaseChannelFactoryCacheHit(object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(52))
        return;
      TD.WriteEtwEvent(52, (EventTraceActivity) null, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientBaseUsingLocalChannelFactoryIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(53);
      return false;
    }

    internal static void ClientBaseUsingLocalChannelFactory(object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(53))
        return;
      TD.WriteEtwEvent(53, (EventTraceActivity) null, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool QueryCompositionExecutedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(54);
      return false;
    }

    internal static void QueryCompositionExecuted(EventTraceActivity eventTraceActivity, string TypeName, string Uri, object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(54))
        return;
      TD.WriteEtwEvent(54, eventTraceActivity, TypeName, Uri, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchFailedIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(55);
      return false;
    }

    internal static void DispatchFailed(EventTraceActivity eventTraceActivity, string OperationName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(55))
        return;
      TD.WriteEtwEvent(55, eventTraceActivity, OperationName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchSuccessfulIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(56);
      return false;
    }

    internal static void DispatchSuccessful(EventTraceActivity eventTraceActivity, string OperationName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null, true);
      if (!TD.IsEtwEventEnabled(56))
        return;
      TD.WriteEtwEvent(56, eventTraceActivity, OperationName, serializedPayload.HostReference, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageReadByEncoderIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(57);
      return false;
    }

    internal static void MessageReadByEncoder(EventTraceActivity eventTraceActivity, int Size, object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(57))
        return;
      TD.WriteEtwEvent(57, eventTraceActivity, Size, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageWrittenByEncoderIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(58);
      return false;
    }

    internal static void MessageWrittenByEncoder(EventTraceActivity eventTraceActivity, int Size, object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(58))
        return;
      TD.WriteEtwEvent(58, eventTraceActivity, Size, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SessionIdleTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(59);
      return false;
    }

    internal static void SessionIdleTimeout(string RemoteAddress)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(59))
        return;
      TD.WriteEtwEvent(59, (EventTraceActivity) null, RemoteAddress, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketAcceptEnqueuedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(60);
      return false;
    }

    internal static void SocketAcceptEnqueued(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(60))
        return;
      TD.WriteEtwEvent(60, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketAcceptedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(61);
      return false;
    }

    internal static void SocketAccepted(EventTraceActivity eventTraceActivity, int ListenerHashCode, int SocketHashCode)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(61))
        return;
      TD.WriteEtwEvent(61, eventTraceActivity, ListenerHashCode, SocketHashCode, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ConnectionPoolMissIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(62);
      return false;
    }

    internal static void ConnectionPoolMiss(string PoolKey, int busy)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(62))
        return;
      TD.WriteEtwEvent(62, (EventTraceActivity) null, PoolKey, busy, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchFormatterDeserializeRequestStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(63);
      return false;
    }

    internal static void DispatchFormatterDeserializeRequestStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(63))
        return;
      TD.WriteEtwEvent(63, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchFormatterDeserializeRequestStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(64);
      return false;
    }

    internal static void DispatchFormatterDeserializeRequestStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(64))
        return;
      TD.WriteEtwEvent(64, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchFormatterSerializeReplyStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(65);
      return false;
    }

    internal static void DispatchFormatterSerializeReplyStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(65))
        return;
      TD.WriteEtwEvent(65, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool DispatchFormatterSerializeReplyStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(66);
      return false;
    }

    internal static void DispatchFormatterSerializeReplyStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(66))
        return;
      TD.WriteEtwEvent(66, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientFormatterSerializeRequestStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(67);
      return false;
    }

    internal static void ClientFormatterSerializeRequestStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(67))
        return;
      TD.WriteEtwEvent(67, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientFormatterSerializeRequestStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(68);
      return false;
    }

    internal static void ClientFormatterSerializeRequestStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(68))
        return;
      TD.WriteEtwEvent(68, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientFormatterDeserializeReplyStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(69);
      return false;
    }

    internal static void ClientFormatterDeserializeReplyStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(69))
        return;
      TD.WriteEtwEvent(69, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientFormatterDeserializeReplyStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(70);
      return false;
    }

    internal static void ClientFormatterDeserializeReplyStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(70))
        return;
      TD.WriteEtwEvent(70, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityNegotiationStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(71);
      return false;
    }

    internal static void SecurityNegotiationStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(71))
        return;
      TD.WriteEtwEvent(71, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityNegotiationStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(72);
      return false;
    }

    internal static void SecurityNegotiationStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(72))
        return;
      TD.WriteEtwEvent(72, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityTokenProviderOpenedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(73);
      return false;
    }

    internal static void SecurityTokenProviderOpened(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(73))
        return;
      TD.WriteEtwEvent(73, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool OutgoingMessageSecuredIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(74);
      return false;
    }

    internal static void OutgoingMessageSecured(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(74))
        return;
      TD.WriteEtwEvent(74, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool IncomingMessageVerifiedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(75);
      return false;
    }

    internal static void IncomingMessageVerified(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(75))
        return;
      TD.WriteEtwEvent(75, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool GetServiceInstanceStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(76);
      return false;
    }

    internal static void GetServiceInstanceStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(76))
        return;
      TD.WriteEtwEvent(76, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool GetServiceInstanceStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(77);
      return false;
    }

    internal static void GetServiceInstanceStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(77))
        return;
      TD.WriteEtwEvent(77, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ChannelReceiveStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(78);
      return false;
    }

    internal static void ChannelReceiveStart(EventTraceActivity eventTraceActivity, int ChannelId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(78))
        return;
      TD.WriteEtwEvent(78, eventTraceActivity, ChannelId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ChannelReceiveStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(79);
      return false;
    }

    internal static void ChannelReceiveStop(EventTraceActivity eventTraceActivity, int ChannelId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(79))
        return;
      TD.WriteEtwEvent(79, eventTraceActivity, ChannelId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ChannelFactoryCreatedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(80);
      return false;
    }

    internal static void ChannelFactoryCreated(object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(80))
        return;
      TD.WriteEtwEvent(80, (EventTraceActivity) null, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool PipeConnectionAcceptStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(81);
      return false;
    }

    internal static void PipeConnectionAcceptStart(EventTraceActivity eventTraceActivity, string uri)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(81))
        return;
      TD.WriteEtwEvent(81, eventTraceActivity, uri, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool PipeConnectionAcceptStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(82);
      return false;
    }

    internal static void PipeConnectionAcceptStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(82))
        return;
      TD.WriteEtwEvent(82, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool EstablishConnectionStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(83);
      return false;
    }

    internal static void EstablishConnectionStart(EventTraceActivity eventTraceActivity, string Key)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(83))
        return;
      TD.WriteEtwEvent(83, eventTraceActivity, Key, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool EstablishConnectionStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(84);
      return false;
    }

    internal static void EstablishConnectionStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(84))
        return;
      TD.WriteEtwEvent(84, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SessionPreambleUnderstoodIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(85);
      return false;
    }

    internal static void SessionPreambleUnderstood(string Via)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(85))
        return;
      TD.WriteEtwEvent(85, (EventTraceActivity) null, Via, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ConnectionReaderSendFaultIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(86);
      return false;
    }

    internal static void ConnectionReaderSendFault(string FaultString)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(86))
        return;
      TD.WriteEtwEvent(86, (EventTraceActivity) null, FaultString, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketAcceptClosedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(87);
      return false;
    }

    internal static void SocketAcceptClosed(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(87))
        return;
      TD.WriteEtwEvent(87, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceHostFaultedIsEnabled()
    {
      if (FxTrace.ShouldTraceCritical)
        return TD.IsEtwEventEnabled(88);
      return false;
    }

    internal static void ServiceHostFaulted(EventTraceActivity eventTraceActivity, object source)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(source, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(88))
        return;
      TD.WriteEtwEvent(88, eventTraceActivity, serializedPayload.EventSource, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ListenerOpenStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(89);
      return false;
    }

    internal static void ListenerOpenStart(EventTraceActivity eventTraceActivity, string Uri, Guid relatedActivityId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(89))
        return;
      TD.WriteEtwTransferEvent(89, eventTraceActivity, relatedActivityId, Uri, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ListenerOpenStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(90);
      return false;
    }

    internal static void ListenerOpenStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(90))
        return;
      TD.WriteEtwEvent(90, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServerMaxPooledConnectionsQuotaReachedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(91);
      return false;
    }

    internal static void ServerMaxPooledConnectionsQuotaReached()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(91))
        return;
      TD.WriteEtwEvent(91, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool TcpConnectionTimedOutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(92);
      return false;
    }

    internal static void TcpConnectionTimedOut(int SocketId, string Uri)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(92))
        return;
      TD.WriteEtwEvent(92, (EventTraceActivity) null, SocketId, Uri, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool TcpConnectionResetErrorIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(93);
      return false;
    }

    internal static void TcpConnectionResetError(int SocketId, string Uri)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(93))
        return;
      TD.WriteEtwEvent(93, (EventTraceActivity) null, SocketId, Uri, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ServiceSecurityNegotiationCompletedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(94);
      return false;
    }

    internal static void ServiceSecurityNegotiationCompleted(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(94))
        return;
      TD.WriteEtwEvent(94, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityNegotiationProcessingFailureIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(95);
      return false;
    }

    internal static void SecurityNegotiationProcessingFailure(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(95))
        return;
      TD.WriteEtwEvent(95, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityIdentityVerificationSuccessIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(96);
      return false;
    }

    internal static void SecurityIdentityVerificationSuccess(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(96))
        return;
      TD.WriteEtwEvent(96, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityIdentityVerificationFailureIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(97);
      return false;
    }

    internal static void SecurityIdentityVerificationFailure(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(97))
        return;
      TD.WriteEtwEvent(97, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool PortSharingDuplicatedSocketIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(98);
      return false;
    }

    internal static void PortSharingDuplicatedSocket(EventTraceActivity eventTraceActivity, string Uri)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(98))
        return;
      TD.WriteEtwEvent(98, eventTraceActivity, Uri, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityImpersonationSuccessIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(99);
      return false;
    }

    internal static void SecurityImpersonationSuccess(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(99))
        return;
      TD.WriteEtwEvent(99, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecurityImpersonationFailureIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(100);
      return false;
    }

    internal static void SecurityImpersonationFailure(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(100))
        return;
      TD.WriteEtwEvent(100, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpChannelRequestAbortedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(101);
      return false;
    }

    internal static void HttpChannelRequestAborted(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(101))
        return;
      TD.WriteEtwEvent(101, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpChannelResponseAbortedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(102);
      return false;
    }

    internal static void HttpChannelResponseAborted(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(102))
        return;
      TD.WriteEtwEvent(102, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpAuthFailedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(103);
      return false;
    }

    internal static void HttpAuthFailed(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(103))
        return;
      TD.WriteEtwEvent(103, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SharedListenerProxyRegisterStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(104);
      return false;
    }

    internal static void SharedListenerProxyRegisterStart(string Uri)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(104))
        return;
      TD.WriteEtwEvent(104, (EventTraceActivity) null, Uri, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SharedListenerProxyRegisterStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(105);
      return false;
    }

    internal static void SharedListenerProxyRegisterStop()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(105))
        return;
      TD.WriteEtwEvent(105, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SharedListenerProxyRegisterFailedIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(106);
      return false;
    }

    internal static void SharedListenerProxyRegisterFailed(string Status)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(106))
        return;
      TD.WriteEtwEvent(106, (EventTraceActivity) null, Status, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ConnectionPoolPreambleFailedIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(107);
      return false;
    }

    internal static void ConnectionPoolPreambleFailed(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(107))
        return;
      TD.WriteEtwEvent(107, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SslOnInitiateUpgradeIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(108);
      return false;
    }

    internal static void SslOnInitiateUpgrade()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(108))
        return;
      TD.WriteEtwEvent(108, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SslOnAcceptUpgradeIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(109);
      return false;
    }

    internal static void SslOnAcceptUpgrade(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(109))
        return;
      TD.WriteEtwEvent(109, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool BinaryMessageEncodingStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(110);
      return false;
    }

    internal static void BinaryMessageEncodingStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(110))
        return;
      TD.WriteEtwEvent(110, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MtomMessageEncodingStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(111);
      return false;
    }

    internal static void MtomMessageEncodingStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(111))
        return;
      TD.WriteEtwEvent(111, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool TextMessageEncodingStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(112);
      return false;
    }

    internal static void TextMessageEncodingStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(112))
        return;
      TD.WriteEtwEvent(112, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool BinaryMessageDecodingStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(113);
      return false;
    }

    internal static void BinaryMessageDecodingStart()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(113))
        return;
      TD.WriteEtwEvent(113, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MtomMessageDecodingStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(114);
      return false;
    }

    internal static void MtomMessageDecodingStart()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(114))
        return;
      TD.WriteEtwEvent(114, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool TextMessageDecodingStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(115);
      return false;
    }

    internal static void TextMessageDecodingStart()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(115))
        return;
      TD.WriteEtwEvent(115, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpResponseReceiveStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(116);
      return false;
    }

    internal static void HttpResponseReceiveStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(116))
        return;
      TD.WriteEtwEvent(116, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketReadStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(117);
      return false;
    }

    internal static void SocketReadStop(int SocketId, int Size, string Endpoint)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(117))
        return;
      TD.WriteEtwEvent(117, (EventTraceActivity) null, SocketId, Size, Endpoint, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketAsyncReadStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(118);
      return false;
    }

    internal static void SocketAsyncReadStop(int SocketId, int Size, string Endpoint)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(118))
        return;
      TD.WriteEtwEvent(118, (EventTraceActivity) null, SocketId, Size, Endpoint, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketWriteStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(119);
      return false;
    }

    internal static void SocketWriteStart(int SocketId, int Size, string Endpoint)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(119))
        return;
      TD.WriteEtwEvent(119, (EventTraceActivity) null, SocketId, Size, Endpoint, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketAsyncWriteStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(120);
      return false;
    }

    internal static void SocketAsyncWriteStart(int SocketId, int Size, string Endpoint)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(120))
        return;
      TD.WriteEtwEvent(120, (EventTraceActivity) null, SocketId, Size, Endpoint, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SequenceAcknowledgementSentIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(121);
      return false;
    }

    internal static void SequenceAcknowledgementSent(string SessionId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(121))
        return;
      TD.WriteEtwEvent(121, (EventTraceActivity) null, SessionId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientReliableSessionReconnectIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(122);
      return false;
    }

    internal static void ClientReliableSessionReconnect(string SessionId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(122))
        return;
      TD.WriteEtwEvent(122, (EventTraceActivity) null, SessionId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ReliableSessionChannelFaultedIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(123);
      return false;
    }

    internal static void ReliableSessionChannelFaulted(string SessionId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(123))
        return;
      TD.WriteEtwEvent(123, (EventTraceActivity) null, SessionId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WindowsStreamSecurityOnInitiateUpgradeIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(124);
      return false;
    }

    internal static void WindowsStreamSecurityOnInitiateUpgrade()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(124))
        return;
      TD.WriteEtwEvent(124, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WindowsStreamSecurityOnAcceptUpgradeIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(125);
      return false;
    }

    internal static void WindowsStreamSecurityOnAcceptUpgrade(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(125))
        return;
      TD.WriteEtwEvent(125, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SocketConnectionAbortIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(126);
      return false;
    }

    internal static void SocketConnectionAbort(int SocketId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(126))
        return;
      TD.WriteEtwEvent(126, (EventTraceActivity) null, SocketId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpGetContextStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled((int) sbyte.MaxValue);
      return false;
    }

    internal static void HttpGetContextStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled((int) sbyte.MaxValue))
        return;
      TD.WriteEtwEvent((int) sbyte.MaxValue, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientSendPreambleStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(128);
      return false;
    }

    internal static void ClientSendPreambleStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(128))
        return;
      TD.WriteEtwEvent(128, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ClientSendPreambleStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(129);
      return false;
    }

    internal static void ClientSendPreambleStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(129))
        return;
      TD.WriteEtwEvent(129, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpMessageReceiveFailedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(130);
      return false;
    }

    internal static void HttpMessageReceiveFailed()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(130))
        return;
      TD.WriteEtwEvent(130, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool TransactionScopeCreateIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(131);
      return false;
    }

    internal static void TransactionScopeCreate(EventTraceActivity eventTraceActivity, string LocalId, Guid Distributed)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(131))
        return;
      TD.WriteEtwEvent(131, eventTraceActivity, LocalId, Distributed, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool StreamedMessageReadByEncoderIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(132);
      return false;
    }

    internal static void StreamedMessageReadByEncoder(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(132))
        return;
      TD.WriteEtwEvent(132, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool StreamedMessageWrittenByEncoderIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(133);
      return false;
    }

    internal static void StreamedMessageWrittenByEncoder(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(133))
        return;
      TD.WriteEtwEvent(133, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MessageWrittenAsynchronouslyByEncoderIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(134);
      return false;
    }

    internal static void MessageWrittenAsynchronouslyByEncoder(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(134))
        return;
      TD.WriteEtwEvent(134, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool BufferedAsyncWriteStartIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(135);
      return false;
    }

    internal static void BufferedAsyncWriteStart(EventTraceActivity eventTraceActivity, int BufferId, int Size)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(135))
        return;
      TD.WriteEtwEvent(135, eventTraceActivity, BufferId, Size, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool BufferedAsyncWriteStopIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(136);
      return false;
    }

    internal static void BufferedAsyncWriteStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(136))
        return;
      TD.WriteEtwEvent(136, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ChannelInitializationTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(137);
      return false;
    }

    internal static void ChannelInitializationTimeout(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(137))
        return;
      TD.WriteEtwEvent(137, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool CloseTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(138);
      return false;
    }

    internal static void CloseTimeout(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(138))
        return;
      TD.WriteEtwEvent(138, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool IdleTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(139);
      return false;
    }

    internal static void IdleTimeout(string msg, string key)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(139))
        return;
      TD.WriteEtwEvent(139, (EventTraceActivity) null, msg, key, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool LeaseTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(140);
      return false;
    }

    internal static void LeaseTimeout(string msg, string key)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(140))
        return;
      TD.WriteEtwEvent(140, (EventTraceActivity) null, msg, key, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool OpenTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(141);
      return false;
    }

    internal static void OpenTimeout(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(141))
        return;
      TD.WriteEtwEvent(141, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ReceiveTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(142);
      return false;
    }

    internal static void ReceiveTimeout(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(142))
        return;
      TD.WriteEtwEvent(142, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SendTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(143);
      return false;
    }

    internal static void SendTimeout(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(143))
        return;
      TD.WriteEtwEvent(143, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool InactivityTimeoutIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(144);
      return false;
    }

    internal static void InactivityTimeout(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(144))
        return;
      TD.WriteEtwEvent(144, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MaxReceivedMessageSizeExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(145);
      return false;
    }

    internal static void MaxReceivedMessageSizeExceeded(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(145))
        return;
      TD.WriteEtwEvent(145, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MaxSentMessageSizeExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(146);
      return false;
    }

    internal static void MaxSentMessageSizeExceeded(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(146))
        return;
      TD.WriteEtwEvent(146, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MaxOutboundConnectionsPerEndpointExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(147);
      return false;
    }

    internal static void MaxOutboundConnectionsPerEndpointExceeded(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(147))
        return;
      TD.WriteEtwEvent(147, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MaxPendingConnectionsExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(148);
      return false;
    }

    internal static void MaxPendingConnectionsExceeded(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(148))
        return;
      TD.WriteEtwEvent(148, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool NegotiateTokenAuthenticatorStateCacheExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(149);
      return false;
    }

    internal static void NegotiateTokenAuthenticatorStateCacheExceeded(string msg)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(149))
        return;
      TD.WriteEtwEvent(149, (EventTraceActivity) null, msg, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool NegotiateTokenAuthenticatorStateCacheRatioIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(150);
      return false;
    }

    internal static void NegotiateTokenAuthenticatorStateCacheRatio(int cur, int max)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(150))
        return;
      TD.WriteEtwEvent(150, (EventTraceActivity) null, cur, max, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SecuritySessionRatioIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(151);
      return false;
    }

    internal static void SecuritySessionRatio(int cur, int max)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(151))
        return;
      TD.WriteEtwEvent(151, (EventTraceActivity) null, cur, max, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool PendingConnectionsRatioIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(152);
      return false;
    }

    internal static void PendingConnectionsRatio(int cur, int max)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(152))
        return;
      TD.WriteEtwEvent(152, (EventTraceActivity) null, cur, max, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool OutboundConnectionsPerEndpointRatioIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(153);
      return false;
    }

    internal static void OutboundConnectionsPerEndpointRatio(int cur, int max)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(153))
        return;
      TD.WriteEtwEvent(153, (EventTraceActivity) null, cur, max, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ConcurrentInstancesRatioIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(154);
      return false;
    }

    internal static void ConcurrentInstancesRatio(int cur, int max)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(154))
        return;
      TD.WriteEtwEvent(154, (EventTraceActivity) null, cur, max, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ConcurrentSessionsRatioIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(155);
      return false;
    }

    internal static void ConcurrentSessionsRatio(int cur, int max)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(155))
        return;
      TD.WriteEtwEvent(155, (EventTraceActivity) null, cur, max, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ConcurrentCallsRatioIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(156);
      return false;
    }

    internal static void ConcurrentCallsRatio(int cur, int max)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(156))
        return;
      TD.WriteEtwEvent(156, (EventTraceActivity) null, cur, max, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool PendingAcceptsAtZeroIsEnabled()
    {
      if (FxTrace.ShouldTraceInformation)
        return TD.IsEtwEventEnabled(157);
      return false;
    }

    internal static void PendingAcceptsAtZero()
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(157))
        return;
      TD.WriteEtwEvent(157, (EventTraceActivity) null, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MaxSessionSizeReachedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(158);
      return false;
    }

    internal static void MaxSessionSizeReached(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(158))
        return;
      TD.WriteEtwEvent(158, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ReceiveRetryCountReachedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(159);
      return false;
    }

    internal static void ReceiveRetryCountReached(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(159))
        return;
      TD.WriteEtwEvent(159, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MaxRetryCyclesExceededMsmqIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(160);
      return false;
    }

    internal static void MaxRetryCyclesExceededMsmq(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(160))
        return;
      TD.WriteEtwEvent(160, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool ReadPoolMissIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(161);
      return false;
    }

    internal static void ReadPoolMiss(string itemTypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(161))
        return;
      TD.WriteEtwEvent(161, (EventTraceActivity) null, itemTypeName, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WritePoolMissIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(162);
      return false;
    }

    internal static void WritePoolMiss(string itemTypeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(162))
        return;
      TD.WriteEtwEvent(162, (EventTraceActivity) null, itemTypeName, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool MaxRetryCyclesExceededIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(163);
      return false;
    }

    internal static void MaxRetryCyclesExceeded(string param0)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(163))
        return;
      TD.WriteEtwEvent(163, (EventTraceActivity) null, param0, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool PipeSharedMemoryCreatedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(164);
      return false;
    }

    internal static void PipeSharedMemoryCreated(string sharedMemoryName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(164))
        return;
      TD.WriteEtwEvent(164, (EventTraceActivity) null, sharedMemoryName, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool NamedPipeCreatedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(165);
      return false;
    }

    internal static void NamedPipeCreated(string pipeName)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(165))
        return;
      TD.WriteEtwEvent(165, (EventTraceActivity) null, pipeName, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool EncryptedDataProcessingStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(166);
      return false;
    }

    internal static void EncryptedDataProcessingStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(166))
        return;
      TD.WriteEtwEvent(166, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool EncryptedDataProcessingSuccessIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(167);
      return false;
    }

    internal static void EncryptedDataProcessingSuccess(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(167))
        return;
      TD.WriteEtwEvent(167, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SignatureVerificationStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(168);
      return false;
    }

    internal static void SignatureVerificationStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(168))
        return;
      TD.WriteEtwEvent(168, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool SignatureVerificationSuccessIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(169);
      return false;
    }

    internal static void SignatureVerificationSuccess(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(169))
        return;
      TD.WriteEtwEvent(169, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WrappedKeyDecryptionStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(170);
      return false;
    }

    internal static void WrappedKeyDecryptionStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(170))
        return;
      TD.WriteEtwEvent(170, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WrappedKeyDecryptionSuccessIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(171);
      return false;
    }

    internal static void WrappedKeyDecryptionSuccess(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(171))
        return;
      TD.WriteEtwEvent(171, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineProcessInboundRequestStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(172);
      return false;
    }

    internal static void HttpPipelineProcessInboundRequestStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(172))
        return;
      TD.WriteEtwEvent(172, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineBeginProcessInboundRequestStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(173);
      return false;
    }

    internal static void HttpPipelineBeginProcessInboundRequestStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(173))
        return;
      TD.WriteEtwEvent(173, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineProcessInboundRequestStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(174);
      return false;
    }

    internal static void HttpPipelineProcessInboundRequestStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(174))
        return;
      TD.WriteEtwEvent(174, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineFaultedIsEnabled()
    {
      if (FxTrace.ShouldTraceWarning)
        return TD.IsEtwEventEnabled(175);
      return false;
    }

    internal static void HttpPipelineFaulted(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(175))
        return;
      TD.WriteEtwEvent(175, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineTimeoutExceptionIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(176);
      return false;
    }

    internal static void HttpPipelineTimeoutException(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(176))
        return;
      TD.WriteEtwEvent(176, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineProcessResponseStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(177);
      return false;
    }

    internal static void HttpPipelineProcessResponseStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(177))
        return;
      TD.WriteEtwEvent(177, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineBeginProcessResponseStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(178);
      return false;
    }

    internal static void HttpPipelineBeginProcessResponseStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(178))
        return;
      TD.WriteEtwEvent(178, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool HttpPipelineProcessResponseStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(179);
      return false;
    }

    internal static void HttpPipelineProcessResponseStop(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(179))
        return;
      TD.WriteEtwEvent(179, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionRequestSendStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(180);
      return false;
    }

    internal static void WebSocketConnectionRequestSendStart(EventTraceActivity eventTraceActivity, string remoteAddress)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(180))
        return;
      TD.WriteEtwEvent(180, eventTraceActivity, remoteAddress, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionRequestSendStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(181);
      return false;
    }

    internal static void WebSocketConnectionRequestSendStop(EventTraceActivity eventTraceActivity, int websocketId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(181))
        return;
      TD.WriteEtwEvent(181, eventTraceActivity, websocketId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionAcceptStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(182);
      return false;
    }

    internal static void WebSocketConnectionAcceptStart(EventTraceActivity eventTraceActivity)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(182))
        return;
      TD.WriteEtwEvent(182, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionAcceptedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(183);
      return false;
    }

    internal static void WebSocketConnectionAccepted(EventTraceActivity eventTraceActivity, int websocketId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(183))
        return;
      TD.WriteEtwEvent(183, eventTraceActivity, websocketId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionDeclinedIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(184);
      return false;
    }

    internal static void WebSocketConnectionDeclined(EventTraceActivity eventTraceActivity, string errorMessage)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(184))
        return;
      TD.WriteEtwEvent(184, eventTraceActivity, errorMessage, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionFailedIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(185);
      return false;
    }

    internal static void WebSocketConnectionFailed(EventTraceActivity eventTraceActivity, string errorMessage)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(185))
        return;
      TD.WriteEtwEvent(185, eventTraceActivity, errorMessage, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionAbortedIsEnabled()
    {
      if (FxTrace.ShouldTraceError)
        return TD.IsEtwEventEnabled(186);
      return false;
    }

    internal static void WebSocketConnectionAborted(EventTraceActivity eventTraceActivity, int websocketId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(186))
        return;
      TD.WriteEtwEvent(186, eventTraceActivity, websocketId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketAsyncWriteStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(187);
      return false;
    }

    internal static void WebSocketAsyncWriteStart(int websocketId, int byteCount, string remoteAddress)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(187))
        return;
      TD.WriteEtwEvent(187, (EventTraceActivity) null, websocketId, byteCount, remoteAddress, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketAsyncWriteStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(188);
      return false;
    }

    internal static void WebSocketAsyncWriteStop(int websocketId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(188))
        return;
      TD.WriteEtwEvent(188, (EventTraceActivity) null, websocketId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketAsyncReadStartIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(189);
      return false;
    }

    internal static void WebSocketAsyncReadStart(int websocketId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(189))
        return;
      TD.WriteEtwEvent(189, (EventTraceActivity) null, websocketId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketAsyncReadStopIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(190);
      return false;
    }

    internal static void WebSocketAsyncReadStop(int websocketId, int byteCount, string remoteAddress)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(190))
        return;
      TD.WriteEtwEvent(190, (EventTraceActivity) null, websocketId, byteCount, remoteAddress, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketCloseSentIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(191);
      return false;
    }

    internal static void WebSocketCloseSent(int websocketId, string remoteAddress, string closeStatus)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(191))
        return;
      TD.WriteEtwEvent(191, (EventTraceActivity) null, websocketId, remoteAddress, closeStatus, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketCloseOutputSentIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(192);
      return false;
    }

    internal static void WebSocketCloseOutputSent(int websocketId, string remoteAddress, string closeStatus)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(192))
        return;
      TD.WriteEtwEvent(192, (EventTraceActivity) null, websocketId, remoteAddress, closeStatus, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketConnectionClosedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(193);
      return false;
    }

    internal static void WebSocketConnectionClosed(int websocketId)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(193))
        return;
      TD.WriteEtwEvent(193, (EventTraceActivity) null, websocketId, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketCloseStatusReceivedIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(194);
      return false;
    }

    internal static void WebSocketCloseStatusReceived(int websocketId, string closeStatus)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(194))
        return;
      TD.WriteEtwEvent(194, (EventTraceActivity) null, websocketId, closeStatus, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketUseVersionFromClientWebSocketFactoryIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(195);
      return false;
    }

    internal static void WebSocketUseVersionFromClientWebSocketFactory(EventTraceActivity eventTraceActivity, string clientWebSocketFactoryType)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(195))
        return;
      TD.WriteEtwEvent(195, eventTraceActivity, clientWebSocketFactoryType, serializedPayload.AppDomainFriendlyName);
    }

    internal static bool WebSocketCreateClientWebSocketWithFactoryIsEnabled()
    {
      if (FxTrace.ShouldTraceVerbose)
        return TD.IsEtwEventEnabled(196);
      return false;
    }

    internal static void WebSocketCreateClientWebSocketWithFactory(EventTraceActivity eventTraceActivity, string clientWebSocketFactoryType)
    {
      TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      if (!TD.IsEtwEventEnabled(196))
        return;
      TD.WriteEtwEvent(196, eventTraceActivity, clientWebSocketFactoryType, serializedPayload.AppDomainFriendlyName);
    }

    [SecuritySafeCritical]
    private static void CreateEventDescriptors()
    {
      EventDescriptor[] eventDescriptors = new EventDescriptor[197]
      {
        new EventDescriptor(217, (byte) 0, (byte) 19, (byte) 4, (byte) 20, 2514, 1152921504607371268L),
        new EventDescriptor(201, (byte) 0, (byte) 19, (byte) 4, (byte) 16, 2514, 1152921504607371268L),
        new EventDescriptor(202, (byte) 0, (byte) 18, (byte) 4, (byte) 17, 2514, 2305843009214218244L),
        new EventDescriptor(203, (byte) 0, (byte) 18, (byte) 4, (byte) 19, 2514, 2305843009214218244L),
        new EventDescriptor(204, (byte) 0, (byte) 18, (byte) 4, (byte) 18, 2514, 2305843009214218244L),
        new EventDescriptor(205, (byte) 0, (byte) 18, (byte) 4, (byte) 53, 2533, 2305843009214218244L),
        new EventDescriptor(206, (byte) 0, (byte) 18, (byte) 4, (byte) 0, 0, 2305843009214218244L),
        new EventDescriptor(207, (byte) 0, (byte) 18, (byte) 4, (byte) 0, 0, 2305843009214218244L),
        new EventDescriptor(208, (byte) 0, (byte) 18, (byte) 4, (byte) 51, 2533, 2305843009214218244L),
        new EventDescriptor(209, (byte) 0, (byte) 18, (byte) 4, (byte) 52, 2533, 2305843009214218244L),
        new EventDescriptor(210, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 0, 2305843009218805764L),
        new EventDescriptor(211, (byte) 0, (byte) 18, (byte) 4, (byte) 56, 2533, 2305843009214218244L),
        new EventDescriptor(212, (byte) 0, (byte) 18, (byte) 4, (byte) 55, 2533, 2305843009214218244L),
        new EventDescriptor(214, (byte) 0, (byte) 18, (byte) 4, (byte) 54, 2533, 2305843009214611460L),
        new EventDescriptor(215, (byte) 0, (byte) 18, (byte) 4, (byte) 2, 2599, 2305843009214219264L),
        new EventDescriptor(216, (byte) 0, (byte) 18, (byte) 4, (byte) 2, 2600, 2305843009214219264L),
        new EventDescriptor(451, (byte) 0, (byte) 18, (byte) 4, (byte) 0, 0, 2305843009214218272L),
        new EventDescriptor(452, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 0, 2305843009214218272L),
        new EventDescriptor(4600, (byte) 0, (byte) 19, (byte) 3, (byte) 0, 0, 1152921504606847008L),
        new EventDescriptor(404, (byte) 0, (byte) 18, (byte) 4, (byte) 7, 2588, 2305843009214218244L),
        new EventDescriptor(402, (byte) 0, (byte) 18, (byte) 4, (byte) 1, 2588, 2305843009214218244L),
        new EventDescriptor(401, (byte) 0, (byte) 18, (byte) 4, (byte) 2, 2588, 2305843009214218244L),
        new EventDescriptor(403, (byte) 0, (byte) 18, (byte) 4, (byte) 8, 2588, 2305843009214218244L),
        new EventDescriptor(218, (byte) 0, (byte) 18, (byte) 4, (byte) 2, 2576, 2305843009214218244L),
        new EventDescriptor(219, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2533, 2305843009214611460L),
        new EventDescriptor(222, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2533, 2305843009214611460L),
        new EventDescriptor(223, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2533, 2305843009214611460L),
        new EventDescriptor(224, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2533, 2305843009218805764L),
        new EventDescriptor(221, (byte) 0, (byte) 18, (byte) 4, (byte) 0, 0, 2305843009214350336L),
        new EventDescriptor(220, (byte) 0, (byte) 18, (byte) 4, (byte) 0, 0, 2305843009214350336L),
        new EventDescriptor(509, (byte) 0, (byte) 18, (byte) 4, (byte) 1, 2583, 2305843009213693953L),
        new EventDescriptor(510, (byte) 0, (byte) 18, (byte) 4, (byte) 2, 2583, 2305843009213693953L),
        new EventDescriptor(701, (byte) 0, (byte) 18, (byte) 4, (byte) 1, 2577, 2305843009213693956L),
        new EventDescriptor(702, (byte) 0, (byte) 18, (byte) 4, (byte) 2, 2577, 2305843009213693956L),
        new EventDescriptor(703, (byte) 0, (byte) 18, (byte) 4, (byte) 1, 2576, 2305843009213693956L),
        new EventDescriptor(704, (byte) 0, (byte) 18, (byte) 4, (byte) 1, 2576, 2305843009213693956L),
        new EventDescriptor(706, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2600, 1152921504606847232L),
        new EventDescriptor(707, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2600, 1152921504606847232L),
        new EventDescriptor(708, (byte) 0, (byte) 18, (byte) 5, (byte) 1, 2599, 2305843009213694208L),
        new EventDescriptor(709, (byte) 0, (byte) 18, (byte) 4, (byte) 49, 2533, 2305843009213693956L),
        new EventDescriptor(710, (byte) 0, (byte) 18, (byte) 5, (byte) 128, 2599, 2305843009213693956L),
        new EventDescriptor(711, (byte) 0, (byte) 18, (byte) 5, (byte) 48, 2533, 2305843009213693956L),
        new EventDescriptor(712, (byte) 0, (byte) 18, (byte) 4, (byte) 50, 2533, 2305843009213693956L),
        new EventDescriptor(715, (byte) 0, (byte) 18, (byte) 4, (byte) 14, 2514, 2305843009213693956L),
        new EventDescriptor(716, (byte) 0, (byte) 18, (byte) 4, (byte) 15, 2514, 2305843009213693956L),
        new EventDescriptor(717, (byte) 0, (byte) 18, (byte) 4, (byte) 1, 2600, 2305843009213694208L),
        new EventDescriptor(3301, (byte) 0, (byte) 19, (byte) 3, (byte) 0, 0, 1152921504606851072L),
        new EventDescriptor(3303, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 0, 1152921504606851072L),
        new EventDescriptor(3300, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 0, 2305843009213698048L),
        new EventDescriptor(3302, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2533, 2305843009213693956L),
        new EventDescriptor(3305, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 2511, 1152921504606846980L),
        new EventDescriptor(3306, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 2511, 1152921504606846980L),
        new EventDescriptor(3307, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 2511, 1152921504606846980L),
        new EventDescriptor(3308, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 2511, 1152921504606846980L),
        new EventDescriptor(3309, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 0, 1152921504606846980L),
        new EventDescriptor(3310, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2533, 2305843009213693956L),
        new EventDescriptor(3311, (byte) 0, (byte) 18, (byte) 4, (byte) 2, 2533, 2305843009213693956L),
        new EventDescriptor(3312, (byte) 0, (byte) 19, (byte) 4, (byte) 2, 2555, 1152921504606851072L),
        new EventDescriptor(3313, (byte) 0, (byte) 19, (byte) 4, (byte) 2, 2556, 1152921504606851072L),
        new EventDescriptor(3314, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2595, 2305843009213693956L),
        new EventDescriptor(3319, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2521, 1152921504606847488L),
        new EventDescriptor(3320, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2521, 1152921504606847488L),
        new EventDescriptor(3321, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2522, 1152921504606851072L),
        new EventDescriptor(3322, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2540, 1152921504606846980L),
        new EventDescriptor(3323, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2540, 1152921504606846980L),
        new EventDescriptor(3324, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2541, 1152921504606846980L),
        new EventDescriptor(3325, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2541, 1152921504606846980L),
        new EventDescriptor(3326, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2542, 1152921504606846980L),
        new EventDescriptor(3327, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2542, 1152921504606846980L),
        new EventDescriptor(3328, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2539, 1152921504606846980L),
        new EventDescriptor(3329, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2539, 1152921504606846980L),
        new EventDescriptor(3330, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2573, 1152921504606846992L),
        new EventDescriptor(3331, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2573, 1152921504606846992L),
        new EventDescriptor(3332, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2571, 1152921504606846992L),
        new EventDescriptor(3333, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2571, 1152921504606846992L),
        new EventDescriptor(3334, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2574, 1152921504606846996L),
        new EventDescriptor(3335, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2584, 1152921504606846980L),
        new EventDescriptor(3336, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2584, 1152921504606846980L),
        new EventDescriptor(3337, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2513, 1152921504606851072L),
        new EventDescriptor(3338, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2513, 1152921504606851072L),
        new EventDescriptor(3339, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2512, 1152921504606846980L),
        new EventDescriptor(3340, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2521, 1152921504606851072L),
        new EventDescriptor(3341, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2521, 1152921504606851072L),
        new EventDescriptor(3342, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2519, 1152921504606851072L),
        new EventDescriptor(3343, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2519, 1152921504606851072L),
        new EventDescriptor(3345, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2519, 1152921504606851072L),
        new EventDescriptor(3346, (byte) 0, (byte) 19, (byte) 2, (byte) 0, 2519, 1152921504606851072L),
        new EventDescriptor(3347, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2521, 1152921504606847488L),
        new EventDescriptor(3348, (byte) 0, (byte) 18, (byte) 1, (byte) 0, 2582, 2305843009213694464L),
        new EventDescriptor(3349, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2552, 1152921504606851072L),
        new EventDescriptor(3350, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2552, 1152921504606851072L),
        new EventDescriptor(3351, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(3352, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2519, 2305843009213694464L),
        new EventDescriptor(3353, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2519, 2305843009213694464L),
        new EventDescriptor(3354, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2573, 1152921504606846992L),
        new EventDescriptor(3355, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2573, 2305843009213693968L),
        new EventDescriptor(3356, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2574, 1152921504606846992L),
        new EventDescriptor(3357, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2574, 2305843009213693968L),
        new EventDescriptor(3358, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2501, 1152921504606849024L),
        new EventDescriptor(3359, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2572, 1152921504606846992L),
        new EventDescriptor(3360, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2572, 2305843009213693968L),
        new EventDescriptor(3361, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2599, 2305843009213694208L),
        new EventDescriptor(3362, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2600, 2305843009213694208L),
        new EventDescriptor(3363, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2574, 2305843009213694208L),
        new EventDescriptor(3364, (byte) 0, (byte) 18, (byte) 5, (byte) 1, 2502, 2305843009213696000L),
        new EventDescriptor(3365, (byte) 0, (byte) 18, (byte) 5, (byte) 2, 2502, 2305843009213696000L),
        new EventDescriptor(3366, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2502, 2305843009213696000L),
        new EventDescriptor(3367, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2586, 2305843009213698048L),
        new EventDescriptor(3368, (byte) 0, (byte) 18, (byte) 5, (byte) 115, 2587, 2305843009213693968L),
        new EventDescriptor(3369, (byte) 0, (byte) 18, (byte) 5, (byte) 114, 2587, 2305843009213693968L),
        new EventDescriptor(3370, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2556, 1152921504606851072L),
        new EventDescriptor(3371, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2556, 1152921504606851072L),
        new EventDescriptor(3372, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2556, 1152921504606851072L),
        new EventDescriptor(3373, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2555, 1152921504606851072L),
        new EventDescriptor(3374, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2555, 1152921504606851072L),
        new EventDescriptor(3375, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2555, 1152921504606851072L),
        new EventDescriptor(3376, (byte) 0, (byte) 19, (byte) 4, (byte) 1, 2599, 1152921504606847232L),
        new EventDescriptor(3377, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2599, 1152921504606847488L),
        new EventDescriptor(3378, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2599, 1152921504606847488L),
        new EventDescriptor(3379, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2600, 1152921504606847488L),
        new EventDescriptor(3380, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2600, 1152921504606847488L),
        new EventDescriptor(3381, (byte) 0, (byte) 19, (byte) 5, (byte) 79, 2561, 1152921504606851072L),
        new EventDescriptor(3382, (byte) 0, (byte) 19, (byte) 4, (byte) 78, 2561, 1152921504606851072L),
        new EventDescriptor(3383, (byte) 0, (byte) 19, (byte) 4, (byte) 77, 2561, 1152921504606851072L),
        new EventDescriptor(3384, (byte) 0, (byte) 18, (byte) 5, (byte) 115, 2587, 2305843009213693968L),
        new EventDescriptor(3385, (byte) 0, (byte) 18, (byte) 5, (byte) 114, 2587, 2305843009213693968L),
        new EventDescriptor(3386, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2520, 2305843009213694464L),
        new EventDescriptor(3388, (byte) 0, (byte) 18, (byte) 5, (byte) 1, 2599, 2305843009213694208L),
        new EventDescriptor(3389, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2515, 1152921504606851072L),
        new EventDescriptor(3390, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2515, 1152921504606851072L),
        new EventDescriptor(3391, (byte) 0, (byte) 18, (byte) 3, (byte) 1, 2599, 2305843009213694208L),
        new EventDescriptor(3392, (byte) 0, (byte) 19, (byte) 4, (byte) 57, 2533, 1152921504606846980L),
        new EventDescriptor(3393, (byte) 0, (byte) 19, (byte) 4, (byte) 2, 2555, 1152921504606851072L),
        new EventDescriptor(3394, (byte) 0, (byte) 19, (byte) 4, (byte) 2, 2556, 1152921504606851072L),
        new EventDescriptor(3395, (byte) 0, (byte) 19, (byte) 4, (byte) 2, 2556, 1152921504606851072L),
        new EventDescriptor(3396, (byte) 0, (byte) 19, (byte) 4, (byte) 1, 2600, 1152921504606851072L),
        new EventDescriptor(3397, (byte) 0, (byte) 19, (byte) 4, (byte) 2, 2600, 1152921504606851072L),
        new EventDescriptor(1400, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1401, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1402, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1403, (byte) 0, (byte) 18, (byte) 4, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1405, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1406, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1407, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1409, (byte) 0, (byte) 18, (byte) 4, (byte) 0, 2596, 2305843009213693956L),
        new EventDescriptor(1416, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1417, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1418, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 2560, 1152921504611041280L),
        new EventDescriptor(1419, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 2560, 1152921504611041280L),
        new EventDescriptor(1422, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1423, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2560, 1152921504611041280L),
        new EventDescriptor(1424, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2560, 1152921504611041280L),
        new EventDescriptor(1430, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1433, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1438, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1432, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1431, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1439, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 2560, 1152921504611041280L),
        new EventDescriptor(1441, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1442, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2558, 2305843009217888256L),
        new EventDescriptor(1443, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2558, 2305843009217888256L),
        new EventDescriptor(1445, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1446, (byte) 0, (byte) 18, (byte) 5, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(1451, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2560, 2305843009217888256L),
        new EventDescriptor(3398, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2552, 1152921504606851072L),
        new EventDescriptor(3399, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2552, 1152921504606851072L),
        new EventDescriptor(3405, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2615, 1152921504606846992L),
        new EventDescriptor(3406, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2615, 1152921504606846992L),
        new EventDescriptor(3401, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2611, 1152921504606846992L),
        new EventDescriptor(3402, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2611, 1152921504606846992L),
        new EventDescriptor(3403, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2614, 1152921504606846992L),
        new EventDescriptor(3404, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2614, 1152921504606846992L),
        new EventDescriptor(3407, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2599, 1152921504606847232L),
        new EventDescriptor(3408, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2599, 1152921504606847232L),
        new EventDescriptor(3409, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2599, 1152921504606847232L),
        new EventDescriptor(3410, (byte) 0, (byte) 18, (byte) 3, (byte) 0, 2599, 2305843009213694208L),
        new EventDescriptor(3411, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2519, 2305843009213694208L),
        new EventDescriptor(3412, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2600, 1152921504606847232L),
        new EventDescriptor(3413, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2600, 1152921504606847232L),
        new EventDescriptor(3414, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2600, 1152921504606847232L),
        new EventDescriptor(3415, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2519, 1152921504606847232L),
        new EventDescriptor(3416, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2519, 1152921504606847232L),
        new EventDescriptor(3417, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2519, 1152921504606847232L),
        new EventDescriptor(3418, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2519, 1152921504606847232L),
        new EventDescriptor(3419, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2519, 2305843009213694208L),
        new EventDescriptor(3420, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2519, 2305843009213694208L),
        new EventDescriptor(3421, (byte) 0, (byte) 18, (byte) 2, (byte) 0, 2519, 2305843009213694208L),
        new EventDescriptor(3422, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2600, 1152921504606847232L),
        new EventDescriptor(3423, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2600, 1152921504606847232L),
        new EventDescriptor(3424, (byte) 0, (byte) 19, (byte) 5, (byte) 1, 2599, 1152921504606847232L),
        new EventDescriptor(3425, (byte) 0, (byte) 19, (byte) 5, (byte) 2, 2599, 1152921504606847232L),
        new EventDescriptor(3426, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2519, 1152921504606847232L),
        new EventDescriptor(3427, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2519, 1152921504606847232L),
        new EventDescriptor(3428, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2519, 1152921504606847232L),
        new EventDescriptor(3429, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2519, 1152921504606847232L),
        new EventDescriptor(3430, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2519, 1152921504606847232L),
        new EventDescriptor(3431, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 2519, 1152921504606847232L)
      };
      FxTrace.UpdateEventDefinitions(eventDescriptors, new List<ushort>(120)
      {
        (ushort) 201,
        (ushort) 202,
        (ushort) 203,
        (ushort) 204,
        (ushort) 205,
        (ushort) 208,
        (ushort) 209,
        (ushort) 211,
        (ushort) 212,
        (ushort) 214,
        (ushort) 215,
        (ushort) 216,
        (ushort) 217,
        (ushort) 218,
        (ushort) 219,
        (ushort) 220,
        (ushort) 221,
        (ushort) 222,
        (ushort) 223,
        (ushort) 509,
        (ushort) 510,
        (ushort) 701,
        (ushort) 702,
        (ushort) 703,
        (ushort) 704,
        (ushort) 706,
        (ushort) 707,
        (ushort) 708,
        (ushort) 709,
        (ushort) 710,
        (ushort) 711,
        (ushort) 712,
        (ushort) 715,
        (ushort) 716,
        (ushort) 717,
        (ushort) 3300,
        (ushort) 3301,
        (ushort) 3302,
        (ushort) 3303,
        (ushort) 3309,
        (ushort) 3310,
        (ushort) 3311,
        (ushort) 3312,
        (ushort) 3313,
        (ushort) 3319,
        (ushort) 3320,
        (ushort) 3322,
        (ushort) 3323,
        (ushort) 3324,
        (ushort) 3325,
        (ushort) 3326,
        (ushort) 3327,
        (ushort) 3328,
        (ushort) 3329,
        (ushort) 3330,
        (ushort) 3331,
        (ushort) 3332,
        (ushort) 3333,
        (ushort) 3334,
        (ushort) 3335,
        (ushort) 3336,
        (ushort) 3337,
        (ushort) 3338,
        (ushort) 3340,
        (ushort) 3341,
        (ushort) 3342,
        (ushort) 3343,
        (ushort) 3347,
        (ushort) 3348,
        (ushort) 3349,
        (ushort) 3350,
        (ushort) 3354,
        (ushort) 3355,
        (ushort) 3356,
        (ushort) 3357,
        (ushort) 3358,
        (ushort) 3359,
        (ushort) 3360,
        (ushort) 3361,
        (ushort) 3362,
        (ushort) 3363,
        (ushort) 3367,
        (ushort) 3369,
        (ushort) 3370,
        (ushort) 3371,
        (ushort) 3372,
        (ushort) 3376,
        (ushort) 3385,
        (ushort) 3388,
        (ushort) 3389,
        (ushort) 3390,
        (ushort) 3392,
        (ushort) 3393,
        (ushort) 3394,
        (ushort) 3395,
        (ushort) 3396,
        (ushort) 3397,
        (ushort) 3401,
        (ushort) 3402,
        (ushort) 3403,
        (ushort) 3404,
        (ushort) 3405,
        (ushort) 3406,
        (ushort) 3407,
        (ushort) 3408,
        (ushort) 3409,
        (ushort) 3410,
        (ushort) 3411,
        (ushort) 3412,
        (ushort) 3413,
        (ushort) 3414,
        (ushort) 3415,
        (ushort) 3416,
        (ushort) 3417,
        (ushort) 3418,
        (ushort) 3419,
        (ushort) 3420,
        (ushort) 3421,
        (ushort) 3430,
        (ushort) 3431
      }.ToArray());
      TD.eventDescriptors = eventDescriptors;
    }

    private static void EnsureEventDescriptors()
    {
      if (TD.eventDescriptorsCreated)
        return;
      Monitor.Enter(TD.syncLock);
      try
      {
        if (TD.eventDescriptorsCreated)
          return;
        TD.CreateEventDescriptors();
        TD.eventDescriptorsCreated = true;
      }
      finally
      {
        Monitor.Exit(TD.syncLock);
      }
    }

    private static bool IsEtwEventEnabled(int eventIndex)
    {
      if (!FxTrace.Trace.IsEtwProviderEnabled)
        return false;
      TD.EnsureEventDescriptors();
      return FxTrace.IsEventEnabled(eventIndex);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2, string eventParam3)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2, string eventParam3, string eventParam4)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3, eventParam4);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, bool eventParam2, string eventParam3, string eventParam4, string eventParam5)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, (object) eventParam1, (object) eventParam2, (object) eventParam3, (object) eventParam4, (object) eventParam5);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, long eventParam2, string eventParam3, string eventParam4)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3, eventParam4);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2, string eventParam3, string eventParam4, string eventParam5)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3, eventParam4, eventParam5);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, Guid eventParam1, string eventParam2, string eventParam3)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, int eventParam1, int eventParam2, string eventParam3, string eventParam4)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, (object) eventParam1, (object) eventParam2, (object) eventParam3, (object) eventParam4);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, int eventParam1, string eventParam2, string eventParam3)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, (object) eventParam1, (object) eventParam2, (object) eventParam3);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, int eventParam1, int eventParam2, string eventParam3)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, (object) eventParam1, (object) eventParam2, (object) eventParam3);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, int eventParam2, string eventParam3)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, (object) eventParam1, (object) eventParam2, (object) eventParam3);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, int eventParam1, string eventParam2)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, new object[2]
      {
        (object) eventParam1,
        (object) eventParam2
      });
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, Guid eventParam2, string eventParam3)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, (object) eventParam1, (object) eventParam2, (object) eventParam3);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, int eventParam1, string eventParam2, string eventParam3, string eventParam4)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, (object) eventParam1, (object) eventParam2, (object) eventParam3, (object) eventParam4);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwTransferEvent(int eventIndex, EventTraceActivity eventParam0, Guid eventParam1, string eventParam2, string eventParam3, string eventParam4, string eventParam5, string eventParam6)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteTransferEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, (object) eventParam2, (object) eventParam3, (object) eventParam4, (object) eventParam5, (object) eventParam6);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwTransferEvent(int eventIndex, EventTraceActivity eventParam0, Guid eventParam1, string eventParam2, string eventParam3, string eventParam4)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteTransferEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, (object) eventParam2, (object) eventParam3, (object) eventParam4);
    }

    [SecuritySafeCritical]
    private static bool WriteEtwTransferEvent(int eventIndex, EventTraceActivity eventParam0, Guid eventParam1, string eventParam2, string eventParam3)
    {
      TD.EnsureEventDescriptors();
      return FxTrace.Trace.EtwProvider.WriteTransferEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3);
    }

    [SecuritySafeCritical]
    private static void WriteTraceSource(int eventIndex, string description, TracePayload payload)
    {
      TD.EnsureEventDescriptors();
      FxTrace.Trace.WriteTraceSource(ref TD.eventDescriptors[eventIndex], description, payload);
    }*/
  }
}
