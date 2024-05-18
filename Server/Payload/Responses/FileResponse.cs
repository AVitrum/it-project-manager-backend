using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class FileResponse
{
    public required string Name { get; init; }
    public required string Link { get; init; }

    public static FileResponse ConvertToResponse(AssignmentFile file)
    {
        return new FileResponse
        {
            Name = file.Name,
            Link = file.Link
        };
    }
}