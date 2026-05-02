using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Mappers;

public static class UserMapper
{
    public static UserResponseDTO ToDto(User user)
    {
        return new UserResponseDTO(
            user.Id,
            user.Email,
            user.Firstname,
            user.Lastname,
            user.Role?.Name ?? "Unknown",
            user.Profile == null ? null : new ProfileDTO(
                user.Profile.Address,
                user.Profile.Phone,
                user.Profile.City,
                user.Profile.State,
                user.Profile.ZipCode,
                user.Profile.Country,
                user.Profile.Nationality,
                user.Profile.Bio,
                user.Profile.FacebookUrl,
                user.Profile.TwitterUrl,
                user.Profile.LinkedInUrl,
                user.Profile.InstagramUrl,
                user.Profile.WebsiteUrl
            )
        );
    }
}