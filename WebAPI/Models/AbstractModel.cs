using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_english_school.Models;

public class AbstractModel
{
    [Key]
    public int Id { get; set; }
}