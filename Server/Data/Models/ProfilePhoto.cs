using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models;

public class ProfilePhoto
{
    [Key] public long Id { get; init; }
    
    public required string PictureName { get; set; }
    public required string PictureLink { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}