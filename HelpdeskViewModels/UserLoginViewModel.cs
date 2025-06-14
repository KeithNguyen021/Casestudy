using HelpdeskDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;


namespace HelpdeskViewModels
{
    public class UserLoginViewModel
    {
        private readonly UserLoginDAO _dao;
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty; 
        public string UserPassword { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string? Timer { get; set; }
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public UserLoginViewModel()
        {
            _dao = new UserLoginDAO();
        }

        public async Task<int?> AuthenticateUser()
        {
            var user = await _dao.GetByEmail(Email);
            if (user == null || user.UserPassword != UserPassword)
                return null; 

            return user.RoleId; 
        }


        public async Task<int> RegisterUser()
        {
            UserLogin newUser = new UserLogin
            {
                Email = Email,
                UserPassword = UserPassword 
            };
            return await _dao.AddUser(newUser);
        }

        public async Task<List<UserLoginViewModel>> GetAll()
        {
            List<UserLoginViewModel> allVms = new();
            try
            {
                List<UserLogin> allEmployees = await _dao.GetAll();
                foreach (UserLogin emp in allEmployees)
                {
                    UserLoginViewModel empVm = new()
                    {
                        Email = emp.Email,
                        Id = emp.Id,
                        UserPassword = emp.UserPassword,
                        RoleId = emp.RoleId,
                        CustomerId = emp.CustomerId,
                        EmployeeId = emp.EmployeeId,
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

        public async Task Add()
        {
            try
            {
                UserLogin user = new()
                {
                    Email = Email,
                    UserPassword = UserPassword,
                    RoleId = RoleId,
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

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                UserLogin user = new()
                {
                    Email = Email,
                    Id = Id,
                    UserPassword = UserPassword,
                    RoleId = RoleId,
                    CustomerId = CustomerId,
                    EmployeeId = EmployeeId,
                    Timer = Convert.FromBase64String(Timer!)
                };

                updateStatus = Convert.ToInt16(await _dao.Update(user));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }
        public async Task GetByEmail()
        {
            try
            {
                UserLogin emp = await _dao.GetByEmail(Email);
                CustomerId = emp.CustomerId;
                EmployeeId = emp.EmployeeId;
                UserPassword = emp.UserPassword;
                Email = emp.Email;
                Id = emp.Id;
                RoleId = emp.RoleId;
                Timer = Convert.ToBase64String(emp.Timer);
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Email = "not found";
            }
            catch (Exception ex)
            {
                Email = "not found";
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
