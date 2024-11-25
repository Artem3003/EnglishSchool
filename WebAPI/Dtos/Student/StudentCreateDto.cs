namespace demo_english_school.Dtos;

public class StudentCreateDto
{
    public int Id { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int UserId { get; set; }
}