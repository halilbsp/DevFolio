using System.Collections.Generic;

namespace DevFolio.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Admin, User
    
    // Her kullanıcının 1 portfolyosu olur
    public Portfolio? Portfolio { get; set; } 
}