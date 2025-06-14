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
    public class RoleViewModel
    {
        private readonly RoleDAO _dao;
        private readonly UserLoginDAO _userdao;

        public int? Id { get; set; }
        public string? RoleName { get; set; } = string.Empty;

        public RoleViewModel()
        {
            _dao = new RoleDAO();
            _userdao = new UserLoginDAO();
        }
        public async Task GetById()
        {
            try
            {
                Role emp = await _dao.GetById(Id!.Value);
                Id = emp.Id;
                RoleName = emp.RoleName;

            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                RoleName = "not found";
            }
            catch (Exception ex)
            {
                RoleName = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
        public async Task<List<RoleViewModel>> GetAll()
        {
            List<RoleViewModel> allVms = new();
            try
            {
                List<Role> allEmployees = await _dao.GetAll();
                foreach (Role emp in allEmployees)
                {
                    RoleViewModel empVm = new()
                    {
                        RoleName = emp.RoleName,
                        Id = emp.Id,
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
