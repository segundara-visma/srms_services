using GradeService.Application.Interfaces;
using GradeService.Application.DTOs;
using GradeService.Application.Common;
using GradeService.Domain.Entities;

namespace GradeService.Application.Services;

public class GradeServiceImpl : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IEnrollmentServiceClient _enrollmentServiceClient;

    public GradeServiceImpl(
        IGradeRepository gradeRepository,
        IEnrollmentServiceClient enrollmentServiceClient)
    {
        _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
        _enrollmentServiceClient = enrollmentServiceClient ?? throw new ArgumentNullException(nameof(enrollmentServiceClient));
    }

    public async Task<GradeDTO?> GetGradeByIdAsync(Guid id)
    {
        var grade = await _gradeRepository.GetByIdAsync(id);
        return grade is null ? null : MapToDTO(grade);
    }

    public async Task<IEnumerable<GradeDTO>> GetAllGradesAsync()
    {
        var grades = await _gradeRepository.GetAllAsync();
        return grades.Select(MapToDTO);
    }

    public async Task<Guid> AddGradeAsync(CreateGradeDTO gradeDto)
    {
        ArgumentNullException.ThrowIfNull(gradeDto);

        var isEnrolled = await _enrollmentServiceClient
            .CheckEnrollmentAsync(gradeDto.StudentId, gradeDto.CourseId);

        if (!isEnrolled)
            throw new InvalidOperationException("Student is not enrolled in the specified course.");

        if (gradeDto.GradeValue < 0 || gradeDto.GradeValue > 100)
            throw new ArgumentException("Grade value must be between 0 and 100.");

        var grade = new Grade
        {
            Id = Guid.NewGuid(),
            StudentId = gradeDto.StudentId,
            CourseId = gradeDto.CourseId,
            GradeValue = gradeDto.GradeValue,
            GradedAt = gradeDto.GradedAt != default ? gradeDto.GradedAt : DateTime.UtcNow,
            Comments = gradeDto.Comments
        };

        await _gradeRepository.AddAsync(grade);

        return grade.Id;
    }

    public async Task<IEnumerable<GradeDTO>> GetGradesByStudentAsync(Guid studentId)
    {
        var grades = await _gradeRepository.GetByStudentIdAsync(studentId);
        return grades.Select(MapToDTO);
    }

    public async Task AssignGradeAsync(Guid studentId, Guid courseId, decimal grade)
    {
        var gradeDto = new CreateGradeDTO(
            studentId,
            courseId,
            grade,
            DateTime.UtcNow,
            "Grade assigned via TutorService"
        );

        await AddGradeAsync(gradeDto);
    }

    public async Task<PaginatedResponse<GradeDTO>> GetGradesByStudentAsync(
        Guid userId,
        int page = 1,
        int pageSize = 10)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        var paginatedResult = await _gradeRepository
            .GetWithPaginationByStudentIdAsync(userId, page, pageSize);

        return new PaginatedResponse<GradeDTO>
        {
            Items = paginatedResult.Items.Select(MapToDTO),
            TotalCount = paginatedResult.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResponse<GradeDTO>> GetGradesByCourseAsync(
        Guid courseId,
        int page = 1,
        int pageSize = 10)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        var paginatedResult = await _gradeRepository
            .GetWithPaginationByCourseIdAsync(courseId, page, pageSize);

        return new PaginatedResponse<GradeDTO>
        {
            Items = paginatedResult.Items.Select(MapToDTO),
            TotalCount = paginatedResult.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private static GradeDTO MapToDTO(Grade grade) =>
        new(
            grade.Id,
            grade.StudentId,
            grade.CourseId,
            grade.GradeValue,
            grade.GradedAt,
            grade.Comments
        );

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        return (page, pageSize);
    }
}