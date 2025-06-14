using ExercisesDAL;
using HelpdeskDAL;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CaseStudyTest
{
    public class DAOTests
    {
        [Fact]
        public async Task Employee_GetByLastnameTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetByLastname("Nguyen");
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetByIDTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetById(1);
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetEmail("john.doe@company.com");
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            EmployeeDAO dao = new();
            List<Employee> allEmployees = await dao.GetAll();
            Assert.True(allEmployees.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNo = "519-555-1234",
                Email = "john.doe@company.com",
                DepartmentId = 100,
                StaffPicture = null 
            };

            Assert.True(await dao.Add(newEmployee) > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForUpdate = await dao.GetByLastname("Doe");
            if (employeeForUpdate != null)
            {
                string oldPhoneNo = employeeForUpdate.PhoneNo!;
                string newPhoneNo = oldPhoneNo == "519-555-1234" ? "555-555-5555" : "519-555-1234";
                employeeForUpdate!.PhoneNo = newPhoneNo;
            }
            Assert.True(await dao.Update(employeeForUpdate!) == UpdateStatus.Ok);
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeDAO dao = new();
            Employee? employeeForDelete = await dao.GetByLastname("Doe");
            int deletedRows = 0;
            if (employeeForDelete != null)
            {
                deletedRows = await dao.Delete(employeeForDelete.Id);
            }
            Assert.True(deletedRows == 1);
        }

        [Fact]
        public async Task Employee_ConcurrencyTest()
        {
            EmployeeDAO dao1 = new();
            EmployeeDAO dao2 = new();
            Employee employeeForUpdate1 = await dao1.GetByLastname("Nguyen");
            Employee employeeForUpdate2 = await dao2.GetByLastname("Nguyen");
            if (employeeForUpdate1 != null)
            {
                string? oldPhoneNo = employeeForUpdate1.PhoneNo;
                string? newPhoneNo = oldPhoneNo == "519-555-1234" ? "555-555-5555" : "519-555-1234";
                employeeForUpdate1.PhoneNo = newPhoneNo;
                if (await dao1.Update(employeeForUpdate1) == UpdateStatus.Ok)
                {
                    // Change phone # to something else
                    employeeForUpdate2.PhoneNo = "666-666-6668";
                    Assert.True(await dao2.Update(employeeForUpdate2) == UpdateStatus.Stale);
                }
                else
                {
                    Assert.True(false); 
                }
            }
            else
            {
                Assert.True(false); 
            }
        }
    }
}
