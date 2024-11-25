namespace demo_english_school.Dtos;

public class TeacherCreateDto
{
    public int Id { get; set; }
    public string? Bio { get; set; }
    
    public string? Qualification { get; set; }

    public int YearsOfExperience { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int UserId { get; set; }
}