using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace demo_english_school.Models;

public class Student : AbstractModel
{
    public DateTime DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public User? User { get; set; }
}