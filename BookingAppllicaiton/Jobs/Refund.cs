using BookingAppllicaiton.Context;
using BookingAppllicaiton.Tables;

namespace BookingAppllicaiton.Jobs;

public class Refund:IRefund
{
    private DatabaseContext context;
    public Refund(DatabaseContext context)
    {
        this.context = context;
    }
    public void MakeAction()
    {
         IEnumerable<ClassTable> tmpclass=context.Classes.Where(p => p.EndDateTime >= DateTime.Now).ToList();
         foreach (ClassTable classTable in tmpclass)
         {
             var schedules=context.Schedules.Where(p => p.RegisteredClassId == classTable.Id && (p.Type == 3)).ToList();
             foreach (var i in schedules)
             {
                 i.Type = 4;
                 var j=context.PackageUsers.Where(p => p.UserId == i.UserId).FirstOrDefault();
                 if (j != null)
                 {
                     j.Credit += classTable.Credit;
                     context.Update(j);
                 }
                 context.Update(i);
             }
             context.SaveChanges();
         }
    }
}