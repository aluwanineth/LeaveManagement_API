using Asp.Versioning;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Features.LeaveRequest.Commands;
using LeaveManagement.Application.Features.LeaveRequest.Queries;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers.V1;

[ApiVersion("1.0")]
public class LeaveRequestController(IMediator mediator, IAuthenticatedUserService authenticatedUserService) : BaseApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> CreateLeaveRequest([FromBody] LeaveRequestRequest leaveRequestRequest)
    {
        var command = new CreateLeaveRequestCommand { LeaveRequest = leaveRequestRequest };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetLeaveRequestsByEmployee(int employeeId)
    {
        var query = new GetLeaveRequestsByEmployeeQuery { EmployeeId = employeeId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "CEO,Manager,TeamLead")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var approverId = authenticatedUserService.EmployeeId;

        if (!approverId.HasValue)
        {
            return BadRequest(new Response<string>("Employee ID not found"));
        }

        var query = new GetPendingApprovalsQuery { ApproverId = approverId.Value };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("approvals")]
    [Authorize(Roles = "CEO,Manager,TeamLead")]
    public async Task<IActionResult> GetAllApprovalsForManager()
    {
        var approverId = authenticatedUserService.EmployeeId;

        if (!approverId.HasValue)
        {
            return BadRequest(new Response<string>("Employee ID not found"));
        }

        var query = new GetApprovalsForManagerQuery { ManagerId = approverId.Value };
        var result = await Mediator.Send(query);

        return Ok(result);
    }

    [HttpPut("approve")]
    [Authorize(Roles = "CEO,Manager,TeamLead")]
    public async Task<IActionResult> ApproveLeaveRequest([FromBody] ApproveLeaveRequest approveLeaveRequest)
    {
        var approverId = authenticatedUserService.EmployeeId;

        if (!approverId.HasValue)
        {
            return BadRequest(new Response<string>("Employee ID not found"));
        }

        var command = new ApproveLeaveRequestCommand
        {
            ApprovalRequest = approveLeaveRequest,
            ApproverId = approverId.Value
        };

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("reject")]
    [Authorize(Roles = "CEO,Manager,TeamLead")]
    public async Task<IActionResult> RejectLeaveRequest([FromBody] RejectLeaveRequest rejectLeaveRequest)
    {
        var rejectorId = authenticatedUserService.EmployeeId;

        if (!rejectorId.HasValue)
        {
            return BadRequest(new Response<string>("Employee ID not found"));
        }

        var command = new RejectLeaveRequestCommand
        {
            LeaveRequestId = rejectLeaveRequest.LeaveRequestId,
            RejectedById = rejectorId.Value,
            RejectionComments = rejectLeaveRequest.RejectionComments
        };

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> CancelLeaveRequest(int id)
    {
        var employeeId = authenticatedUserService.EmployeeId;

        if (!employeeId.HasValue)
        {
            return BadRequest(new Response<string>("Employee ID not found"));
        }

        var command = new CancelLeaveRequestCommand
        {
            LeaveRequestId = id,
            EmployeeId = employeeId.Value
        };

        var result = await Mediator.Send(command);

        if (!result.Succeeded)
        {
            if (result.Message.Contains("not found"))
            {
                return NotFound(result);
            }
            if (result.Message.Contains("can only cancel your own"))
            {
                return Forbid();
            }
            if (result.Message.Contains("Cannot cancel"))
            {
                return BadRequest(result);
            }
        }

        return Ok(result);
    }
}
