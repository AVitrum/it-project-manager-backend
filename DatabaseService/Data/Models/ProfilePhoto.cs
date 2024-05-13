using System.ComponentModel.DataAnnotations;

namespace DatabaseService.Data.Models;

public class ProfilePhoto
{
    [Key] public long Id { get; init; }

    public string PictureName { get; set; } = string.Empty;
    public required string PictureLink { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}