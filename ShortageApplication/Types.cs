using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace ShortageApplication.Types
{

    public class Shortage
    {
        public string Title { get; }
        public string Name { get; }
        public string Room { get; }
        public string Category { get; }
        public int Priority { get; }
        public DateTime CreatedOn { get; }

        public Shortage(string title, string name, string room, string category, int priority, DateTime createdOn)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty");

            if (room != "kitchen" && room != "meeting room" && room != "bathroom")
                throw new ArgumentException("Room can either be: kitchen, meeting room, bathroom");

            if (category != "Electronics" && category != "Food" && category != "Other")
                throw new ArgumentException("Category can either be: Electronics, Food, Other");

            if (priority < 1 || priority > 10)
                throw new ArgumentOutOfRangeException("priority is a number from 1 to 10");

            
            Title = title;
            Name = name;
            Room = room;
            Category = category;
            Priority = priority;
            CreatedOn = createdOn;


        }
    }

    public class User
    {
        public string Name { get; }
        public bool IsAdmin { get; }
        public string HashedPassword { get; }
        public User(string name, string password, bool isAdmin = false)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            Name = name;
            IsAdmin = isAdmin;
            HashedPassword = HashPassword(password);
        }

        [JsonConstructor]
        private User(string name, bool isAdmin, string hashedpassword)
        {
            Name = name;
            HashedPassword = hashedpassword;
            IsAdmin = isAdmin;
        }

        public bool CheckPassword(string password)
        {
            return HashedPassword == HashPassword(password);
        }

        private static string HashPassword(string password)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }



}
