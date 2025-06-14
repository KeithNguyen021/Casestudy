using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class RoleDAO
    {
        readonly IRepository<Role> _repo;

        public RoleDAO()
        {
            _repo = new HelpdeskRepository<Role>();
        }

        public async Task<Role> GetById(int id)
        {
            Role? selectedRole;
            try
            {
                selectedRole = await _repo.GetOne(emp => emp.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return selectedRole!;
        }

        public async Task<List<Role>> GetAll()
        {
            List<Role> allProblema;
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
