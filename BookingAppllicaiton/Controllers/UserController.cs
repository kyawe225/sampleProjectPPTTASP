using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookingAppllicaiton.Context;
using BookingAppllicaiton.facade;
using BookingAppllicaiton.Model;
using BookingAppllicaiton.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookingAppllicaiton.Controllers;
[ApiController]
[Route("/api/[controller]/[action]")]
public class UserController:ControllerBase
{
    private DatabaseContext _context;
    private IConfiguration _configuration;
    public UserController(DatabaseContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [AllowAnonymous]
    [HttpPost]
    public IActionResult register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            User user = new User()
            {
                Address = model.address,
                Email = model.email,
                Password = model.password
            };
            _context.User.Add(user);
            Helper.SendVerifyEmail(user);
            _context.SaveChanges();
            return Ok(new
            {
                message="Successfully Registered"
            });
        }
        return Ok(new
        {
            message="Successfully Not Registered"
        });
    }
    [AllowAnonymous]
    [HttpPost]
    public IActionResult login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            User? user=_context.User
                .Where(p=> p.Email.Equals(model.email) && p.Password.Equals(model.password))
                .FirstOrDefault();
            if (user != null)
            {
                Claim[] claims= new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Name,user.Name)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            return RedirectToAction("");
        }
        var errors=ModelState.Values.SelectMany(p => p.Errors.Select(q => q.ErrorMessage));
        return BadRequest(errors);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        Claim? ID=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (ID != null)
        {
            User? user=_context.User.Where(p => p.Id.Equals(Convert.ToInt64(ID.Value))).FirstOrDefault();
            if (user != null)
            {
                UserModel model = new UserModel()
                {
                    name = user.Name,
                    address = user.Address,
                    email = user.Email
                };
                return Ok(new
                {
                    data = model
                });
            }
        }
        return BadRequest(new 
        {
            message="Not Well"
        });
    }

    [Authorize]
    [HttpPut]
    public IActionResult ChangePassword(PasswordChangeModel model)
    {
        Claim? ID=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (ID != null)
        {
            User? user = _context.User.Where(p => p.Id.Equals(Convert.ToInt64(ID.Value))).FirstOrDefault();
            if (user != null)
            {
                if (user.Password.Equals(model.OldPassword))
                {
                    user.Password = model.Password;
                    _context.User.Update(user);
                    _context.SaveChanges();
                    return Ok(new
                    {
                        message="Password Changed Successfully!"
                    });
                }
            }
        }
        return BadRequest(new
        {
            message="Bad Request!"
        });
    }
    [HttpPut]
    public IActionResult ResetPassword(PasswordChangeModel model)
    {
        Claim? ID=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (ID != null)
        {
            User? user = _context.User.Where(p => p.Id.Equals(Convert.ToInt64(ID.Value))).FirstOrDefault();
            if (user != null)
            {
                if (user.Password.Equals(model.OldPassword))
                {
                    user.Password = model.Password;
                    _context.User.Update(user);
                    _context.SaveChanges();
                    return Ok(new
                    {
                        message="Password Changed Successfully!"
                    });
                }
            }
        }
        return BadRequest(new
        {
            message="Bad Request!"
        });
    }
}