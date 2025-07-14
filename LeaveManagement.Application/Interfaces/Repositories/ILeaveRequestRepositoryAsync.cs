using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Interfaces.Repositories;

public interface ILeaveRequestRepositoryAsync : IGenericRepositoryAsync<LeaveRequest>
{
    Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
    Task<IReadOnlyList<LeaveRequest>> GetPendingRequestsForManagerAsync(int managerId);
    Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsForApprovalAsync(int approverId);
    Task<IReadOnlyList<LeaveRequest>> GetPendingApprovalsAsync(int approverId);
    Task<LeaveRequest?> GetLeaveRequestWithDetailsAsync(int leaveRequestId);
    Task<bool> HasOverlappingPendingLeaveRequestsAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveRequestId = null);
    Task<IReadOnlyList<LeaveRequest>> GetOverlappingLeaveRequestsAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveRequestId = null);
}
