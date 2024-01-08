using api.Data.Models;

namespace api.Data.Responses
{
    public class UserInfoResponse
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public static UserInfoResponse? UserToUserInfoResponse(User user)
        {
            return new UserInfoResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}