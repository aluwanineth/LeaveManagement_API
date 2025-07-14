using LeaveManagement.Application.DTOs.LeaveRequest;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Application.Features.LeaveRequest.Validators;

public class NoOverlappingLeaveRequestsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not LeaveRequestRequest request)
        {
            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}
