namespace Server.Data.Requests;

public class AddInfoRequest
{
    public required string Type { get; set; }
    public required string Info { get; set; }
}