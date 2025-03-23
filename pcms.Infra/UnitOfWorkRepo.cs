using pcms.Domain.Interfaces;

namespace pcms.Infra
{
    public class UnitOfWorkRepo : IUnitOfWorkRepo
    {
        private readonly AppDBContext _context;
        public IMemberServiceRepo Members { get; }
        public IContributionRepo Contributions { get; }
        public UnitOfWorkRepo(AppDBContext context)
        {
            _context = context;
            Members = new MemberServiceRepo(context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
