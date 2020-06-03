using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBeginners.Models
{
    public static class ModelBuilderExtensionForEFCore 
    {
       public static void MySeed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 3,
                    Name = "Amit",
                    Department = Dept.IT,
                    Email = "amit@gmail.com"
                }
             );
        }
    }
}
