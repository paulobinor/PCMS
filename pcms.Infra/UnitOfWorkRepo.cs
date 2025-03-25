using Microsoft.Extensions.Logging;
using pcms.Domain.Interfaces;

namespace pcms.Infra
{
    public class UnitOfWorkRepo : IUnitOfWorkRepo
    {
        private readonly AppDBContext _context;
        public IMemberServiceRepo Members { get; }
        public IContributionRepo Contributions { get; }
       // private readonly ILogger<MemberServiceRepo> _logger;
        public UnitOfWorkRepo(AppDBContext context, ILogger<MemberServiceRepo> logger)
        {
            _context = context;
           // _logger = logger;
            Members = new MemberServiceRepo(context, logger);
            Contributions = new ContributionRepo(context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
