using System.Collections.Generic;
using EntityObjects;

namespace Repository
{
    public interface IUserRepository
    {
        User GetUserByConnectionId(string connectionId);
        User GetUserByName(string name);
        bool UserExistsByName(string name);
        bool UserExistsByConnectionId(string connectionId);
        void RemoveUser(User user);
        void AddUser(User user);
        IReadOnlyList<User> GetAllUsers();
    }
}