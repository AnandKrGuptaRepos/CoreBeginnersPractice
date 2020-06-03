using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBeginners.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeelist;
        public MockEmployeeRepository()
        {
            _employeelist = new List<Employee>()
            {
                new Employee (){Id=1,Name="Anand",Email="aannd@gmail.com",Department=Dept.HR},
                new Employee (){Id=2,Name="Anand1",Email="aannd1@gmail.com",Department=Dept.IT},
                new Employee (){Id=3,Name="Anand2",Email="aannd2@gmail.com",Department=Dept.PayRoll},
            };
        }

        public Employee AddEmployee(Employee employee)
        {
            employee.Id = _employeelist.Max(e => e.Id) + 1;
            _employeelist.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
           Employee employee= _employeelist.FirstOrDefault(e => e.Id == id);
            if (employee!=null)
            {
                _employeelist.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeelist;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeelist.FirstOrDefault(a => a.Id == Id);

        }

        public Employee Update(Employee employee)
        {
            Employee employee1 = _employeelist.FirstOrDefault(e => e.Id == employee.Id);
            if (employee1!=null)
            {
                employee1.Name = employee.Name;
                employee1.Email = employee.Email;
                employee1.Department = employee.Department;
            }
            return employee1;
        }
    }
}
