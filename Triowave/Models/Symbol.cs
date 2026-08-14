using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Triowave.Models;

public partial class Symbol
{
    [Key]
    public int Id { get; set; }

    [Column("Symbol")]
    [StringLength(50)]
    public string Symbol1 { get; set; } = null!;

    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Exchange { get; set; }

    [StringLength(50)]
    public string? AssetType { get; set; }

    public DateOnly? IpoDate { get; set; }

    public bool Status { get; set; }

    public bool? Enabled { get; set; }
}
