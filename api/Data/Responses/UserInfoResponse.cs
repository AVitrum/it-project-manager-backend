using api.Data.Models;

namespace api.Data.Responses
{
    public class UserInfoResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public static UserInfoResponse UserToUserInfoResponse(User user)
        {
            return new UserInfoResponse
            {
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}