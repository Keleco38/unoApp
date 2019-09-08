using System;
using System.Collections.Generic;
using System.Linq;
using EntityObjects;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepository()
        {
            _users = new List<User>();
        }


        public User GetUserByConnectionId(string connectionId)
        {
            var user = _users.First(x => x.ConnectionId == connectionId);
            return user;
        }

        public bool UserExistsByName(string name)
        {
            return _users.Exists(x => x.Name == name);
        }

        public bool UserExistsByConnectionId(string connectionId)
        {
            return _users.Exists(x => x.ConnectionId == connectionId);
        }

        public User GetUserByName(string name)
        {
            var user = _users.First(x => x.Name == name);
            return user;
        }


        public void RemoveUser(User user)
        {
            _users.Remove(user);
        }


        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public IReadOnlyList<User> GetAllUsers()
        {
            return _users.AsReadOnly();
        }


    }
}
