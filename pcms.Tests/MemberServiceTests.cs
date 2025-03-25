using Moq;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Application.Services;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;
using pcms.Infra;

namespace pcms.Tests
{
    public class MemberServiceTests
    {
        private Mock<IValidationService> _validationService;
        private Mock<IUnitOfWorkRepo> _unitOfWorkMock;
        private Mock<ICacheService>  _cacheServiceMock;
        private Mock<IMemberServiceRepo> _memberServiceRepo;
        private IMemberService _memberService;
        private Mock<IGenericRepository<Member>> _memberRepositoryMock;

        public MemberServiceTests()
        {

            _validationService = new Mock<IValidationService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _memberServiceRepo = new Mock<IMemberServiceRepo>();
            _unitOfWorkMock = new Mock<IUnitOfWorkRepo>();
            _memberRepositoryMock = new Mock<IGenericRepository<Member>>();
            _unitOfWorkMock.Setup(u => u.Members).Returns(_memberServiceRepo.Object);
           // _mockDbContext = new Mock<AppDBContext>();
            _memberService = new MemberService(_unitOfWorkMock.Object, _validationService.Object, _cacheServiceMock.Object );
        }

        [Fact]
        public async Task RegisterMember_Should_Add_Member_And_Save()
        {
            var memberDto = new MemberDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Phone = "+2348100000000",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            await _memberService.AddNewMember(memberDto);

            _memberRepositoryMock.Verify(u => u.AddAsync(It.IsAny<Member>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        //[Fact]
        //public async Task CheckEligibility_ShouldReturnFalse_IfContributionPeriodIsLessThanRequired()
        //{
        //    // Arrange
        //    var memberId = Guid.NewGuid();
        //    var contributions = new List<Contribution> { new Contribution { Amount = 5000, Date = DateTime.UtcNow.AddMonths(-2) } };
        //    _unitOfWorkMock.Setup(uow => uow.Contributions.GetByMemberIdAsync(memberId)).ReturnsAsync(contributions);

        //    // Act
        //    var isEligible = await _memberService.CheckEligibilityAsync(memberId);

        //    // Assert
        //    isEligible.Should().BeFalse();
        //}

        //[Fact]
        //public async Task RemoveMember_ShouldSoftDeleteMember()
        //{
        //    // Arrange
        //    var member = new Member { MemberId = Guid.NewGuid().ToString(), Name = "Jane Doe", IsDeleted = false };
        //    _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(member.MemberId)).ReturnsAsync(member);

        //    // Act
        //    await _memberService.DeleteMemberRecord(member.MemberId);

        //    // Assert
        //    member.IsDeleted.Should().BeTrue();
        //}
    }
}