using Server.Data.SubModels;

namespace Server.Data.Responses;

public class AdditionalUserInfoResponse
{
    public required string Type { get; set; }
    public required string Info { get; set; }

    public static AdditionalUserInfoResponse ToResponse(AdditionalUserInfo info)
    {
        return new AdditionalUserInfoResponse
        {
            Type = info.Type,
            Info = info.Info
        };
    } 
}