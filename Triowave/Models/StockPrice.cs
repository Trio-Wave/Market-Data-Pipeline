using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Triowave.Models;

[Table("StockPrice")]
public partial class StockPrice
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Symbol { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Open { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? High { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Low { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Close { get; set; }

    public int? Volume { get; set; }
}
