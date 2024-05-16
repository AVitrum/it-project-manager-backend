using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class CommentResponse
{
    public required long Id { get; set; }
    public required string Message { get; set; }
    
    public required EmployeeResponse User { get; set; }

    public static CommentResponse ConvertToResponse(Comment comment)
    {
        var user = EmployeeResponse.ConvertToResponse(
            comment.User.ProjectPerformer.Employee);
        
        return new CommentResponse
        {
            Id = comment.Id,
            Message = comment.Message,
            User = user,
        };
    }
}