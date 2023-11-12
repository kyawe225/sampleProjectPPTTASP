using Microsoft.EntityFrameworkCore;

namespace BookingAppllicaiton.Tables;

[Index("Email",IsUnique = true,Name="Email_Unique_Key")]
public class User:BaseTable
{
    public string Email { set; get; }
    public string Password { set; get; }
    public string Name { set; get; }
    public string Address { set; get; }
}