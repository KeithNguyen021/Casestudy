using ExercisesDAL;
using HelpdeskDAL;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaseStudyTest
{
    public class DAOTests
    {
        private readonly ITestOutputHelper output;
        public DAOTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Employee_ComprehensiveTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "Joe",
                LastName = "Smith",
                PhoneNo = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "js@abc.com"
            };
            int newEmployeeId = await dao.Add(newEmployee);
            output.WriteLine("New Employee Generated - Id = " + newEmployeeId);
            newEmployee = await dao.GetById(newEmployeeId);
            byte[] oldtimer = newEmployee.Timer!;
            output.WriteLine("New Employee " + newEmployee.Id + " Retrieved");
            newEmployee.PhoneNo = "(555)555-1233";
            if (await dao.Update(newEmployee) == UpdateStatus.Ok)
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was updated to -" + newEmployee.PhoneNo);
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was not updated!");
            }
            newEmployee.Timer = oldtimer; // to simulate another user
            newEmployee.PhoneNo = "doesn't matter data is stale now";
            if (await dao.Update(newEmployee) == UpdateStatus.Stale)
            {
                output.WriteLine("Employee " + newEmployeeId + " was not updated due to stale data");
            }

            dao = new();
            await dao.GetById(newEmployeeId);
            if (await dao.Delete(newEmployeeId) == 1)
            {
                output.WriteLine("Employee " + newEmployeeId + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetById(newEmployeeId));
        }

        [Fact]
        public async Task Call_ComprehensiveTest()
        {
            CallDAO dao = new();
            EmployeeDAO empdao = new();
            ProblemDAO pdao = new();
            Employee selectedEmployee = await empdao.GetByLastname("Nguyen");
            Employee selectedTech = await empdao.GetByLastname("Burner");
            Problem badDrive = await pdao.GetByDescription("Hard Drive Failure");
            Call newCall = new()
            {
                EmployeeId = selectedEmployee.Id,
                TechId = selectedTech.Id,
                ProblemId = badDrive.Id,
                DateOpened = DateTime.Now,
                DateClosed = null,
                OpenStatus = true,
                Notes = selectedEmployee.LastName + "’s drive is shot, " + selectedTech.LastName + " to fix it"
            };
            int newCallId = await dao.Add(newCall);
            output.WriteLine("New Call Generated - Id = " + newCallId);
            newCall = await dao.GetCallById(newCallId);
            byte[] oldtimer = newCall.Timer!;
            output.WriteLine("New Call Retrieved");
            newCall.Notes += "\n Ordered new drive!";
            if (await dao.Update(newCall) == UpdateStatus.Ok)
            {
                output.WriteLine("Call was updated to " + newCall.Notes);
            }
            else
            {
                output.WriteLine("Call was not updated!");
            }
            newCall.Timer = oldtimer; // to simulate another user
            newCall.Notes = "\n Ordered new drive!";
            if (await dao.Update(newCall) == UpdateStatus.Stale)
            {
                output.WriteLine("Call was not updated due to stale data");
            }

            dao = new();
            await dao.GetCallById(newCallId);
            if (await dao.Delete(newCallId) == 1)
            {
                output.WriteLine("Call was deleted!");
            }
            else
            {
                output.WriteLine("Call was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetCallById(newCallId));
        }


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
