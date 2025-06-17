using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace BusinessLogicLayer
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private const decimal MAX_SALARY = 1000000; 
        private const decimal MAX_INDIVIDUAL_SALARY = 100000; 
        private const int MAX_ENGINEERS = 5;
        private const int MAX_MANAGERS = 5;
        private const int MAX_HR = 1;



		public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;
        }

        //Implement the method from the interface, and here I can add logic to whichever methods I want 
        //to add logic to
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
           
            return await _employeeRepository.GetEmployeesAsync();
        }

        //This employee being passed in is coming from my controller
        //...and I am writing logic on this employee before passing it to my DAL
        //..where the changes are being executed on my DbSet which goes to the Database
        public async Task AddEmployeeAsync(Employee employee)
        {
			//Add logic to check if the total salary of all employees plus the new employee's salary exceeds the maximum allowed salary
			var employees = await _employeeRepository.GetEmployeesAsync();

			decimal totalSalary = employees.Sum(e => e.Salary);

			if (totalSalary + employee.Salary > MAX_SALARY)
			{
				return;
			}

			if (employee.Salary > MAX_INDIVIDUAL_SALARY)
			{
				return;
			}

			int engineerCount = employees.Count(e => e.Position == "Engineer");
			int managerCount = employees.Count(e => e.Position == "Manager");
			int hrCount = employees.Count(e => e.Position == "HR");

			if (employee.Position == "Engineer" && engineerCount >= MAX_ENGINEERS)
			{
				return;
			}

			if (employee.Position == "Manager" && managerCount >= MAX_MANAGERS)
			{
				return;
			}

			if (employee.Position == "HR" && hrCount >= MAX_HR)
			{
				return;
			}

			await _employeeRepository.AddEmployeeAsync(employee);

		}

        //This employee being passed in is coming from my controller
        //...and I am writing logic on this employee before passing it to my DAL
        //..where the changes are being executed on my DbSet which goes to the Database
        //going to the DAL, and getting the employee from the employees DbSet
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(id);
        }

        //This employee being passed in is coming from my controller
        //...and I am writing logic on this employee before passing it to my DAL
        //..where the changes are being executed on my DbSet which goes to the Database
        public async Task GetDetailsAsync(Employee employee)
        {
            await _employeeRepository.GetDetailsAsync(employee);
        }

        //This employee being passed in is coming from my controller
        //...and I am writing logic on this employee before passing it to my DAL
        //..where the changes are being executed on my DbSet which goes to the Database
        public async Task UpdateEmployeeAsync(Employee employee)
        {
			//Logic 
			var employees = await _employeeRepository.GetEmployeesAsync();

			decimal totalSalaryExcludingCurrent = employees
				.Where(e => e.Id != employee.Id)
				.Sum(e => e.Salary);

			if (totalSalaryExcludingCurrent + employee.Salary > MAX_SALARY)
			{
				return;
			}

			if (employee.Salary > MAX_INDIVIDUAL_SALARY)
			{
				return;
			}

			int engineerCount = employees.Count(e => e.Position == "Engineer");
			int managerCount = employees.Count(e => e.Position == "Manager");
			int hrCount = employees.Count(e => e.Position == "HR");

			var currentEmployee = employees.FirstOrDefault(e => e.Id == employee.Id);
			if (currentEmployee != null)
			{
				if (currentEmployee.Position == "Engineer")
				{
					engineerCount--;
				}
				if (currentEmployee.Position == "Manager")
				{
					managerCount--;
				}
				if (currentEmployee.Position == "HR")
				{
					hrCount--;
				}
			}

			if (employee.Position == "Engineer" && engineerCount >= MAX_ENGINEERS)
			{
				return;
			}

			if (employee.Position == "Manager" && managerCount >= MAX_MANAGERS)
			{
				return;
			}

			if (employee.Position == "HR" && hrCount >= MAX_HR)
			{
				return;
			}

			await _employeeRepository.UpdateEmployeeAsync(employee);

		}
       

	}
}
