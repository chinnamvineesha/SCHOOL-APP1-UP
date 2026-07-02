using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Models
{
    public class TeachingStaff
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(80)]
        public string Designation { get; set; } = string.Empty;

        [MaxLength(30)]
        public string EmployeeId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Qualification { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
