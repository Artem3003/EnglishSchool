namespace demo_english_school.Dtos;

public class UserCreateDto
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }
    
    public string? Password { get; set; }

    public string? FullName { get; set; }
}