using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_english_school.Models;

public class AbstractModel
{
    [Key]
    [Column(nameof(Id))]
    public int Id { get; set; }
}