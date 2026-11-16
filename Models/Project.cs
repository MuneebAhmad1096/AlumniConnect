using System.ComponentModel.DataAnnotations;

namespace AlumniConnect.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Domain { get; set; } = "Web"; 

        public string BatchYear { get; set; } = "2024";

        [Required]
        public string AuthorName { get; set; } = string.Empty;

        [Required]
        public string RollNumber { get; set; } = string.Empty;

        [Required]
        public string SupervisorName { get; set; } = string.Empty;

        public string? PdfFilePath { get; set; }

        public string? GitHubUrl { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime SubmittedDate { get; set; } = DateTime.Now;
    }
}