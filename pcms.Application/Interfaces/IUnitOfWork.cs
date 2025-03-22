using pcms.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Member> Members { get; }
        IGenericRepository<Contribution> Contributions { get; }
        Task<int> CompleteAsync();
    }
}
