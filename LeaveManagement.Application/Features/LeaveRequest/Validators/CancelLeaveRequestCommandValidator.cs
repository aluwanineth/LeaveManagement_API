using FluentValidation;
using LeaveManagement.Application.Features.LeaveRequest.Commands;

namespace LeaveManagement.Application.Features.LeaveRequest.Validators;

public class CancelLeaveRequestCommandValidator : AbstractValidator<CancelLeaveRequestCommand>
{
    public CancelLeaveRequestCommandValidator()
    {
        RuleFor(x => x.LeaveRequestId)
            .GreaterThan(0).WithMessage("Leave request ID must be greater than 0");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Employee ID must be greater than 0");
    }
}
