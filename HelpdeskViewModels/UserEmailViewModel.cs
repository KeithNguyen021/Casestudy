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
    public class UserEmailViewModel
    {
        private readonly UserEmailDAO _dao;
        public int? Id { get; set; }
        public string Email { get; set; } = string.Empty;

        public UserEmailViewModel()
        {
            _dao = new UserEmailDAO();
        }




        public async Task Add()
        {
            try
            {
                UserEmail user = new()
                {
                    Email = Email,

                };
                Id = await _dao.Add(user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int> Delete()
        {
            try
            {
                return await _dao.Delete(Email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
    }
}
