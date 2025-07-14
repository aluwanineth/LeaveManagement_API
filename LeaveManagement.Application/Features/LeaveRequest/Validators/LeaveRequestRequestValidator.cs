using FluentValidation;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Interfaces.Repositories;

namespace LeaveManagement.Application.Features.LeaveRequest.Validators;

public class LeaveRequestRequestValidator : AbstractValidator<LeaveRequestRequest>
{
    private readonly ILeaveRequestRepositoryAsync _leaveRequestRepository;

    public LeaveRequestRequestValidator(ILeaveRequestRepositoryAsync leaveRequestRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Employee ID must be greater than 0");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.LeaveType)
            .IsInEnum().WithMessage("Invalid leave type");

        RuleFor(x => x.Comments)
            .MaximumLength(500).WithMessage("Comments cannot exceed 500 characters");

        RuleFor(x => x)
            .MustAsync(async (request, cancellation) =>
            {
                return !await _leaveRequestRepository.HasOverlappingPendingLeaveRequestsAsync(
                    request.EmployeeId,
                    request.StartDate,
                    request.EndDate,
                    null);
            })
            .WithMessage("You already have a pending leave request that overlaps with these dates.")
            .WithName("DateRange");
    }
}