using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands
{
    public class CancelLeaveRequestCommandHandler : IRequestHandler<CancelLeaveRequestCommand, Response<string>>
    {
        private readonly ILeaveRequestRepositoryAsync _leaveRequestRepository;

        public CancelLeaveRequestCommandHandler(ILeaveRequestRepositoryAsync leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<Response<string>> Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var leaveRequest = await _leaveRequestRepository.GetLeaveRequestWithDetailsAsync(request.LeaveRequestId);

                if (leaveRequest == null)
                {
                    return new Response<string>("Leave request not found");
                }

                if (leaveRequest.EmployeeId != request.EmployeeId)
                {
                    return new Response<string>("You can only cancel your own leave requests");
                }

                if (leaveRequest.Status != LeaveStatus.Pending)
                {
                    return new Response<string>($"Cannot cancel leave request. Only pending requests can be cancelled. Current status: {leaveRequest.Status}");
                }

                leaveRequest.Status = LeaveStatus.Cancelled;
                leaveRequest.ApprovalComments = "Cancelled by employee";

                await _leaveRequestRepository.UpdateAsync(leaveRequest);

                return new Response<string>("Leave request cancelled successfully",
                    $"Your leave request from {leaveRequest.StartDate:yyyy-MM-dd} to {leaveRequest.EndDate:yyyy-MM-dd} has been cancelled");
            }
            catch (Exception ex)
            {
                return new Response<string>($"An error occurred while cancelling the leave request: {ex.Message}");
            }
        }
    }
}
