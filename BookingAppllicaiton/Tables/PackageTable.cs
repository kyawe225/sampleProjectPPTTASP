namespace BookingAppllicaiton.Tables;

public class PackageTable : BaseTable
{
    public string Name { set; get; }
    public long Price { set; get; }
    public string County { set; get; }
    public string Description { set; get; }
    public long Credit { set; get; }
    public DateTime ExpiredDate { set; get; }
}