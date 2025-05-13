using Microsoft.AspNetCore.Mvc;
using EnrollmentService.Application.Interfaces;
using EnrollmentService.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace EnrollmentService.API.Controllers;

/// <summary>
/// Controller for managing student enrollments in courses.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentController"/> class.
    /// </summary>
    /// <param name="enrollmentService">The enrollment service to manage enrollment operations.</param>
    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Retrieves an enrollment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the enrollment.</param>
    /// <returns>An <see cref="IActionResult"/> containing the <see cref="EnrollmentDTO"/> if found; otherwise, a 404 Not Found response.</returns>
    /// <exception cref="ArgumentException">Thrown when the enrollment with the specified ID is not found.</exception>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEnrollmentById(Guid id)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
        return Ok(enrollment);
    }

    /// <summary>
    /// Retrieves all enrollments for a specific student.
    /// </summary>
    /// <param name="userId">The unique identifier of the user (student).</param>
    /// <returns>An <see cref="IActionResult"/> containing a list of <see cref="EnrollmentDTO"/> objects representing the student's enrollments.</returns>
    /// <exception cref="ArgumentException">Thrown when the user is not found or is not a student.</exception>
    [HttpGet("student/{userId}")]
    public async Task<IActionResult> GetEnrollmentsByStudent(Guid userId)
    {
        var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(userId);
        return Ok(enrollments);
    }

    /// <summary>
    /// Retrieves all enrollments for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>An <see cref="IActionResult"/> containing a list of <see cref="EnrollmentDTO"/> objects representing the enrollments in the course.</returns>
    /// <exception cref="ArgumentException">Thrown when the course is not found.</exception>
    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetEnrollmentsByCourse(Guid courseId)
    {
        var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
        return Ok(enrollments);
    }

    /// <summary>
    /// Enrolls a student in a course.
    /// </summary>
    /// <param name="request">The request containing the student and course identifiers.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success with a 200 OK response.</returns>
    /// <exception cref="ArgumentException">Thrown when the student or course is not found.</exception>
    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollmentRequest request)
    {
        await _enrollmentService.EnrollStudentAsync(request.StudentId, request.CourseId);
        return Ok();
    }

    /// <summary>
    /// Cancels an enrollment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the enrollment to cancel.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success with a 200 OK response.</returns>
    /// <exception cref="ArgumentException">Thrown when the enrollment with the specified ID is not found.</exception>
    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelEnrollment(Guid id)
    {
        await _enrollmentService.CancelEnrollmentAsync(id);
        return Ok();
    }
}

/// <summary>
/// Represents a request to enroll a student in a course.
/// </summary>
public class EnrollmentRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the student (user ID).
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the course.
    /// </summary>
    public Guid CourseId { get; set; }
}