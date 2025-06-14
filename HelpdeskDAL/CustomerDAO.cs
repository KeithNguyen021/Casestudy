using HelpdeskDAL;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace ExercisesDAL
{
    public class CustomerDAO
    {
        readonly IRepository<Customer> _repo;

        public CustomerDAO()
        {
            _repo = new HelpdeskRepository<Customer>();
        }



        public async Task<Customer> GetEmail(string email)
        {
            Customer? selectedEmployee;
            try
            {
                selectedEmployee = await _repo.GetOne(emp => emp.Email == email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return selectedEmployee!;
        }

        public async Task<List<Customer>> GetAll()
        {
            List<Customer> allEmployees;
            try
            {
                allEmployees = await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allEmployees;
        }

        public async Task<int> Add(Customer newEmployee)
        {
            try
            {
                await _repo.Add(newEmployee);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return newEmployee.Id;
        }

        public async Task<UpdateStatus> Update(Customer  updatedEmployee)
        {
            UpdateStatus status;

            try
            {
                status = await _repo.Update(updatedEmployee);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return status;
        }

        public async Task<int> Delete(int? id)
        {
            int employeesDeleted = -1;
            try
            {
                employeesDeleted = await _repo.Delete((int)id!);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return employeesDeleted;
        }
    }
}

