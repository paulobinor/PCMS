using pcms.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Domain.Interfaces
{
    public interface IUnitOfWorkRepo : IDisposable
    {
        IMemberServiceRepo Members { get; }
        IContributionRepo Contributions { get; }
      //  IGenericRepository<Contribution> Contributions { get; }
        Task<int> CompleteAsync();
    }
}
