using Microsoft.AspNetCore.Mvc;
using GradeService.Application.Interfaces;
using GradeService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace GradeService.API.Controllers;

/// <summary>
/// Controller for handling service-to-service API operations, restricted to Auth0-authenticated requests.
/// </summary>
[ApiController]
[Route("api/s2s/grade")]
[Authorize(AuthenticationSchemes = "Auth0")] // Restrict to Auth0-authenticated service requests
public class ServiceToServiceController : ControllerBase
{
    private readonly IGradeService _gradeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceToServiceController"/> class.
    /// </summary>
    /// <param name="gradeService">The course service.</param>
    public ServiceToServiceController(IGradeService gradeService)
    {
        _gradeService = gradeService ?? throw new ArgumentNullException(nameof(gradeService));
    }

    /// <summary>
    /// StudentService requests retrieve grade by studentId.
    /// </summary>
    /// <param name="studentId">The ID of the student whose grades are to be retrieved.</param>
    /// <returns>The grades details if found.</returns>
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetGradesByStudent(Guid studentId)
    {
        var grades = await _gradeService.GetGradesByStudentAsync(studentId);
        return Ok(grades);
    }
}