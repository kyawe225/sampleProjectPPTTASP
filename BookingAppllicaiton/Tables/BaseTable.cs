using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingAppllicaiton.Tables;

public class BaseTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { set; get; }
    public DateTime CreatedAt { set; get; }=DateTime.Now;
    public DateTime UpdatedAt { set; get; }=DateTime.Now;
}