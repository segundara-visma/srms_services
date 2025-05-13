using StudentService.Application.DTOs;
using StudentService.Application.Services;
using StudentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace StudentService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Retrieves a student by their User ID.
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetStudentById(Guid userId)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(userId);
            return Ok(student);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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

    ///// <summary>
    ///// Enrolls a student in a course.
    ///// </summary>
    //[HttpPost("{userId}/enroll")]
    //public async Task<IActionResult> EnrollStudent(Guid userId, [FromBody] EnrollStudentRequest request)
    //{
    //    try
    //    {
    //        await _studentService.EnrollStudentAsync(userId, request.CourseId);
    //        return NoContent();
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

    ///// <summary>
    ///// Retrieves all courses a student is enrolled in.
    ///// </summary>
    //[HttpGet("{userId}/courses")]
    //public async Task<IActionResult> GetStudentCourses(Guid userId)
    //{
    //    try
    //    {
    //        var courses = await _studentService.GetStudentCoursesAsync(userId);
    //        return Ok(courses);
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}
}

//public record EnrollStudentRequest(Guid CourseId);