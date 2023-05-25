using Infrastructure.Models;

namespace ConsoleProgram
{
    public static class InMemoryDatabase
    {
        public static List<User> Users = new List<User>
        {
            new User
            {
                Id = 1,
                Login = "user",
                Password = "128",
                Role = "user"
            },
            new User
            {
                Id = 2,
                Login = "admin",
                Password = "128dm256",
                Role = "admin"

            },
            new User
            {
                Id = 3,
                Login = "security",
                Password = "128dm256sec/",
                Role = "security"
            },
        };
    }
}
