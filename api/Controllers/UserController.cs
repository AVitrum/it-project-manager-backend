using api.Data.Requests;
using api.Data.Responses;
using api.Data.SubModels;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("info"), Authorize]
        public ActionResult<UserInfoResponse> Info()
        {
            return Ok(UserInfoResponse.UserToUserInfoResponse(userService.GetFromToken()));
        }

        [HttpPut("addInfo"), Authorize]
        public ActionResult<string> AddInfo(AddInfoRequest request)
        {
            var user = userService.GetFromToken();

            var additionalUserInfo = new AdditionalUserInfo
            {
                UserId = user.Id,
                Type = request.Type,
                Info = request.Info,
            };

            userService.AddInfo(additionalUserInfo);
            return "Added";
        }
    }