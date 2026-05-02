using EnrollmentService.Application.Interfaces;
using EnrollmentService.Application.Common;
using EnrollmentService.Application.DTOs;
using EnrollmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnrollmentService.Application.Services;

public class EnrollmentServiceImpl : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserServiceClient _userServiceClient;
    private readonly ICourseServiceClient _courseServiceClient;

    public EnrollmentServiceImpl(IEnrollmentRepository enrollmentRepository, IUserServiceClient userServiceClient, ICourseServiceClient courseServiceClient)
    {
        _enrollmentRepository = enrollmentRepository;
        _userServiceClient = userServiceClient;
        _courseServiceClient = courseServiceClient;
    }

    public async Task<EnrollmentDTO> GetEnrollmentByIdAsync(Guid enrollmentId)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
        if (enrollment == null)
            throw new ArgumentException($"Enrollment with ID {enrollmentId} not found.");

        return new EnrollmentDTO(enrollment.Id, enrollment.StudentId, enrollment.CourseId, enrollment.EnrollmentDate, enrollment.Status.ToString(), enrollment.PaymentAmount);

        //return new EnrollmentDTO
        //{
        //    Id = enrollment.Id,
        //    StudentId = enrollment.StudentId,
        //    CourseId = enrollment.CourseId,
        //    EnrollmentDate = enrollment.EnrollmentDate,
        //    Status = enrollment.Status.ToString(), // Convert enum to string for DTO
        //    PaymentAmount = enrollment.PaymentAmount
        //};
    }

    public async Task<PaginatedResponse<EnrollmentDTO>> GetEnrollmentsByStudentAsync(Guid userId, int page = 1, int pageSize = 10)
    {
        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null || user.Role != "Student")
            throw new ArgumentException($"User with ID {userId} is not a student.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var paginatedResult = await _enrollmentRepository.GetByStudentIdAsync(userId, page, pageSize);

        var items = paginatedResult.Items.Select(e => new EnrollmentDTO(e.Id, e.StudentId, e.CourseId, e.EnrollmentDate, e.Status.ToString(), e.PaymentAmount));

        return new PaginatedResponse<EnrollmentDTO>
        {
            Items = items,
            TotalCount = paginatedResult.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    //public async Task<IEnumerable<EnrollmentDTO>> GetEnrollmentsByStudentAsync(Guid userId)
    //{
    //    var user = await _userServiceClient.GetUserByIdAsync(userId);
    //    if (user == null || user.Role != "Student")
    //        throw new ArgumentException($"User with ID {userId} is not a student.");

    //    var enrollments = await _enrollmentRepository.GetByStudentIdAsync(userId);
    //    return enrollments.Select(e => new EnrollmentDTO
    //    (
    //        e.Id, 
    //        e.StudentId, 
    //        e.CourseId, 
    //        e.EnrollmentDate, 
    //        e.Status.ToString(), 
    //        e.PaymentAmount
    //    ));
    //}

    public async Task<PaginatedResponse<EnrollmentDTO>> GetEnrollmentsByCourseAsync(Guid courseId, int page = 1, int pageSize = 10)
    {
        var course = await _courseServiceClient.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new ArgumentException($"Course with ID {courseId} not found.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var paginatedResult = await _enrollmentRepository.GetByCourseIdAsync(courseId, page, pageSize);

        var items = paginatedResult.Items.Select(e => new EnrollmentDTO(e.Id, e.StudentId, e.CourseId, e.EnrollmentDate, e.Status.ToString(), e.PaymentAmount));

        return new PaginatedResponse<EnrollmentDTO>
        {
            Items = items,
            TotalCount = paginatedResult.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    //public async Task<IEnumerable<EnrollmentDTO>> GetEnrollmentsByCourseAsync(Guid courseId)
    //{
    //    var course = await _courseServiceClient.GetCourseByIdAsync(courseId);
    //    if (course == null)
    //        throw new ArgumentException($"Course with ID {courseId} not found.");

    //    var enrollments = await _enrollmentRepository.GetByCourseIdAsync(courseId);
    //    return enrollments.Select(e => new EnrollmentDTO
    //    (
    //        e.Id,
    //        e.StudentId,
    //        e.CourseId,
    //        e.EnrollmentDate,
    //        e.Status.ToString(),
    //        e.PaymentAmount
    //    ));
    //}

    public async Task EnrollStudentAsync(Guid studentId, Guid courseId)
    {
        var user = await _userServiceClient.GetUserByIdAsync(studentId);
        if (user == null || user.Role != "Student")
            throw new ArgumentException("Invalid student.");

        var course = await _courseServiceClient.GetCourseByIdAsync(courseId);
        if (course == null)
            throw new ArgumentException("Invalid course.");

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Enrolled
        };

        await _enrollmentRepository.AddAsync(enrollment);
    }

    public async Task CancelEnrollmentAsync(Guid enrollmentId)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
        if (enrollment == null)
            throw new ArgumentException($"Enrollment with ID {enrollmentId} not found.");

        enrollment.Status = EnrollmentStatus.Cancelled;
        await _enrollmentRepository.UpdateAsync(enrollment);
    }

    public async Task<bool> CheckEnrollmentAsync(Guid studentId, Guid courseId)
    {
        var user = await _userServiceClient.GetUserByIdAsync(studentId);
        if (user == null || user.Role != "Student")
            return false; // Don't throw an exception; just return false for GradeService integration

        var course = await _courseServiceClient.GetCourseByIdAsync(courseId);
        if (course == null)
            return false; // Don't throw an exception; just return false

        var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
        return enrollment != null && enrollment.Status == EnrollmentStatus.Enrolled;
    }

    public async Task<IEnumerable<EnrollmentDTO>> GetAllEnrollmentsAsync()
    {
        var enrollments = await _enrollmentRepository.GetAllAsync();
        return enrollments.Select(MapToDTO);
    }

    private EnrollmentDTO MapToDTO(Enrollment enrollment)
    {
        return new EnrollmentDTO(enrollment.Id, enrollment.StudentId, enrollment.CourseId, enrollment.EnrollmentDate, enrollment.Status.ToString(), enrollment.PaymentAmount);
        //return new EnrollmentDTO
        //{
        //    Id = enrollment.Id,
        //    StudentId = enrollment.StudentId,
        //    CourseId = enrollment.CourseId,
        //    EnrollmentDate = enrollment.EnrollmentDate,
        //    Status = enrollment.Status.ToString()
        //};
    }
}