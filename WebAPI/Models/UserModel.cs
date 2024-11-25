using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Dtos;

namespace demo_english_school.Models;

public class User : AbstractModel
{
    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }
    
    public string? FullName { get; set; }

    public Admin? Admin { get; set; }

    public Student? Student { get; set; }

    public Teacher? Teacher { get; set; }
}
