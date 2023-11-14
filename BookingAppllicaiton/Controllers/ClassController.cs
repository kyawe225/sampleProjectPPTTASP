using System.Security.Claims;
using System.Text;
using BookingAppllicaiton.Context;
using BookingAppllicaiton.Model;
using BookingAppllicaiton.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace BookingAppllicaiton.Controllers;

/// <summary>
/// 1 is for booked
/// 0 is for cancel
/// 2 is for wait
/// 3 is for checkin 
/// </summary>
[Authorize]
[ApiController]
[Route("/api/[controller]/[action]")]
public class ClassController : ControllerBase
{
    private DatabaseContext _context;
    private IDistributedCache _cache;

    public ClassController(DatabaseContext context,IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Index()
    {
        List<IGrouping<string, ClassTable>> classes = _context.Classes.GroupBy(p => p.Country).ToList();
        return Ok(new
        {
            data = classes
        });
    }

    [HttpPost("{Id}")]
    public IActionResult Book(long Id, BookModel model)
    {
        if (this._cache.Get("Billy") != null)
        {
            this._cache.Set("Billy", Encoding.UTF8.GetBytes(Id.ToString()));
        }
        else
        {
            return Accepted(new Uri($"api/Class/Book/{Id}"),"You are accepted");
        }
        var claim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Convert.ToInt64(claim?.Value);
        if (ModelState.IsValid)
        {
            IEnumerable<Schedule>
                schedules = _context.Schedules.Include(q => q.RegisteredClass).Where(p => p.RegisteredClassId == Id)
                    .ToList(); //default userID

            Schedule? schedule = schedules.Where(p => p.RegisteredClassId == Id && p.UserId == userId).FirstOrDefault();
            if (model.booked)
            {
                if (schedule != null)
                {
                    _cache.Remove("Billy");
                    return Ok(new
                    {
                        message = "Already Booked"
                    });
                }
            }

            ClassTable? classes = _context.Classes.Where(p => p.Id == Id).FirstOrDefault();
            var badSchedule = schedules.Where(p => p.RegisteredClass.StartDateTime >= classes.StartDateTime &&
                                                   p.RegisteredClass.EndDateTime <= classes.StartDateTime)
                .FirstOrDefault();
            if (badSchedule != null)
            {
                _cache.Remove("Billy");
                return Ok(new
                {
                    message = "Overlap Classes"
                });
            }

            if (classes == null)
            {
                _cache.Remove("Billy");
                return Ok(new
                {
                    message = "Class not found"
                });
            }

            PackageUser? packageUser = _context.PackageUsers.Include(p => p.Package).Where(p => p.UserId == userId)
                .FirstOrDefault();
            if (model.booked)
            {
                if (classes.Country.Equals(packageUser.Package.County))
                {
                    int registerCount = schedules.Count();
                    bool wait = registerCount == classes.NumberOfPersons ? true : false;
                    _context.Schedules.Add(new()
                    {
                        Type = wait ? (short)2 : (short)userId,
                        RegisteredClassId = Id,
                        UserId = userId
                    });

                    if (packageUser != null || packageUser.Credit > classes.Credit)
                    {
                        packageUser.Credit = packageUser.Credit - classes.Credit;
                        _context.Update(packageUser);
                    }
                    else
                    {
                        _cache.Remove("Billy");
                        return Ok(
                            new
                            {
                                message = "Not Enough Credit"
                            }
                        );
                    }
                }
                else
                {
                    _cache.Remove("Billy");
                    return Ok(new
                    {
                        messsage = "These are not same country"
                    });
                }
            }
            else
            {
                if (schedule != null)
                {
                    schedule.Type = (short)0;
                    _context.Update(schedule);
                    Schedule? firstPerson = _context.Schedules.Where(p => p.RegisteredClassId == Id && p.Type == 3)
                        .OrderBy(p => p.CreatedAt).FirstOrDefault();
                    if (firstPerson != null)
                    {
                        firstPerson.Type = (short)1;
                        _context.Update(firstPerson);
                    }

                    if (packageUser != null)
                    {
                        if ((classes.StartDateTime - DateTime.Now).Hours >= 4)
                        {
                            packageUser.Credit = packageUser.Credit + classes.Credit;
                            _context.Update(packageUser);
                        }
                    }
                }
            }

            _context.SaveChanges();
            _cache.Remove("Billy");
            return Ok(new
            {
                message = "planned the Schedule."
            });
        }
        _cache.Remove("Billy");
        return Ok(new
        {
            message = "Not planned Schedule."
        });
    }

    [HttpPost]
    public IActionResult CheckIn(int Id)
    {
        var claim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Convert.ToInt64(claim?.Value);
        Schedule? schedule = _context.Schedules.Include(q => q.RegisteredClass)
            .Where(p => p.Id == Id && p.UserId == userId && p.Type == (short)userId).FirstOrDefault();
        if (schedule == null)
        {
            return Ok(new
            {
                message = "Already canceled or checked in"
            });
        }

        if (schedule.RegisteredClass.StartDateTime >= DateTime.Now &&
            schedule.RegisteredClass.EndDateTime <= DateTime.Now)
        {
            schedule.Type = 3;
            _context.Update(schedule);
            _context.SaveChanges();
            return Ok(new
            {
                message = "Checked in Successfully"
            });
        }

        return Ok(new
        {
            message = "Over the class Time"
        });
        
    }
}