using System.ComponentModel.DataAnnotations.Schema;

namespace BookingAppllicaiton.Tables;

public class Schedule:BaseTable
{
    public long UserId { set; get; }
    [ForeignKey("UserId")]
    public virtual User? User { set; get; }
    public long RegisteredClassId { set; get; }
    [ForeignKey("RegisteredClassId")]
    public virtual ClassTable? RegisteredClass { set; get; }    
    public short Type { set; get; }
}