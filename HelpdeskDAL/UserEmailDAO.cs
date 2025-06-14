using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class UserEmailDAO
    {
        readonly IRepository<UserEmail> _repo;

        public UserEmailDAO()
        {
            _repo = new HelpdeskRepository<UserEmail>();
        }

        public async Task<int> Add(UserEmail email)
        {
            try
            {
                await _repo.Add(email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return email.Id;
        }

        public async Task<UpdateStatus> Update(UserEmail updatedUser)
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
