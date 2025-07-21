using System.Security.Claims;
using HR.DTO;
using HR.Interfaces;
using HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class LeaveController(ILeaveService leaveService, IEmployeeService employeeService) : ControllerBase
{
    private readonly ILeaveService _leaveService = leaveService;
    private readonly IEmployeeService _employeeService = employeeService;

    [HttpPost("apply")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> ApplyLeave(DateTime startDate, DateTime endDate, string reason)
    {
        var applicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(applicationUserId))
            return Unauthorized("User ID not found in token.");

        var employeeResponse = await _employeeService.GetEmployeeByUserId(applicationUserId);
        if (employeeResponse == null || employeeResponse.Data == null)
            return NotFound("Employee record not found for this user.");

        var employeeId = employeeResponse.Data.Id;

        var response = await _leaveService.ApplyForLeave(employeeId, startDate, endDate, reason);
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("update")]
    [Authorize(Roles = "HR,Admin")]
    public async Task<IActionResult> UpdateLeave(int leaveId, LeaveStatus status)
    {
        var response = await _leaveService.UpdateLeaveRequest(leaveId, status);
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("my-leaves")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetMyLeaves()
    {
        var applicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(applicationUserId))
            return Unauthorized("User ID not found in token.");

        var response = await _leaveService.GetMyLeaves(applicationUserId);
        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("pending-requests")]
    [Authorize(Roles = "HR,Admin")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var response = await _leaveService.ViewPendingLeaveRequestsAsync();
        return Ok(response);
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetLeaveSummary()
    {
        var applicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(applicationUserId))
            return Unauthorized("User ID not found in token.");

        var response = await _leaveService.GetLeaveSummaryAsync(applicationUserId);
        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("department-summary")]
    [Authorize(Roles = "HR,Admin")]
    public async Task<IActionResult> GetDepartmentLeaveSummaries(DateTime? date)
    {
        var queryDate = date ?? DateTime.Today;
        var response = await _leaveService.GetDepartmentLeaveSummariesAsync(queryDate);
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
