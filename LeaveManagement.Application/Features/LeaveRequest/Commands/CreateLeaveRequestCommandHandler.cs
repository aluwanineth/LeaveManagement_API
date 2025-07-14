using AutoMapper;
using LeaveManagement.Application.DTOs.LeaveRequest;
using LeaveManagement.Application.Interfaces.Repositories;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Domain.Enums;
using MediatR;

namespace LeaveManagement.Application.Features.LeaveRequest.Commands;

public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, Response<LeaveRequestResponse>>
{
    private readonly ILeaveRequestRepositoryAsync _leaveRequestRepository;
    private readonly IMapper _mapper;

    public CreateLeaveRequestCommandHandler(ILeaveRequestRepositoryAsync leaveRequestRepository, IMapper mapper)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _mapper = mapper;
    }

    public async Task<Response<LeaveRequestResponse>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var overlappingRequests = await _leaveRequestRepository.GetOverlappingLeaveRequestsAsync(
                request.LeaveRequest.EmployeeId,
                request.LeaveRequest.StartDate,
                request.LeaveRequest.EndDate);

            if (overlappingRequests.Any())
            {
                var overlapDetails = overlappingRequests.Select(lr =>
                    $"{lr.StartDate:yyyy-MM-dd} to {lr.EndDate:yyyy-MM-dd} ({lr.LeaveType})").ToList();

                var errorMessage = $"Cannot create leave request. You have the following overlapping pending requests: {string.Join(", ", overlapDetails)}";

                return new Response<LeaveRequestResponse>(errorMessage);
            }

            var leaveRequest = _mapper.Map<Domain.Entities.LeaveRequest>(request.LeaveRequest);
            leaveRequest.Status = LeaveStatus.Pending;

            var createdLeaveRequest = await _leaveRequestRepository.AddAsync(leaveRequest);
            var leaveRequestResponse = _mapper.Map<LeaveRequestResponse>(createdLeaveRequest);

            return new Response<LeaveRequestResponse>(leaveRequestResponse, "Leave request created successfully");
        }
        catch (Exception ex)
        {
            return new Response<LeaveRequestResponse>($"An error occurred while creating the leave request: {ex.Message}");
        }
    }
}
