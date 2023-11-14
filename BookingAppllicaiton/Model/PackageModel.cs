using Microsoft.IdentityModel.Tokens;

namespace BookingAppllicaiton.Model;

public class PackageModel
{
    public long Id { set;get; }
    public string Name { set; get; }
    public string Country { set; get; }
    public string Description { set; get; }
    public string Credit { set; get; }
    public long Price { set; get; }
    public DateTime EndDate { set; get; }
    public long? RemainCredit { set; get; }
}