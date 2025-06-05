using Microsoft.AspNetCore.Mvc;
using ReportService.Application.Interfaces;
using ReportService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace ReportService.API.Controllers;

/// <summary>
/// Controller for managing student academic reports, such as transcripts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportController"/> class.
    /// </summary>
    /// <param name="reportService">The report service to handle business logic.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="reportService"/> is null.</exception>
    public ReportController(IReportService reportService)
    {
        _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
    }

    /// <summary>
    /// Retrieves a report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report.</param>
    /// <returns>An <see cref="IActionResult"/> containing the report if found; otherwise, a 404 Not Found response.</returns>
    /// <response code="200">Returns the report with the specified ID.</response>
    /// <response code="404">If no report is found with the specified ID.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetReportById(Guid id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Retrieves a report for a specific student by their student ID.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <returns>An <see cref="IActionResult"/> containing the report if found; otherwise, a 404 Not Found response.</returns>
    /// <response code="200">Returns the report for the specified student.</response>
    /// <response code="404">If no report is found for the specified student.</response>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetReportByStudentId(Guid studentId)
    {
        var report = await _reportService.GetReportByStudentIdAsync(studentId);
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Generates a new report for a student based on their grades and enrollments.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student for whom to generate the report.</param>
    /// <returns>An <see cref="IActionResult"/> containing the newly created report.</returns>
    /// <response code="201">Returns the newly created report.</response>
    /// <response code="400">If the request is invalid (e.g., no grades found).</response>
    /// <response code="409">If a report already exists for the student.</response>
    [HttpPost("generate/{studentId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Authorize(Policy = "TutorOnly")]
    public async Task<IActionResult> GenerateReport(Guid studentId)
    {
        try
        {
            var report = await _reportService.GenerateReportAsync(studentId);
            return CreatedAtAction(nameof(GetReportById), new { id = report.Id }, report);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}