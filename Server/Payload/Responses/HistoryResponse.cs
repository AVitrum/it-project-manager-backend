using DatabaseService.Data.Models;

namespace Server.Payload.Responses;

public class HistoryResponse
{
    public required string Change { get; init; }
    public required DateTime UpdatedAt { get; init; }

    public static HistoryResponse ConvertToResponse(AssignmentHistory history)
    {
        return new HistoryResponse
        {
            Change = history.Change,
            UpdatedAt = history.UpdatedAt
        };
    } 
}