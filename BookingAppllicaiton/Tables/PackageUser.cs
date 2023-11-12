using System.ComponentModel.DataAnnotations.Schema;

namespace BookingAppllicaiton.Tables;

public class PackageUser:BaseTable
{
    public long UserId { set; get; }
    public long PackageId { set; get; }
    [ForeignKey("UserId")]
    public virtual User? User { set; get; }
    [ForeignKey("PackageId")]
    public virtual PackageTable? Package { set; get; }
    public long Credit { set; get; }
}