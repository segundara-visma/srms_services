using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using StudentService.Application.Interfaces;

namespace StudentService.API.Controllers;

[ApiController]
[Route("api/s2s/students")]
[Authorize(AuthenticationSchemes = "Auth0")]
public class ServiceToServiceStudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public ServiceToServiceStudentsController(IStudentService studentService)
    {
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
    }

    /// <summary>
    /// Creates a student record for the specified user, called by AdminService.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to create a student record for.</param>
    /// <returns>
    /// A 204 No Content response if the student record is successfully created.
    /// </returns>
    /// <response code="204">Student record was successfully created.</response>
    /// <response code="400">If the input is invalid.</response>
    /// <response code="409">If a student record already exists for the user.</response>
    [HttpPost("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStudentAsync(Guid userId)
    {
        try
        {
            await _studentService.CreateStudentAsync(userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all students.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students);
    }
}