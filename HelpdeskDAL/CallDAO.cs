using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class CallDAO
    {
        readonly IRepository<Call> _repo;
        readonly IRepository<Employee> _employeeRepo;
        readonly IRepository<Problem> _problemRepo;


        public CallDAO()
        {
            _repo = new HelpdeskRepository<Call>();
            _employeeRepo = new HelpdeskRepository<Employee>();
            _problemRepo = new HelpdeskRepository<Problem>();
        }

        public async Task<int> Add(Call newCall)
        {
            try
            {
                await _repo.Add(newCall);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return newCall.Id;
        }
        public async Task<Call> GetCallById(int id)
        {
            Call? selectedCall;
            try
            {
                selectedCall = await _repo.GetOne(emp => emp.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return selectedCall!;
        }

        public async Task<UpdateStatus> Update(Call updatedCall)
        {
            UpdateStatus status;

            try
            {
                status = await _repo.Update(updatedCall);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return status;
        }

        public async Task<List<Call>> GetAll()
        {
            List<Call> allCalls;
            try
            {
                allCalls = await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allCalls;
        }

        public async Task<int> Delete(int? id)
        {
            int callDeleted = -1;
            try
            {
                callDeleted = await _repo.Delete((int)id!);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return callDeleted;
        }
        public async Task<List<Call>> GetCallsByCustomerId(int customerId)
        {
            List<Call> customerCalls;
            try
            {
                customerCalls = await _repo.GetSome(call => call.CustomerId == customerId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return customerCalls;
        }

    }
}
