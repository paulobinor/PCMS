using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using pcms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Application.Services
{
    public class HangfireFailedJobListener : IServerFilter
    {
        private readonly IFailedTransactionHandler _failedTransactionHandler;

        public HangfireFailedJobListener(IFailedTransactionHandler failedTransactionHandler)
        {
            _failedTransactionHandler = failedTransactionHandler;
        }

        public void OnPerforming(PerformingContext filterContext) { }

        public void OnPerformed(PerformedContext filterContext) { }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context.NewState is FailedState failedState)
            {
                string jobId = context.BackgroundJob?.Id;
                string errorMessage = failedState.Exception?.Message;

                Task.Run(async () => await _failedTransactionHandler.HandleFailedTransactionAsync(jobId, errorMessage));
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
    }
}
