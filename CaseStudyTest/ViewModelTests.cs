using ExercisesDAL;
using HelpdeskDAL;
using HelpdeskViewModels;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ExercisesTests
{
    public class ViewModelTests
    {
        private readonly ITestOutputHelper output;
        public ViewModelTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public async Task Employee_ComprehensiveVMTest()
        {
            EmployeeViewModel evm = new()
            {
                Title = "Mr.",
                Firstname = "Some",
                Lastname = "Employee",
                Email = "some@abc.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure department id is in Departments table
            };
            await evm.Add();
            output.WriteLine("New Employee Added - Id = " + evm.Id);
            int? id = evm.Id; // need id for delete later
            await evm.GetById();
            output.WriteLine("New Employee " + id + " Retrieved");
            evm.Phoneno = "(555)555-1233";
            if (await evm.Update() == 1)
            {
                output.WriteLine("Employee " + id + " phone# was updated to - " + evm.Phoneno);
            }
            else
            {
                output.WriteLine("Employee " + id + " phone# was not updated!");
            }
            evm.Phoneno = "Another change that should not work";
            if (await evm.Update() == -2)
            {
                output.WriteLine("Employee " + id + " was not updated due to stale data");
            }
            evm = new EmployeeViewModel
            {
                Id = id
            };
            // need to reset because of concurrency error
            await evm.GetById();
            if (await evm.Delete() == 1)
            {
                output.WriteLine("Employee " + id + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + id + " was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
           => await evm.GetById());
        }

        [Fact]
        public async Task Call_ComprehensiveVMTest()
        {
            CallViewModel cvm = new();
            EmployeeViewModel empvm = new();
            ProblemViewModel probvm = new();
            empvm.Lastname = "Nguyen";
            await empvm.GetByLastname();
            cvm.EmployeeId = Convert.ToInt16(empvm.Id);

            probvm.Description = "Memory Upgrade";
            await probvm.GetByEmpDescription();
            empvm.Lastname = "Burner";
            await empvm.GetByLastname();
            cvm.TechId = empvm.Id.Value;

            cvm.ProblemId = Convert.ToInt16(probvm.Id);
            cvm.DateOpened = DateTime.Now;
            cvm.DateClosed = null;
            cvm.OpenStatus = true;
            cvm.Notes = "Kiet has bad RAM, Burner to fix it";
            
            await cvm.Add();
            output.WriteLine("New Call Added - Id = " + cvm.Id);
            int? id = cvm.Id; // need id for delete later
            await cvm.GetById();

            cvm.Notes += " \n Ordered new RAM!";
            if (await cvm.Update() == 1)
            {
                output.WriteLine("Call was updated " + cvm.Notes);
            }
            else
            {
                output.WriteLine("Call was not updated!");
            }
            cvm.Notes = "Another change that should not work";
            if (await cvm.Update() == -2)
            {
                output.WriteLine("Call was not updated due to stale data");
            }
            cvm = new CallViewModel
            {
                Id = (int)id
            };
            // need to reset because of concurrency error
            await cvm.GetById();
            if (await cvm.Delete() == 1)
            {
                output.WriteLine("Call was deleted!");
            }
            else
            {
                output.WriteLine("Call was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
           => await cvm.GetById());
        }

        [Fact]
        public async Task Employee_GetByLastnameTest()
        {
            EmployeeViewModel vm = new() { Lastname = "Employee" };
            await vm.GetByLastname();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByIDTest()
        {
            EmployeeViewModel vm = new() { Id = 1 };
            await vm.GetById();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByPhoneNumber()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777)777-7777" };
            await vm.GetByPhoneNumber();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            List<EmployeeViewModel> allEmployeeVms;
            EmployeeViewModel vm = new();
            allEmployeeVms = await vm.GetAll();
            Assert.True(allEmployeeVms.Count > 0);
        }



        [Fact]
        public async Task Call_GetAllTest()
        {
            List<CallViewModel> allEmployeeVms;
            CallViewModel vm = new();
            allEmployeeVms = await vm.GetAll();
            Assert.True(allEmployeeVms.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            EmployeeViewModel vm;
            vm = new()
            {
                Title = "Mr.",
                Firstname = "Tu ",
                Lastname = "Nguyen",
                Email = "021ntk@gmail.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 200
            };
            await vm.Add();
            Assert.True(vm.Id > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777)777-7777" };
            await vm.GetByPhoneNumber();
            vm.Email = vm.Email == "some@abc.com" ? "employee@email.com" : "some@abc.com";

            Assert.True(await vm.Update() == 1);
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            EmployeeViewModel vm = new() { Phoneno = "(777)777-7777" };
            await vm.GetByPhoneNumber();
            Assert.True(await vm.Delete() == 1);
        }



    }
}
