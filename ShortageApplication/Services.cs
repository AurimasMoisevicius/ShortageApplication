using ShortageApplication.Types;
using System.Text.Json;

namespace ShortageApplication.Services
{
    public class ShortageService
    {
        private Dictionary<string, Shortage> shortages;

        private readonly string filePath = @"shortages.json";

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public ShortageService()
        {
            shortages = LoadShortages();
        }

        private Dictionary<string, Shortage> LoadShortages()
        {
            if (!File.Exists(filePath))
                return new Dictionary<string, Shortage>();

            string json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<string, Shortage>();

            Dictionary<string, Shortage> shortages =
                JsonSerializer.Deserialize<Dictionary<string, Shortage>>(json)
                ?? new Dictionary<string, Shortage>();

            return shortages;

        }

        private void SaveShortages()
        {
            string json = JsonSerializer.Serialize(shortages, jsonOptions);

            File.WriteAllText(filePath, json);
        }

        public string AddShortage(Shortage shortage)
        {
            string key = (shortage.Title + "-" + shortage.Room).ToLowerInvariant();

            if (shortages.ContainsKey(key))
            {
                Shortage oldShortage = shortages[key];

                if (shortage.Priority > oldShortage.Priority)
                {
                    shortages[key] = shortage;
                    SaveShortages();
                    return "Shortage updated with higher priority";
                }
                else
                {
                    return "Shortage already exists with equal or higher priority";
                }
            }

            shortages[key] = shortage;
            SaveShortages();
            return "Shortage added successfully";
        }

        public string DeleteShortage(string title, string room, User user)
        {
            string key = (title + "-" + room).ToLowerInvariant();

            if (!shortages.ContainsKey(key))
                return "Shortage not found";

            Shortage shortage = shortages[key];

            if (shortage.Name != user.Name && !user.IsAdmin)
                return "You do not have permission to delete this shortage";

            shortages.Remove(key);
            SaveShortages();
            return "Shortage deleted successfully";
        }

        public void ListFilteredShortages(
            User user,
            string? titleFilter = null,
            DateTime? createdFromFilter = null,
            DateTime? createdToFilter = null,
            string? categoryFilter = null,
            string? roomFilter = null)
        {
            IEnumerable<Shortage> query = shortages.Values;

            if (!user.IsAdmin)
            {
                query = query.Where(s => s.Name.Equals(user.Name));
            }

            if (!string.IsNullOrWhiteSpace(titleFilter))
            {
                string titleFilterLower = titleFilter.ToLowerInvariant();
                query = query.Where(s => s.Title.ToLowerInvariant().Contains(titleFilterLower));
            }

            if (createdFromFilter.HasValue)
            {
                query = query.Where(s => s.CreatedOn >= createdFromFilter.Value);
            }
            if (createdToFilter.HasValue)
            {
                query = query.Where(s => s.CreatedOn <= createdToFilter.Value);
            }

            if (!string.IsNullOrWhiteSpace(categoryFilter))
            {
                query = query.Where(s => s.Category.Equals(categoryFilter));
            }

            if (!string.IsNullOrWhiteSpace(roomFilter))
            {
                query = query.Where(s => s.Room.Equals(roomFilter));
            }

            query = query.OrderByDescending(s => s.Priority);

            if (!query.Any())
            {
                Console.WriteLine("No shortages found ");
                return;
            }

            foreach (Shortage shortage in query)
            {
                Console.WriteLine("Title: " + shortage.Title);
                Console.WriteLine("Name: " + shortage.Name);
                Console.WriteLine("Room: " + shortage.Room);
                Console.WriteLine("Category: " + shortage.Category);
                Console.WriteLine("Priority: " + shortage.Priority);
                Console.WriteLine("Created On: " + shortage.CreatedOn.ToString("yyyy-MM-dd"));
                Console.WriteLine("------------------------------------------");
            }

        }

        public void ListShortages(User user)
        {
            IEnumerable<Shortage> query = shortages.Values;

            if (!user.IsAdmin)
            {
                query = query.Where(s => s.Name.Equals(user.Name));
            }

            query = query.OrderByDescending(s => s.Priority);

            if (!query.Any())
            {
                Console.WriteLine("No shortages found ");
                return;
            }

            foreach (Shortage shortage in query)
            {
                Console.WriteLine("Title: " + shortage.Title);
                Console.WriteLine("Name: " + shortage.Name);
                Console.WriteLine("Room: " + shortage.Room);
                Console.WriteLine("Category: " + shortage.Category);
                Console.WriteLine("Priority: " + shortage.Priority);
                Console.WriteLine("Created On: " + shortage.CreatedOn.ToString("yyyy-MM-dd"));
                Console.WriteLine("------------------------------------------");
            }

        }


    }

    public class UserService 
    {
        private Dictionary<string, User> users;

        private readonly string filePath = @"users.json";

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public UserService()
        {
            users = LoadUsers();
        }

        public bool UserExists(string name)
        {
            return users.ContainsKey(name);
        }

        private Dictionary<string, User> LoadUsers()
        {
            if (!File.Exists(filePath))
                return new Dictionary<string, User>();

            string json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<string, User>();

            Dictionary<string, User> users =
                JsonSerializer.Deserialize<Dictionary<string, User>>(json)
                ?? new Dictionary<string, User>();

            return users;

        }
        private void SaveUsers()
        {
            string json = JsonSerializer.Serialize(users, jsonOptions);

            File.WriteAllText(filePath, json);
        }

        public (bool, string, User?) UserRegistered(string name, string password, bool isAdmin)
        {
            if (users.ContainsKey(name))
                return (false, $"User '{name}' already exists", null);

            User newUser = new User(name, password, isAdmin);
            users[name] = newUser;
            SaveUsers();
            return (true, "Registration successful", newUser);
        }

        public (bool, string, User?) UserLoggedIn(string name, string password)
        {
            if (!users.TryGetValue(name, out User? user))
                return (false, "User does not exist", null);

            if (!user.CheckPassword(password))
                return (false, "Password does not match", null);

            return (true, "Login successful", user);
        }

    }
}
