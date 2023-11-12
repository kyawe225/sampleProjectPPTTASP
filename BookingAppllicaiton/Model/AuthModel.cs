using System.ComponentModel.DataAnnotations;

namespace BookingAppllicaiton.Model;

public class LoginModel
{
    [EmailAddress]
    public string email { set; get; }
    [StringLength(30)]
    public string password { set; get; }
}

public class UserModel
{
    [StringLength(200)]
    public string name { set; get; }
    [EmailAddress]
    public string email { set; get; }
    [StringLength(120)]
    public string address { set; get; }
    
}
public class RegisterModel : UserModel
{
[StringLength(30)]
[Compare(nameof(ConfirmPassword),ErrorMessage = "Password and Compare Password are not same")]
public string password { set; get; }
[StringLength(30)]
public string ConfirmPassword { set; get; }
}
public class PasswordChangeModel
{
    public string OldPassword { set; get; }
    [StringLength(30)]
    [Compare(nameof(ConfirmPassword),ErrorMessage="Password and Compare Password are not same")]
    public string Password { set; get; }
    [StringLength(30)]
    public string ConfirmPassword { set; get; }
}