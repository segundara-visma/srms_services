using GradeService.Application.Interfaces;
using GradeService.Application.DTOs;
using GradeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradeService.Application.Services;

public class GradeServiceImpl : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IEnrollmentServiceClient _enrollmentServiceClient;

    public GradeServiceImpl(IGradeRepository gradeRepository, IEnrollmentServiceClient enrollmentServiceClient)
    {
        _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
        _enrollmentServiceClient = enrollmentServiceClient ?? throw new ArgumentNullException(nameof(enrollmentServiceClient));
    }

    public async Task<GradeDTO?> GetGradeByIdAsync(Guid id)
    {
        var grade = await _gradeRepository.GetByIdAsync(id);
        if (grade == null)
            return null;

        return MapToDTO(grade);
    }

    public async Task<IEnumerable<GradeDTO>> GetAllGradesAsync()
    {
        var grades = await _gradeRepository.GetAllAsync();
        return grades.Select(MapToDTO);
    }

    public async Task AddGradeAsync(GradeDTO gradeDto)
    {
        if (gradeDto == null)
            throw new ArgumentNullException(nameof(gradeDto));

        // Validate enrollment
        var isEnrolled = await _enrollmentServiceClient.CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId);
        if (!isEnrolled)
            throw new InvalidOperationException("Student is not enrolled in the specified course.");

        // Validate GradeValue range (e.g., 0-100)
        if (gradeDto.GradeValue < 0 || gradeDto.GradeValue > 100)
            throw new ArgumentException("Grade value must be between 0 and 100.");

        var grade = new Grade
        {
            Id = Guid.NewGuid(),
            StudentId = gradeDto.StudentId,
            CourseId = gradeDto.CourseId,
            GradeValue = gradeDto.GradeValue,
            DateGraded = gradeDto.DateGraded != default ? gradeDto.DateGraded : DateTime.UtcNow,
            Comments = gradeDto.Comments
        };

        await _gradeRepository.AddAsync(grade);
    }

    public async Task<IEnumerable<GradeDTO>> GetGradesByStudentAsync(Guid studentId)
    {
        var grades = await _gradeRepository.GetByStudentIdAsync(studentId);
        return grades.Select(MapToDTO);
    }

    public async Task AssignGradeAsync(Guid studentId, Guid courseId, decimal grade)
    {
        var gradeDto = new GradeDTO
        {
            StudentId = studentId,
            CourseId = courseId,
            GradeValue = grade,
            DateGraded = DateTime.UtcNow,
            Comments = "Grade assigned via TutorService"
        };

        await AddGradeAsync(gradeDto);
    }

    private GradeDTO MapToDTO(Grade grade)
    {
        return new GradeDTO
        {
            Id = grade.Id,
            StudentId = grade.StudentId,
            CourseId = grade.CourseId,
            GradeValue = grade.GradeValue,
            DateGraded = grade.DateGraded,
            Comments = grade.Comments
        };
    }
}