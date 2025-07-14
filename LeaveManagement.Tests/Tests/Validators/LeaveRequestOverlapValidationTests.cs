using FluentValidation.TestHelper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Validators;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Enums;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Validators;

[TestFixture]
public class LeaveRequestOverlapValidationTests
{
    private ILeaveRequestRepositoryAsync _repository;
    private LeaveRequestRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<ILeaveRequestRepositoryAsync>();
        _validator = new LeaveRequestRequestValidator(_repository);
    }

    [Test]
    public async Task Validate_NoOverlappingRequests_ShouldPass()
    {
        // Arrange
        var request = new LeaveRequestRequest
        {
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            LeaveType = LeaveType.Annual
        };

        _repository.HasOverlappingPendingLeaveRequestsAsync(1, request.StartDate, request.EndDate)
            .Returns(false);

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
