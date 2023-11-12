using BookingAppllicaiton.Context;
using BookingAppllicaiton.Model;
using BookingAppllicaiton.Tables;
using Microsoft.EntityFrameworkCore;

namespace BookingAppllicaiton.Repository;

public class PackagesRepository
{
    private DatabaseContext _context;
    private ILogger<PackagesRepository> _logger;
    public PackagesRepository(DatabaseContext context,ILogger<PackagesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IList<IGrouping<string,PackageModel>> getAll()
    {
        var packages=_context.Package.AsNoTracking()
            .OrderBy(p => p.CreatedAt)
            .Select(p => new PackageModel()
            {
                Name = p.Name,
                Price = p.Price,
                Credit = p.Credit.ToString(),
                Country = p.County,
                Description = p.Description
            }).GroupBy(p => p.Country).ToList();
        return packages;
    }

    public PackageUser? getByUserId(long userId,long Id)
    {
        return _context.PackageUsers.Include(p=> p.Package).Where(p => p.UserId == userId && p.PackageId == Id && p.Package.ExpiredDate<=DateTime.Now)
            .FirstOrDefault(); // default id for user
    }

    public PackageTable? getById(long Id)
    {
        return _context.Package.Where(p => p.Id == Id).FirstOrDefault();
    }

    public bool SavePackageUser(PackageUser pu)
    {
        try
        {
            _context.PackageUsers.Add(pu);
            _context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogDebug(e.Message);
            return false;
        }
        
    }
}