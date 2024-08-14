using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Authorization
{
    public class UserService
    {
        private readonly traveltestContext context;

        public UserService(traveltestContext context)
        {
            this.context = context; 
        }

        public User GetUserById(int id)
        {
            var user = context.Users.Where(u => u.Equals(id)).FirstOrDefault();
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}
