using api.Dtos.User;
using api.Models;

namespace api.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        public static User ToUserFromCreateDto(this CreateUserRequestDto createUserRequest, string passwordHash)
        {
            return new User
            {
                Username = createUserRequest.Username,
                PasswordHash = passwordHash,
                Email = createUserRequest.Email
            };
        }
    }
}