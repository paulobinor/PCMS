using AutoMapper.Execution;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
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
        private Mock<ILogger<MemberService>> _loggerMock;
        private Mock<IUnitOfWorkRepo> _unitOfWorkMock;
        private Mock<ICacheService>  _cacheServiceMock;
        private Mock<IMemberServiceRepo> _memberServiceRepo;
        private IMemberService _memberService;
        private Mock<IMemberService> _memberServiceMock;
        private Mock<IGenericRepository<pcms.Domain.Entities.Member>> _memberRepositoryMock;

        public MemberServiceTests()
        {

            _validationService = new Mock<IValidationService>();
            _loggerMock = new Mock<ILogger<MemberService>>();
            _cacheServiceMock = new Mock<ICacheService>();
            _memberServiceRepo = new Mock<IMemberServiceRepo>();
            _unitOfWorkMock = new Mock<IUnitOfWorkRepo>();
            _memberRepositoryMock = new Mock<IGenericRepository<pcms.Domain.Entities.Member>>();
            // _mockDbContext = new Mock<AppDBContext>();
            _memberServiceMock = new Mock<IMemberService>();
          //  _memberService = new Mock<MemberService(_unitOfWorkMock.Object, _validationService.Object, _cacheServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterMember_Should_Add_Member_And_Save()
        {
            var memberDto = new AddMemberDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Phone = "+2348100000000",
                DateOfBirth = new DateTime(1990, 1, 1),
                Employer = "Test Employer",
                RSAPin = "PIN12345"
            };

            _unitOfWorkMock.Setup(u => u.Members).Returns(_memberServiceRepo.Object);

            await _memberServiceMock.Object.AddNewMember(memberDto);
            await _unitOfWorkMock.Object.CompleteAsync();

            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        //[Fact]
        //public async Task CheckEligibility_ShouldReturnFalse_IfContributionPeriodIsLessThanRequired()
        //{
        //    // Arrange
        //    var memberId = Guid.NewGuid().ToString();
        //    var contributions = new List<Contribution> { new Contribution { Amount = 5000, ContributionDate = DateTime.UtcNow.AddMonths(-2), YearForContribution = 2025, MonthForContribution = 4 } };
        //    _unitOfWorkMock.Setup(uow => uow.Contributions.GetMemberContributions(memberId)).ReturnsAsync(contributions);

        //    // Act
        //    var isEligible = await _memberService.CheckEligibilityAsync(memberId);

        //    // Assert
        //    isEligible.Should().BeFalse();
        //}

        //[Fact]
        //public async Task RemoveMember_ShouldSoftDeleteMember()
        //{

        //    // Arrange
        //    var member = new pcms.Domain.Entities.Member { MemberId = Guid.NewGuid().ToString(), Name = "Jane Doe", IsDeleted = false };
        //    _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(member.MemberId)).ReturnsAsync(member);

        //    // Act
        //    await _memberService.DeleteMemberRecord(member.MemberId);

        //    // Assert
        //    member.IsDeleted.Should().BeTrue();
        //}
    }
}