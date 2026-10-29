using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models;

public class Student
{
    public int Id { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Department { get; set; } = string.Empty;

    [Range(0, 100)]
    [Display(Name = "Marks (%)")]
    public decimal Marks { get; set; }

    [Required, StringLength(24)]
    [Display(Name = "Student ID")]
    public string StudentNumber { get; set; } = string.Empty;

    [StringLength(24)]
    public string Status { get; set; } = "Active";

    [DataType(DataType.Date)]
    [Display(Name = "Enrollment date")]
    public DateTime EnrollmentDate { get; set; } = DateTime.Today;
}
