using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreBeginners.Models;
using CoreBeginners.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CoreBeginners.Controllers
{
    public class HomeController:Controller
    {
        private IEmployeeRepository _empployeeRepository;
        private IHostingEnvironment hostingEnvironment;
        public HomeController(IEmployeeRepository employee, IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            _empployeeRepository = employee;
        }
        public ViewResult Details(int? id)
        {
           // throw new Exception("This is Demo Exception ");
            Employee employee = _empployeeRepository.GetEmployee(id.Value);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound",id.Value);
            }
            HomeControllerViewModel homeControllerViewModel = new HomeControllerViewModel();
            homeControllerViewModel.Employee = employee;
            homeControllerViewModel.PageTitle = "Employee Details";
            return View(homeControllerViewModel);
        }
        [AllowAnonymous]
        public ViewResult Index()
        {
            return View(_empployeeRepository.GetAllEmployee());
        }
        
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(EmployeeCreateViewModel model)
        {    
            if (ModelState.IsValid)
            {
                string uniqueFilename = ProcessUploadedFileImage(model);
                
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFilename

                };
                  _empployeeRepository.AddEmployee(newEmployee);
                return RedirectToAction("Details", new { id = newEmployee.Id });

            }
            return View();
        }
        [HttpGet]
        public ViewResult Update(int id)
        {
            Employee employee = _empployeeRepository.GetEmployee(id);
            EmployeeCreateViewModel employeeCreateViewModel = new EmployeeCreateViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeCreateViewModel);
        }
        [HttpPost]
        public IActionResult Update(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee emp = _empployeeRepository.GetEmployee(model.Id);
                emp.Name = model.Name;
                emp.Department = model.Department;
                emp.Email = model.Email;
                if (model.Photos!=null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    emp.PhotoPath = ProcessUploadedFileImage(model);
                }
               Employee updatedemployee= _empployeeRepository.Update(emp);
                return RedirectToAction("Index");

            }
            return View();
        }

        private string ProcessUploadedFileImage(EmployeeCreateViewModel model)
        {
            string uniqueFilename = null;
            if (model.Photos != null && model.Photos.Count > 0)
            {
                foreach (var photo in model.Photos)
                {
                    string UploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    uniqueFilename = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(UploadsFolder, uniqueFilename);
                    using (var filestream= new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(filestream);
                    }
                }
            }

            return uniqueFilename;
        }

        public ViewResult Delete(int Id)
        {
            _empployeeRepository.Delete(Id);
            return View();
        }
    }
}
