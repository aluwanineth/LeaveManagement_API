using LeaveManagement.Application.Wrappers;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class CancelLeaveRequestCommand : IRequest<Response<string>>
{
    public int LeaveRequestId { get; set; }
    public int EmployeeId { get; set; }
}
