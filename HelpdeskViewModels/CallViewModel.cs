using HelpdeskDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskViewModels
{
    public class CallViewModel
    {
        private readonly CallDAO _dao;
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProblemId { get; set; }
        public string? EmployeeName { get; set; }
        public string? ProblemDescription { get; set; }
        public string? CustumerName { get; set; }
        public int CustomerId { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public bool OpenStatus { get; set; }
        public string? Notes { get; set; }

        public int? ExpectedProcessingDays { get; set; }

        public string? Timer { get; set; }


        public CallViewModel()
        {
            _dao = new CallDAO();
        }

        public async Task GetById()
        {
            try
            {
                Call call = await _dao.GetCallById(Id!);
                EmployeeId = call.EmployeeId;
                ProblemId = call.ProblemId;
                CustomerId = call.CustomerId;
                DateOpened = call.DateOpened;
                DateClosed = call.DateClosed;
                OpenStatus = call.OpenStatus;
                Notes = call.Notes;
                ExpectedProcessingDays = call.ExpectedProcessingDays;
                Timer = Convert.ToBase64String(call.Timer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
        public async Task<List<CallViewModel>> GetCallByCustomerID()
        {
            List<CallViewModel> allVms = new();
            try
            {
                List<Call> allCalls = await _dao.GetCallsByCustomerId(CustomerId);
                foreach (Call call in allCalls)
                {
                    CallViewModel empVm = new()
                    {
                        Id = call.Id,
                        EmployeeId = call.EmployeeId,
                        ProblemId = call.ProblemId,
                        CustomerId = call.CustomerId,
                        DateOpened = call.DateOpened,
                        DateClosed = call.DateClosed,
                        OpenStatus = call.OpenStatus,
                        Notes = call.Notes,
                        ExpectedProcessingDays = call.ExpectedProcessingDays,
                        Timer = Convert.ToBase64String(call.Timer)
                    };
                    allVms.Add(empVm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }
        public async Task<List<CallViewModel>> GetAll()
        {
            List<CallViewModel> allVms = new();
            try
            {
                List<Call> allCalls = await _dao.GetAll();
                foreach (Call call in allCalls)
                {
                    CallViewModel empVm = new()
                    {
                        Id = call.Id,
                        EmployeeId = call.EmployeeId,
                        ProblemId = call.ProblemId,
                        CustomerId= call.CustomerId,
                        DateOpened = call.DateOpened,
                        DateClosed = call.DateClosed,
                        OpenStatus = call.OpenStatus,
                        Notes = call.Notes,
                        ExpectedProcessingDays = call.ExpectedProcessingDays,
                        Timer = Convert.ToBase64String(call.Timer)
                    };
                    allVms.Add(empVm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }

        public async Task Add()
        {
            try
            {
                Call emp = new()
                {
                    EmployeeId = EmployeeId,
                    ProblemId = ProblemId,
                    CustomerId = CustomerId,
                    DateOpened = DateOpened,
                    DateClosed = DateClosed,
                    OpenStatus = OpenStatus,
                    Notes = Notes,
                    ExpectedProcessingDays = ExpectedProcessingDays

                };
                Id = await _dao.Add(emp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                Call emp = new()
                {
                    Id = Id,
                    EmployeeId = EmployeeId,
                    ProblemId = ProblemId,
                    CustomerId = CustomerId,
                    DateOpened = DateOpened,
                    DateClosed = DateClosed,
                    OpenStatus = OpenStatus,
                    Notes = Notes,
                    Timer = Convert.FromBase64String(Timer!),
                    ExpectedProcessingDays = ExpectedProcessingDays

                };
                updateStatus = -1; // Start out with a failed state
                updateStatus = Convert.ToInt16(await _dao.Update(emp));

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }

        public async Task<int> Delete()
        {
            try
            {
                // DAO will return # of rows deleted
                return await _dao.Delete(Id!);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

    }
}
