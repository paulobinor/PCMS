using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Application.Interfaces
{
    public interface IFailedTransactionHandler
    {
        Task HandleFailedTransactionAsync(string jobId, string errorMessage);
    }
}
