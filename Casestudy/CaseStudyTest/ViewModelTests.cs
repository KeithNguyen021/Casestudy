using ExercisesDAL;
using HelpdeskViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExercisesTests
{
    public class ViewModelTests
    {
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

        [Fact]
        public async Task Employee_LoadPicsTest()
        {
            {
                PicsUtility util = new();
                Assert.True(await util.AddEmployeePicsToDb());
            }
        }

    }
}
