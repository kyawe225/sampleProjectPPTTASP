using System.Security.Cryptography;
using BookingAppllicaiton.Context;
using BookingAppllicaiton.Model;
using BookingAppllicaiton.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public ClassController(DatabaseContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Index()
    {
        List<ClassTable> classes = _context.Classes.ToList();
        return Ok(new
        {
            data = classes
        });
    }

    [HttpPost("{Id}")]
    public IActionResult Book(long Id, BookModel model)
    {
        if (ModelState.IsValid)
        {
            IEnumerable<Schedule>
                schedules = _context.Schedules.Include(q=>q.RegisteredClass).Where(p => p.RegisteredClassId == Id).ToList(); //default userID
            
            Schedule? schedule = schedules.Where(p => p.RegisteredClassId == Id && p.UserId == 1).FirstOrDefault();
            if (model.booked)
            {
                if (schedule != null)
                {
                    return Ok(new
                    {
                        message = "Already Booked"
                    });
                }
            }

            ClassTable? classes = _context.Classes.Where(p => p.Id == Id).FirstOrDefault();
            var badSchedule=schedules.Where(p => p.RegisteredClass.StartDateTime >= classes.StartDateTime &&
                                 p.RegisteredClass.EndDateTime <= classes.StartDateTime).FirstOrDefault();
            if (badSchedule!=null)
            {
                return Ok(new
                {
                    message="Overlap Classes"
                });
            }
            if (classes == null)
            {
                return Ok(new
                {
                    message = "Class not found"
                });
            }
            PackageUser? packageUser=_context.PackageUsers.Where(p => p.UserId == 1).FirstOrDefault();
            if (model.booked)
            {
                int registerCount = schedules.Count();
                bool wait = registerCount == classes.NumberOfPersons ? true : false;
                _context.Schedules.Add(new()
                {
                    Type = wait ? (short)2 : (short)1,
                    RegisteredClassId = Id,
                    UserId = 1
                });
                
                if (packageUser != null)
                {
                    packageUser.Credit = packageUser.Credit - classes.Credit;
                    _context.Update(packageUser);
                }
            }
            else
            {
                if (schedule != null)
                {
                    schedule.Type = (short)0;
                    _context.Update(schedule);
                    if (packageUser != null)
                    {
                        if ((classes.StartDateTime - DateTime.Now).Hours>=4)
                        {
                            packageUser.Credit = packageUser.Credit + classes.Credit;
                            _context.Update(packageUser);
                        }
                    }
                }
            }
            _context.SaveChanges();
            return Ok(new
            {
                message = "planned the Schedule."
            });
        }

        return Ok(new
        {
            message = "Not planned Schedule."
        });
    }
    [HttpPost]
    public IActionResult CheckIn(int Id)
    {
        Schedule? schedule = _context.Schedules.Include(q=> q.RegisteredClass).Where(p => p.Id == Id && p.UserId == 1 && p.Type==(short)1).FirstOrDefault();
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