// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.OperationWithTimeoutComposer
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;

namespace System.ServiceModel.Channels
{
  public static class OperationWithTimeoutComposer
  {
    public static IAsyncResult BeginComposeAsyncOperations(TimeSpan timeout, OperationWithTimeoutBeginCallback[] beginOperations, OperationEndCallback[] endOperations, AsyncCallback callback, object state)
    {
      return (IAsyncResult) new OperationWithTimeoutComposer.ComposedAsyncResult(timeout, beginOperations, endOperations, callback, state);
    }

    public static void EndComposeAsyncOperations(IAsyncResult result)
    {
      OperationWithTimeoutComposer.ComposedAsyncResult.End(result);
    }

    public static TimeSpan RemainingTime(IAsyncResult result)
    {
      return ((OperationWithTimeoutComposer.ComposedAsyncResult) result).RemainingTime();
    }

    private class ComposedAsyncResult : AsyncResult
    {
      private static AsyncCallback onOperationCompleted = Fx.ThunkCallback(new AsyncCallback(OperationWithTimeoutComposer.ComposedAsyncResult.OnOperationCompletedStatic));
      private bool completedSynchronously = true;
      private OperationWithTimeoutBeginCallback[] beginOperations;
      private int currentOperation;
      private OperationEndCallback[] endOperations;
      private TimeoutHelper timeoutHelper;

      internal ComposedAsyncResult(TimeSpan timeout, OperationWithTimeoutBeginCallback[] beginOperations, OperationEndCallback[] endOperations, AsyncCallback callback, object state)
        : base(callback, state)
      {
        this.timeoutHelper = new TimeoutHelper(timeout);
        this.beginOperations = beginOperations;
        this.endOperations = endOperations;
        this.SkipToNextOperation();
        if (this.currentOperation < this.beginOperations.Length)
        {
          IAsyncResult asyncResult = this.beginOperations[this.currentOperation](this.RemainingTime(), OperationWithTimeoutComposer.ComposedAsyncResult.onOperationCompleted, (object) this);
        }
        else
          this.Complete(this.completedSynchronously);
      }

      public TimeSpan RemainingTime()
      {
        return this.timeoutHelper.RemainingTime();
      }

      internal static void End(IAsyncResult result)
      {
        AsyncResult.End<OperationWithTimeoutComposer.ComposedAsyncResult>(result);
      }

      private void OnOperationCompleted(IAsyncResult result)
      {
        this.completedSynchronously = this.completedSynchronously && result.CompletedSynchronously;
        Exception exception = (Exception) null;
        try
        {
          this.endOperations[this.currentOperation](result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            exception = ex;
        }
        if (exception != null)
        {
          this.Complete(this.completedSynchronously, exception);
        }
        else
        {
          this.currentOperation = this.currentOperation + 1;
          this.SkipToNextOperation();
          if (this.currentOperation < this.beginOperations.Length)
          {
            try
            {
              IAsyncResult asyncResult = this.beginOperations[this.currentOperation](this.RemainingTime(), OperationWithTimeoutComposer.ComposedAsyncResult.onOperationCompleted, (object) this);
            }
            catch (Exception ex)
            {
              if (Fx.IsFatal(ex))
                throw;
              else
                exception = ex;
            }
            if (exception == null)
              return;
            this.Complete(this.completedSynchronously, exception);
          }
          else
            this.Complete(this.completedSynchronously);
        }
      }

      private static void OnOperationCompletedStatic(IAsyncResult result)
      {
        ((OperationWithTimeoutComposer.ComposedAsyncResult) result.AsyncState).OnOperationCompleted(result);
      }

      private void SkipToNextOperation()
      {
        while (this.currentOperation < this.beginOperations.Length && this.beginOperations[this.currentOperation] == null)
          this.currentOperation = this.currentOperation + 1;
      }
    }
  }
}
