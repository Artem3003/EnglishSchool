using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demo_english_school.Models;

public class Admin : AbstractModel
{
    public string? Role { get; set; }
    public int UserId { get; set; }

    public User? User { get; set; }
}