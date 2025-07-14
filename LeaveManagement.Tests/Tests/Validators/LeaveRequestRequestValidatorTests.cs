using FluentValidation.TestHelper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Validators;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Domain.Enums;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Tests.Tests.Validators
{
    [TestFixture]
    public class LeaveRequestRequestValidatorTests
    {
        private LeaveRequestRequestValidator _validator;
        private ILeaveRequestRepositoryAsync _leaveRequestRepository;


        [SetUp]
        public void SetUp()
        {
            _leaveRequestRepository = Substitute.For<ILeaveRequestRepositoryAsync>();
            _validator = new LeaveRequestRequestValidator(_leaveRequestRepository);
        }

        [Test]
        public async Task Validate_ValidLeaveRequest_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual,
                Comments = "Family vacation"
            };

            // Setup repository to return no overlapping requests
            _leaveRequestRepository.HasOverlappingPendingLeaveRequestsAsync(
                request.EmployeeId, request.StartDate, request.EndDate, null)
                .Returns(false);

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task Validate_InvalidEmployeeId_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 0,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual
            };

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId)
                .WithErrorMessage("Employee ID must be greater than 0");
        }

        [Test]
        public async Task Validate_StartDateInPast_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(-1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual
            };

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StartDate)
                .WithErrorMessage("Start date cannot be in the past");
        }

        [Test]
        public async Task Validate_EndDateBeforeStartDate_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(3),
                LeaveType = LeaveType.Annual
            };

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                .WithErrorMessage("End date must be after start date");
        }

        [Test]
        public async Task Validate_CommentsTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var longComments = new string('A', 501);
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                LeaveType = LeaveType.Annual,
                Comments = longComments
            };

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Comments)
                .WithErrorMessage("Comments cannot exceed 500 characters");
        }

        [Test]
        public async Task Validate_OverlappingPendingRequests_ShouldHaveValidationError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                LeaveType = LeaveType.Annual,
                Comments = "Family vacation"
            };

            // Setup repository to return overlapping requests exist
            _leaveRequestRepository.HasOverlappingPendingLeaveRequestsAsync(
                request.EmployeeId, request.StartDate, request.EndDate, null)
                .Returns(true);

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor("DateRange")
                .WithErrorMessage("You already have a pending leave request that overlaps with these dates.");
        }

        [Test]
        public async Task Validate_NoOverlappingPendingRequests_ShouldNotHaveOverlapError()
        {
            // Arrange
            var request = new LeaveRequestRequest
            {
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                LeaveType = LeaveType.Annual,
                Comments = "Family vacation"
            };

            // Setup repository to return no overlapping requests
            _leaveRequestRepository.HasOverlappingPendingLeaveRequestsAsync(
                request.EmployeeId, request.StartDate, request.EndDate, null)
                .Returns(false);

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor("DateRange");
        }
    }
}
