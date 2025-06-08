namespace TutorService.Application.DTOs;

public record TutorDTO(Guid Id, Guid UserId, string FirstName, string LastName, string Email, string Role, Profile? Profile);

//public class TutorDTO
//{
//    public Guid Id { get; set; }
//    public Guid UserId { get; set; }
//    public string FirstName { get; set; }
//    public string LastName { get; set; }
//    public string Email { get; set; }
//}