// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SessionActionFilter
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Security
{
  internal class SessionActionFilter : HeaderFilter
  {
    private SecurityStandardsManager standardsManager;
    private string[] actions;

    public SessionActionFilter(SecurityStandardsManager standardsManager, params string[] actions)
    {
      this.actions = actions;
      this.standardsManager = standardsManager;
    }

    public override bool Match(Message message)
    {
      for (int index = 0; index < this.actions.Length; ++index)
      {
        if (message.Headers.Action == this.actions[index])
          return this.standardsManager.DoesMessageContainSecurityHeader(message);
      }
      return false;
    }
  }
}
