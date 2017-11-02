// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.ServiceModelAppSettings
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Specialized;
using System.Configuration;

namespace System.ServiceModel
{
  internal static class ServiceModelAppSettings
  {
    private static volatile bool settingsInitalized = false;
    private static object appSettingsLock = new object();
    internal const string HttpTransportPerFactoryConnectionPoolString = "wcf:httpTransportBinding:useUniqueConnectionPoolPerFactory";
    internal const string EnsureUniquePerformanceCounterInstanceNamesString = "wcf:ensureUniquePerformanceCounterInstanceNames";
    internal const string UseConfiguredTransportSecurityHeaderLayoutString = "wcf:useConfiguredTransportSecurityHeaderLayout";
    internal const string UseBestMatchNamedPipeUriString = "wcf:useBestMatchNamedPipeUri";
    internal const string DisableOperationContextAsyncFlowString = "wcf:disableOperationContextAsyncFlow";
    private const bool DefaultHttpTransportPerFactoryConnectionPool = false;
    private const bool DefaultEnsureUniquePerformanceCounterInstanceNames = false;
    private const bool DefaultUseConfiguredTransportSecurityHeaderLayout = false;
    private const bool DefaultUseBestMatchNamedPipeUri = false;
    private const bool DefaultDisableOperationContextAsyncFlow = true;
    private static bool httpTransportPerFactoryConnectionPool;
    private static bool ensureUniquePerformanceCounterInstanceNames;
    private static bool useConfiguredTransportSecurityHeaderLayout;
    private static bool useBestMatchNamedPipeUri;
    private static bool disableOperationContextAsyncFlow;

    internal static bool HttpTransportPerFactoryConnectionPool
    {
      get
      {
        ServiceModelAppSettings.EnsureSettingsLoaded();
        return ServiceModelAppSettings.httpTransportPerFactoryConnectionPool;
      }
    }

    internal static bool EnsureUniquePerformanceCounterInstanceNames
    {
      get
      {
        ServiceModelAppSettings.EnsureSettingsLoaded();
        return ServiceModelAppSettings.ensureUniquePerformanceCounterInstanceNames;
      }
    }

    internal static bool DisableOperationContextAsyncFlow
    {
      get
      {
        ServiceModelAppSettings.EnsureSettingsLoaded();
        return ServiceModelAppSettings.disableOperationContextAsyncFlow;
      }
    }

    internal static bool UseConfiguredTransportSecurityHeaderLayout
    {
      get
      {
        ServiceModelAppSettings.EnsureSettingsLoaded();
        return ServiceModelAppSettings.useConfiguredTransportSecurityHeaderLayout;
      }
    }

    internal static bool UseBestMatchNamedPipeUri
    {
      get
      {
        ServiceModelAppSettings.EnsureSettingsLoaded();
        return ServiceModelAppSettings.useBestMatchNamedPipeUri;
      }
    }

    private static void EnsureSettingsLoaded()
    {
      if (ServiceModelAppSettings.settingsInitalized)
        return;
      lock (ServiceModelAppSettings.appSettingsLock)
      {
        if (ServiceModelAppSettings.settingsInitalized)
          return;
        NameValueCollection nameValueCollection = (NameValueCollection) null;
        Console.WriteLine("TODO - skipping ConfigurationManager due to missing dependency");
        /*try
        {
          nameValueCollection = ConfigurationManager.AppSettings;
        }
        catch (ConfigurationErrorsException ex)
        {
        }
        finally
        {
          if (nameValueCollection == null || !bool.TryParse(nameValueCollection["wcf:httpTransportBinding:useUniqueConnectionPoolPerFactory"], out ServiceModelAppSettings.httpTransportPerFactoryConnectionPool))
            ServiceModelAppSettings.httpTransportPerFactoryConnectionPool = false;
          if (nameValueCollection == null || !bool.TryParse(nameValueCollection["wcf:ensureUniquePerformanceCounterInstanceNames"], out ServiceModelAppSettings.ensureUniquePerformanceCounterInstanceNames))
            ServiceModelAppSettings.ensureUniquePerformanceCounterInstanceNames = false;
          if (nameValueCollection == null || !bool.TryParse(nameValueCollection["wcf:disableOperationContextAsyncFlow"], out ServiceModelAppSettings.disableOperationContextAsyncFlow))
            ServiceModelAppSettings.disableOperationContextAsyncFlow = true;
          if (nameValueCollection == null || !bool.TryParse(nameValueCollection["wcf:useConfiguredTransportSecurityHeaderLayout"], out ServiceModelAppSettings.useConfiguredTransportSecurityHeaderLayout))
            ServiceModelAppSettings.useConfiguredTransportSecurityHeaderLayout = false;
          if (nameValueCollection == null || !bool.TryParse(nameValueCollection["wcf:useBestMatchNamedPipeUri"], out ServiceModelAppSettings.useBestMatchNamedPipeUri))
            ServiceModelAppSettings.useBestMatchNamedPipeUri = false;
          ServiceModelAppSettings.settingsInitalized = true;
        }*/
      }
    }
  }
}
