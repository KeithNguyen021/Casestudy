using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class ProblemDAO
    {
        readonly IRepository<Problem> _repo;
        readonly IRepository<Employee> _employeeRepo;
        readonly IRepository<Call> _callRepo;


        public ProblemDAO()
        {
            _repo = new HelpdeskRepository<Problem>();
            _employeeRepo = new HelpdeskRepository<Employee>();
            _callRepo = new HelpdeskRepository<Call>();
        }
        public async Task<Problem> GetByDescription(string? description)
        {
            Problem? selectedPromlem;
            try
            {
                selectedPromlem = await _repo.GetOne(prob => prob.Description == description);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return selectedPromlem!;
        }

        public async Task<List<Problem>> GetAll()
        {
            List<Problem> allProblema;
            try
            {
                allProblema = await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allProblema;
        }

    }
}
