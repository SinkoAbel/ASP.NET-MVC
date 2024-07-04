using DemoMvcApp.Data;
using DemoMvcApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoMvcApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly MvcDemoDbContext dbContext;

        public EmployeeController(MvcDemoDbContext mvcDemoDbContext)
        {
            dbContext = mvcDemoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await dbContext
                                            .Employees
                                            .OrderByDescending(e => e.Salary)
                                            .ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)
        {
            Employee employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                Department = addEmployeeRequest.Department,
                DataOfBirth = addEmployeeRequest.DataOfBirth,
            };

            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }

            Employee employee = await dbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == id);

            if (employee != null)
            {
                UpdateEmployeeViewModel viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    Department = employee.Department,
                    DataOfBirth = employee.DataOfBirth,
                };

                return await Task.Run(() => View("View", viewModel));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel model)
        {
            Employee foundEmployee = await dbContext.Employees.FindAsync(model.Id);

            if (foundEmployee != null)
            {
                foundEmployee.Name = model.Name;
                foundEmployee.Email = model.Email;
                foundEmployee.Salary = model.Salary;
                foundEmployee.Department = model.Department;
                foundEmployee.DataOfBirth = model.DataOfBirth;

                await dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel viewModel)
        {
            Employee employee = await dbContext.Employees.FindAsync(viewModel.Id);

            if (employee != null)
            {
                dbContext.Employees.Remove(employee);
                await dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
