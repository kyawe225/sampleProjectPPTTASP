using System.Security.Claims;
using BookingAppllicaiton.Repository;
using BookingAppllicaiton.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingAppllicaiton.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class PackageController : ControllerBase
{
    private PackagesRepository _context;

    public PackageController(PackagesRepository context)
    {
        _context = context;
    }
    [Authorize]
    [HttpGet]
    public IActionResult Index()
    {
        var sample = _context.getAll();
        return Ok(new
        {
            data = sample
        });
    }
    [Authorize]
    [HttpPost("{Id}")]
    public IActionResult Purchase(int Id)
    {
        var claim=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var package = _context.getByUserId(Convert.ToInt64(claim?.Value),Id); // default id for user
        var pack = _context.getById(Id);
        if (package == null && pack != null)
        {
            PackageUser packages = new PackageUser
            {
                PackageId = Id,
                UserId = 1,
                Credit = pack.Credit
            };
            _context.SavePackageUser(packages);
            return Ok(new
            {
                message="You bought a package."
            });
        }
        return BadRequest(new
        {
            message="Something Wrong"
        });
    }
}

