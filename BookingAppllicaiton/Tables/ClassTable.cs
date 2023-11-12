using System.ComponentModel.DataAnnotations.Schema;

namespace BookingAppllicaiton.Tables;

public class ClassTable:BaseTable
{
    public string Name { set; get; }
    public string Description { set; get; }
    public long Credit { set; get; }
    public DateTime StartDateTime { set; get; }
    public DateTime EndDateTime { set; get; }
    public long PackageId { set; get; }
    [ForeignKey("PackageId")]
    public virtual PackageTable? Package { set; get; }
    public string Address { set; get; }
    public int NumberOfPersons { set; get; }
}