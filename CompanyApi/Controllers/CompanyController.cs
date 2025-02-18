﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : Controller
    {
        private static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companies.Exists(_ => _.Name.Equals(company.Name)))
            {
                return Conflict();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return Created($"/companies/{company.CompanyID}", company);
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAllCompanies()
        {
            return Ok(companies);
        }

        [HttpGet("{companyID}")]
        public ActionResult<Company> GetCompany([FromRoute] string companyID)
        {
            return Ok(companies.Find(_ => _.CompanyID.Equals(companyID)));
        }

        [HttpGet("page/{pageIndex}")]
        public ActionResult<Company> GetCompany([FromRoute] int pageIndex, [FromQuery] int pageSize)
        {
            int companyCount = companies.Count();
            int beginCompanyIndex = (pageIndex * pageSize) - 1;
            int pageIndexCount = Math.Min(pageSize, companyCount - (pageIndex * pageSize));
            if (pageIndexCount <= 0)
            {
                return NotFound();
            }

            return Ok(companies.GetRange(beginCompanyIndex, pageIndexCount));
        }

        [HttpPut("{updateCompany.CompanyID}")]
        public ActionResult<Company> ModifyCompanyName(Company updateCompany)
        {
            var targetCompany = companies.Find(_ => _.CompanyID.Equals(updateCompany.CompanyID));
            if (targetCompany == null)
            {
                return NotFound();
            }

            targetCompany.Name = updateCompany.Name;
            return Ok(targetCompany);
        }

        [HttpDelete]
        public ActionResult DeleteAllPets()
        {
            companies.Clear();
            return Ok();
        }

        [HttpPost("{companyID}/employees")]
        public ActionResult<Employee> AddNewEmployee([FromRoute] string companyID, Employee employee)
        {
            employee.EmployeeID = Guid.NewGuid().ToString();
            companies.Find(_ => _.CompanyID.Equals(companyID)).Employees.Add(employee);
            return Ok(employee);
        }

        [HttpGet("{companyID}/employees")]
        public ActionResult<List<Employee>> GetAllEmployees([FromRoute] string companyID)
        {
            return Ok(companies.Find(_ => _.CompanyID.Equals(companyID)).Employees);
        }

        [HttpPut("{CompanyID}/employees/{updateEmployee.EmployeeID}")]
        public ActionResult<Company> UpdateEmployeeInformation([FromRoute] string companyID, Employee updateEmployee)
        {
            var company = companies.Find(_ => _.CompanyID.Equals(companyID));
            var targetEmployee = company.Employees.Find(_ => _.EmployeeID.Equals(updateEmployee.EmployeeID));
            if (targetEmployee == null)
            {
                return NotFound();
            }

            targetEmployee.Name = updateEmployee.Name;
            targetEmployee.Salary = updateEmployee.Salary;
            return Ok(targetEmployee);
        }

        [HttpDelete("{companyID}/employees/{employeeId}")]
        public ActionResult<List<Employee>> DeleteEmployee([FromRoute] string companyID, [FromRoute] string employeeId)
        {
            Company company = companies.Find(_ => _.CompanyID.Equals(companyID));
            var targetEmployee = company.Employees.Find(_ => _.EmployeeID.Equals(employeeId));
            company.Employees.Remove(targetEmployee);
            return Ok(company.Employees);
        }

        [HttpDelete("{companyID}")]
        public ActionResult DeleteCompany([FromRoute] string companyID)
        {
            Company company = companies.Find(item => item.CompanyID.Equals(companyID));
            companies.Remove(company);
            return Ok();
        }
    }
}
