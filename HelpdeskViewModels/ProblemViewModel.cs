using ExercisesDAL;
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
    public class ProblemViewModel
    {
        private readonly ProblemDAO _dao;
        public string? Description { get; set; }
        public string? Timer { get; set; }
        public int? Id { get; set; }


        // constructor
        public ProblemViewModel()
        {
            _dao = new ProblemDAO();
        }
        public async Task GetByEmpDescription()
        {
            try
            {
                Problem emp = await _dao.GetByDescription(Description!);
                Id = emp.Id;
                Description = emp.Description;
                Timer = Convert.ToBase64String(emp.Timer);
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Description = "not found";
            }
            catch (Exception ex)
            {
                Description = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<List<ProblemViewModel>> GetAll()
        {
            List<ProblemViewModel> allVms = new();
            try
            {
                List<Problem> allEmployees = await _dao.GetAll();
                foreach (Problem emp in allEmployees)
                {
                    ProblemViewModel empVm = new()
                    {
                        Description = emp.Description,
                        Id = emp.Id,
                        Timer = Convert.ToBase64String(emp.Timer)
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
    }
}
