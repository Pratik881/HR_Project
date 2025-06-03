using HR.DTO;
using HR.Interfaces;
using HR.Models;
using HR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _employeeService.GetAllEmployees();
            if (!response.Success)
            {
                return BadRequest(response.Message); // Return a 400 Bad Request if something goes wrong
            }
            return Ok(response.Data); // Return the list of employees
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _employeeService.GetEmployeeById(id);
            if (!response.Success)
            {
                return NotFound(response.Message); // Return 404 if the employee isn't found
            }
            return Ok(response.Data); // Return the employee data
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest("Employee ID mismatch."); // Return 400 if the ID in the path and the body don't match
            }

            var response = await _employeeService.UpdateEmployee(employee);
            if (!response.Success)
            {
                return BadRequest(response.Message); // Return 400 if there is an issue with the update
            }

            return NoContent(); // Return 204 No Content on successful update
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _employeeService.DeleteEmployeeById(id);
            if (!response.Success)
            {
                return NotFound(response.Message); // Return 404 if the employee is not found
            }

            return NoContent(); // Return 204 No Content on successful delete  
        }
    }
}
