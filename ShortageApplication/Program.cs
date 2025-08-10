using ShortageApplication.Types;
using ShortageApplication.Services;

namespace ShortageApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {

            UserService userService = new UserService();
            ShortageService shortageService = new ShortageService();

            User? currentUser = null;

            Console.WriteLine("Welcome to shortage management application");

            while (currentUser == null)
            {
                Console.WriteLine("Press 'r' for register and 'l' for login");
                string action = Console.ReadLine() ?? "";

                try
                {
                    if (action == "l")
                    {
                        Console.Write("Enter username: ");
                        string username = Console.ReadLine() ?? "";

                        Console.Write("Enter password: ");
                        string password = Console.ReadLine() ?? "";

                        (bool success, string message, User? user) = userService.UserLoggedIn(username, password);
                        Console.WriteLine(message);

                        if (success)
                        {
                            currentUser = user;
                        }
                    }
                    else if (action == "r")
                    {
                        Console.Write("Enter new username: ");
                        string username = Console.ReadLine() ?? "";

                        Console.Write("Enter password: ");
                        string password = Console.ReadLine() ?? "";

                        Console.Write("Register as administrator? Yes - 'y', No - press Enter: ");
                        string adminMessage = Console.ReadLine() ?? "";

                        bool isAdmin = adminMessage == "y";

                        (bool success, string message, User? user) = userService.UserRegistered(username, password, isAdmin);
                        Console.WriteLine(message);

                        if (success)
                        {
                            currentUser = user;
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


            }

            bool exit = false;

            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("Shortage Management Menu:");
                Console.WriteLine("1 - Add a shortage");
                Console.WriteLine("2 - List all shortages");
                Console.WriteLine("3 - Remove a shortage");
                Console.WriteLine("4 - List filtered shortages");
                Console.WriteLine("0 - Exit");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter title: ");
                        string title = Console.ReadLine() ?? "";

                        string name = currentUser.Name;

                        Console.Write("Enter room (kitchen, meeting room, bathroom): ");
                        string room = Console.ReadLine() ?? "";

                        Console.Write("Enter category (Electronics, Food, Other): ");
                        string category = Console.ReadLine() ?? "";

                        Console.Write("Enter priority (1-10): ");
                        string priorityInput = Console.ReadLine() ?? "";

                        DateTime currentDate = DateTime.Today;

                        if (!int.TryParse(priorityInput, out int priority))
                        {
                            Console.WriteLine("Invalid priority. Please enter a number between 1 and 10.");
                            break;
                        }

                        try
                        {
                            Shortage shortage = new Shortage(title, name, room, category, priority, currentDate);
                            string messageAdding = shortageService.AddShortage(shortage);
                            Console.WriteLine(messageAdding);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;

                    case "2":
                        shortageService.ListShortages(currentUser);
                        break;

                    case "3":
                        Console.Write("Enter title: ");
                        string titleRemoved = Console.ReadLine() ?? "";

                        Console.Write("Enter room (kitchen, meeting room, bathroom): ");
                        string roomRemoved = Console.ReadLine() ?? "";

                        string messageRemoving = shortageService.DeleteShortage(titleRemoved, roomRemoved, currentUser);
                        Console.WriteLine(messageRemoving);
                        break;

                    case "4":
                        Console.Write("Enter title to filter or press Enter to skip: ");
                        string? titleInput = Console.ReadLine();
                        string? titleFilter = string.IsNullOrWhiteSpace(titleInput) ? null : titleInput;

                        Console.Write("Enter createdOn filter date: 'yyyy-mm-dd' or press Enter to skip: ");
                        string? createdOnDateInput = Console.ReadLine();
                        DateTime? createdOnDateFilter = null;
                        if (!string.IsNullOrWhiteSpace(createdOnDateInput) &&
                            DateTime.TryParse(createdOnDateInput, out DateTime parsedOnDate))
                        {
                            createdOnDateFilter = parsedOnDate;
                        }

                        Console.Write("Enter createdTo filter date: 'yyyy-mm-dd' or press Enter to skip: ");
                        string? createdToDateInput = Console.ReadLine();
                        DateTime? createdToDateFilter = null;
                        if (!string.IsNullOrWhiteSpace(createdToDateInput) &&
                            DateTime.TryParse(createdToDateInput, out DateTime parsedToDate))
                        {
                            createdToDateFilter = parsedToDate;
                        }

                        Console.Write("Enter room (kitchen, meeting room, bathroom) or press Enter to skip: ");
                        string? roomInput = Console.ReadLine();
                        string? roomFilter = string.IsNullOrWhiteSpace(roomInput) ? null : roomInput;

                        Console.Write("Enter category (Electronics, Food, Other) or press Enter to skip: ");
                        string? categoryInput = Console.ReadLine();
                        string? categoryFilter = string.IsNullOrWhiteSpace(categoryInput) ? null : categoryInput;

                        shortageService.ListFilteredShortages(currentUser, titleFilter, createdOnDateFilter, createdToDateFilter, categoryFilter, roomFilter);
                        break;

                    case "0":
                        exit = true;
                        break;
                }
            }


        }
    }
}
