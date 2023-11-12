using System.Security.Claims;
using BookingAppllicaiton.facade;
using BookingAppllicaiton.Repository;
using BookingAppllicaiton.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingAppllicaiton.Controllers;
[Authorize]
[ApiController]
[Route("/api/[controller]/[action]")]
public class PackageController : ControllerBase
{
    private PackagesRepository _context;

    public PackageController(PackagesRepository context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var sample = _context.getAll();
        return Ok(new
        {
            data = sample
        });
    }

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
                UserId = Convert.ToInt64(claim?.Value),
                Credit = pack.Credit
            };
            _context.SavePackageUser(packages);
            Helper.PaymentCharge("name");
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

