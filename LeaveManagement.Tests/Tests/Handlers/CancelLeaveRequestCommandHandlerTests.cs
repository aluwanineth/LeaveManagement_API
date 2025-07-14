using LeaveManagement.Application.Features.LeaveRequest.Commands;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using NSubstitute;

namespace LeaveManagement.Tests.Tests.Handlers;

[TestFixture]
public class CancelLeaveRequestCommandHandlerTests
{
    private ILeaveRequestRepositoryAsync _repository;
    private CancelLeaveRequestCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<ILeaveRequestRepositoryAsync>();
        _handler = new CancelLeaveRequestCommandHandler(_repository);
    }

    [Test]
    public async Task Handle_ValidCancellation_ShouldCancelLeaveRequest()
    {
        // Arrange
        var employee = new Employee { EmployeeId = 1, FullName = "John Doe" };
        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 1,
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(15),
            Status = LeaveStatus.Pending,
            Employee = employee
        };

        var command = new CancelLeaveRequestCommand
        {
            LeaveRequestId = 1,
            EmployeeId = 1 // Same employee
        };

        _repository.GetLeaveRequestWithDetailsAsync(1).Returns(leaveRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Message, Is.EqualTo("Your leave request from 2025-07-24 to 2025-07-29 has been cancelled"));
        Assert.That(leaveRequest.Status, Is.EqualTo(LeaveStatus.Cancelled));
        Assert.That(leaveRequest.ApprovalComments, Is.EqualTo("Cancelled by employee"));
        await _repository.Received(1).UpdateAsync(leaveRequest);
    }

    [Test]
    public async Task Handle_LeaveRequestNotFound_ShouldReturnError()
    {
        // Arrange
        var command = new CancelLeaveRequestCommand
        {
            LeaveRequestId = 999,
            EmployeeId = 1
        };

        _repository.GetLeaveRequestWithDetailsAsync(999).Returns((LeaveRequest)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Message, Is.EqualTo("Leave request not found"));
    }

    [Test]
    public async Task Handle_NotOwnLeaveRequest_ShouldReturnError()
    {
        // Arrange
        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 2, // Different employee
            Status = LeaveStatus.Pending
        };

        var command = new CancelLeaveRequestCommand
        {
            LeaveRequestId = 1,
            EmployeeId = 1 // Different employee trying to cancel
        };

        _repository.GetLeaveRequestWithDetailsAsync(1).Returns(leaveRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Message, Is.EqualTo("You can only cancel your own leave requests"));
    }

    [Test]
    public async Task Handle_ApprovedLeaveRequest_ShouldReturnError()
    {
        // Arrange
        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 1,
            Status = LeaveStatus.Approved // Already approved
        };

        var command = new CancelLeaveRequestCommand
        {
            LeaveRequestId = 1,
            EmployeeId = 1
        };

        _repository.GetLeaveRequestWithDetailsAsync(1).Returns(leaveRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Message, Does.Contain("Only pending requests can be cancelled"));
    }

    [Test]
    public async Task Handle_RejectedLeaveRequest_ShouldReturnError()
    {
        // Arrange
        var leaveRequest = new LeaveRequest
        {
            LeaveRequestId = 1,
            EmployeeId = 1,
            Status = LeaveStatus.Rejected
        };

        var command = new CancelLeaveRequestCommand
        {
            LeaveRequestId = 1,
            EmployeeId = 1
        };

        _repository.GetLeaveRequestWithDetailsAsync(1).Returns(leaveRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Message, Does.Contain("Only pending requests can be cancelled"));
    }
}