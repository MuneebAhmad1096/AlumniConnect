using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AlumniConnect.Data;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? RollNumber { get; set; }

    [MaxLength(100)]
    public string Department { get; set; } = "Software Engineering";
}