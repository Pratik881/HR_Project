using HR.DTO;
using HR.Interfaces;
using HR.Models;
using HR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost("apply")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ApplyLeave(int employeeId, DateTime startDate, DateTime endDate, string reason)
    {
        var response = await _leaveService.ApplyForLeave(employeeId, startDate, endDate, reason);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpPut("update")]
    [Authorize(Roles = "HR,Admin")]
    public async Task<IActionResult> UpdateLeave(int leaveId, LeaveStatus status)
    {
        var response = await _leaveService.UpdateLeaveRequest(leaveId, status);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpGet("my-leaves")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetMyLeaves()
    {
        var applicationUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(applicationUserId))
            return Unauthorized("User ID not found in token.");

        var response = await _leaveService.GetMyLeaves(applicationUserId);
        if (!response.Success) return NotFound(response);

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
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetLeaveSummary()
    {
        var applicationUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(applicationUserId))
            return Unauthorized("User ID not found in token.");

        var response = await _leaveService.GetLeaveSummaryAsync(applicationUserId);

        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpGet("department-summary")]
    [Authorize(Roles = "HR,Admin")]
    public async Task<IActionResult> GetDepartmentLeaveSummaries(DateTime? date)
    {
        var queryDate = date ?? DateTime.Today;
        var response = await _leaveService.GetDepartmentLeaveSummariesAsync(queryDate);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response); // or return NotFound, depending on the error context
        }
    }

}
