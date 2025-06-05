namespace AdminService.Application.DTOs;

public record SystemOverviewDTO(
    int TotalTutors,
    int TotalStudents,
    int TotalGrades,
    int TotalCourses,
    int TotalEnrollments,
    decimal AverageGrade);