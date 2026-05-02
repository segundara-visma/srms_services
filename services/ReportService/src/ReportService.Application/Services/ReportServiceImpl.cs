using ReportService.Application.Interfaces;
using ReportService.Application.Common;
using ReportService.Application.DTOs;
using ReportService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportService.Application.Services;

public class ReportServiceImpl : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IGradeServiceClient _gradeServiceClient;
    private readonly IEnrollmentServiceClient _enrollmentServiceClient;
    private readonly ICourseServiceClient _courseServiceClient;

    public ReportServiceImpl(IReportRepository reportRepository, IGradeServiceClient gradeServiceClient,
        IEnrollmentServiceClient enrollmentServiceClient, ICourseServiceClient courseServiceClient)
    {
        _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
        _gradeServiceClient = gradeServiceClient ?? throw new ArgumentNullException(nameof(gradeServiceClient));
        _enrollmentServiceClient = enrollmentServiceClient ?? throw new ArgumentNullException(nameof(enrollmentServiceClient));
        _courseServiceClient = courseServiceClient ?? throw new ArgumentNullException(nameof(courseServiceClient));
    }

    public async Task<ReportDTO?> GetReportByIdAsync(Guid id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
            return null;

        return await MapToDTO(report);
    }

    public async Task<ReportDTO?> GetReportByStudentIdAsync(Guid studentId)
    {
        var report = await _reportRepository.GetByStudentIdAsync(studentId);
        if (report == null)
            return null;

        return await MapToDTO(report);
    }

    public async Task<ReportDTO> GenerateReportAsync(Guid studentId)
    {
        var existingReport = await _reportRepository.GetByStudentIdAsync(studentId);
        if (existingReport != null)
            throw new InvalidOperationException("Report already exists for this student.");

        var grades = await _gradeServiceClient.GetGradesByStudentAsync(studentId);
        if (!grades.Any())
            throw new ArgumentException("No grades found for this student.");

        var report = new Report
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            GeneratedAt = DateTime.UtcNow,
            GPA = CalculateGPA(grades),
            Status = "Completed",
            ReportDetails = new List<ReportDetail>()
        };

        await _reportRepository.AddAsync(report);

        foreach (var grade in grades)
        {
            if (await _enrollmentServiceClient.CheckEnrollmentAsync(studentId, grade.CourseId))
            {
                var courseDetails = await _courseServiceClient.GetCourseDetailsAsync(grade.CourseId);
                if (courseDetails.HasValue)
                {
                    var detail = new ReportDetail
                    {
                        Id = Guid.NewGuid(),
                        ReportId = report.Id,
                        CourseId = grade.CourseId,
                        Grade = grade.GradeValue,
                        CourseTitle = courseDetails.Value.Title,
                        Credits = courseDetails.Value.Credits,
                        Status = "Completed"
                    };
                    report.ReportDetails.Add(detail);
                    await _reportRepository.AddDetailAsync(detail);
                }
            }
        }

        if (!report.ReportDetails.Any())
            throw new ArgumentException("No valid courses found to include in the report.");

        return await MapToDTO(report);
    }

    private Task<ReportDTO> MapToDTO(Report report)
    {
        var details = report.ReportDetails
            .Select(td => new ReportDetailDTO(
                td.CourseId,
                td.Grade ?? 0,
                td.CourseTitle,
                td.Credits,
                td.Status
            ))
            .ToList();

        var dto = new ReportDTO(
            report.Id,
            report.StudentId,
            report.GeneratedAt,
            report.GPA,
            report.Status,
            details
        );

        return Task.FromResult(dto);
    }

    private decimal CalculateGPA(IEnumerable<GradeDTO> grades)
    {
        var totalGrades = grades.Average(g => g.GradeValue);
        return Math.Round(totalGrades / 25m, 2); // Convert 100-point scale to 4.0 scale
    }
}