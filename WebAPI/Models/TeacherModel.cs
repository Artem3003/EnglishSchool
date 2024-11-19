using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace demo_english_school.Models;

public class Teacher : AbstractModel
{
    public string? Bio { get; set; }
    
    public string? Qualification { get; set; }

    public int YearsOfExperience { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }
}