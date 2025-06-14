using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class UserLoginDAO
    {
        readonly IRepository<UserLogin> _repo;

        public UserLoginDAO()
        {
            _repo = new HelpdeskRepository<UserLogin>();
        }

        public async Task<UserLogin?> GetByEmail(string email)
        {
            var users = await _repo.GetAll();
            return users.FirstOrDefault(u => u.Email == email);
        }

        public async Task<int> AddUser(UserLogin newUser)
        {
            await _repo.Add(newUser);
            return newUser.Id;
        }

        public async Task<List<UserLogin>> GetAll()
        {
            List<UserLogin> allProblema;
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

        public async Task<int> Add(UserLogin newUser)
        {
            try
            {
                await _repo.Add(newUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return newUser.Id;
        }

        public async Task<UpdateStatus> Update(UserLogin updatedUser)
        {
            UpdateStatus status;

            try
            {
                status = await _repo.Update(updatedUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return status;
        }
        public async Task<int> Delete(string email)
        {
            int deletedRecords = -1;
            try
            {
                var user = await _repo.GetAll(); 
                var userToDelete = user.FirstOrDefault(u => u.Email == email); 

                if (userToDelete == null)
                {
                    Debug.WriteLine($"User with email {email} not found.");
                    return 0; 
                }

                deletedRecords = await _repo.Delete(userToDelete.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return deletedRecords;
        }


    }
}
