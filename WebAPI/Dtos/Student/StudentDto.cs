namespace demo_english_school.Dtos;

public class StudentDto
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int UserId { get; set; }
}