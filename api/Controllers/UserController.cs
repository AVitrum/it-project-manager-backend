using api.Data.Requests;
using api.Data.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPut("addImage")]
        public ActionResult AddImage([FromForm] AddFileRequest request)
        {
            if (request.Equals(null))
            {
                return BadRequest("Error");
            }
            
            userService.SaveImage(request.File);
            return Ok("Saved");
        }

        [HttpPut("addInfo")]
        public ActionResult<string> AddInfo(AddInfoRequest request)
        {
            
            userService.AddInfo(request);
            return "Added";
        }

        [HttpDelete("deleteFile")]
        public ActionResult DeleteImage()
        {
            var response = userService.DeleteImage();
            return response ? Ok(response) : BadRequest(response);
        }

        [HttpGet("info")]
        public ActionResult<UserInfoResponse> Info()
        {
            return Ok(userService.Profile());
        }
    }