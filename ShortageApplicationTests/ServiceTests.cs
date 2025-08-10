using ShortageApplication.Services;
using ShortageApplication.Types;

namespace ShortageApplicationTests.ServiceTests
{
    [TestClass]
    [DoNotParallelize]
    public class UserServiceTests
    {
        private readonly string usersFile = "users.json";

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(usersFile))
            {
                File.Delete(usersFile);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(usersFile))
            {
                File.Delete(usersFile);
            }
        }

        [TestMethod]
        public void UserRegistered_NewUser_ShouldRegisterSuccessfully()
        {
            UserService service = new UserService();
            (bool success, string message, User? user) = service.UserRegistered("Alice", "123", false);

            Assert.IsTrue(success);
            Assert.AreEqual("Registration successful", message);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void UserRegistered_DuplicateUser_ShouldFail()
        {
            UserService service = new UserService();
            (bool success1, _, _) = service.UserRegistered("Alice", "123", false);
            (bool success2, string message2, User? user2) = service.UserRegistered("Alice", "1234", true);

            Assert.IsTrue(success1);
            Assert.IsFalse(success2);
            Assert.AreEqual("User 'Alice' already exists", message2);
            Assert.IsNull(user2);
        }

        [TestMethod]
        public void UserLoggedIn_ValidCredentials_ShouldLoginSuccessfully()
        {
            UserService service = new UserService();
            service.UserRegistered("Bob", "123", false);

            (bool success, string message, User? user) = service.UserLoggedIn("Bob", "123");

            Assert.IsTrue(success);
            Assert.AreEqual("Login successful", message);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void UserLoggedIn_InvalidUsername_ShouldFail()
        {
            UserService service = new UserService();

            (bool success, string message, User? user) = service.UserLoggedIn("NonExistent", "pass");

            Assert.IsFalse(success);
            Assert.AreEqual("User does not exist", message);
            Assert.IsNull(user);
        }

        [TestMethod]
        public void UserLoggedIn_WrongPassword_ShouldFail()
        {
            UserService service = new UserService();
            service.UserRegistered("Charlie", "rightpass", false);

            (bool success, string message, User? user) = service.UserLoggedIn("Charlie", "wrongpass");

            Assert.IsFalse(success);
            Assert.AreEqual("Password does not match", message);
            Assert.IsNull(user);
        }

        [TestMethod]
        public void UserExists_ExistingUser_ShouldReturnTrue()
        {
            var service = new UserService();
            service.UserRegistered("Dave", "password", false);

            bool exists = service.UserExists("Dave");

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void UserExists_NonExistingUser_ShouldReturnFalse()
        {
            var service = new UserService();

            bool exists = service.UserExists("Eve");

            Assert.IsFalse(exists);
        }

    }

    [TestClass]
    [DoNotParallelize]
    public class ShortageServiceTests
    {
        private readonly string testFilePath = "shortages.json";

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestMethod]
        public void AddShortage_NewShortage_ShouldAddSuccessfully()
        {
            ShortageService shortageService = new ShortageService();

            Shortage shortage = new Shortage(
                "Laptop",
                "Alice",
                "meeting room",
                "Electronics",
                3,
                DateTime.Today);

            string result = shortageService.AddShortage(shortage);

            Assert.AreEqual("Shortage added successfully", result);
        }

        [TestMethod]
        public void AddShortage_ExistingShortageLowerPriority_ShouldNotUpdate()
        {
            ShortageService shortageService = new ShortageService();

            Shortage shortage1 = new Shortage(
                "Laptop",
                "Alice",
                "meeting room",
                "Electronics",
                5,
                DateTime.Today);

            Shortage shortage2 = new Shortage(
                "Laptop",
                "Alice",
                "meeting room",
                "Electronics",
                3,
                DateTime.Today);

            string firstAdd = shortageService.AddShortage(shortage1);
            string secondAdd = shortageService.AddShortage(shortage2);

            Assert.AreEqual("Shortage added successfully", firstAdd);
            Assert.AreEqual("Shortage already exists with equal or higher priority", secondAdd);
        }

        [TestMethod]
        public void AddShortage_ExistingShortageHigherPriority_ShouldUpdate()
        {
            ShortageService shortageService = new ShortageService();

            Shortage shortage1 = new Shortage(
                "Laptop",
                "Alice",
                "meeting room",
                "Electronics",
                2,
                DateTime.Today);

            Shortage shortage2 = new Shortage(
                "Laptop",
                "Alice",
                "meeting room",
                "Electronics",
                5,
                DateTime.Today);

            string firstAdd = shortageService.AddShortage(shortage1);
            string secondAdd = shortageService.AddShortage(shortage2);

            Assert.AreEqual("Shortage added successfully", firstAdd);
            Assert.AreEqual("Shortage updated with higher priority", secondAdd);
        }

        [TestMethod]
        public void DeleteShortage_UserIsOwner_ShouldDeleteSuccessfully()
        {
            ShortageService shortageService = new ShortageService();

            Shortage shortage = new Shortage(
                "Chair",
                "Bob",
                "meeting room",
                "Other",
                1,
                DateTime.Today);

            shortageService.AddShortage(shortage);

            User user = new User("Bob", "123", false);

            string result = shortageService.DeleteShortage("Chair", "meeting room", user);

            Assert.AreEqual("Shortage deleted successfully", result);
        }

        [TestMethod]
        public void DeleteShortage_UserNotOwnerOrAdmin_ShouldDenyPermission()
        {
            ShortageService shortageService = new ShortageService();

            Shortage shortage = new Shortage(
                "Chair",
                "Bob",
                "meeting room",
                "Other",
                1,
                DateTime.Today);

            shortageService.AddShortage(shortage);

            User user = new User("Alice", "123", false);

            string result = shortageService.DeleteShortage("Chair", "meeting room", user);

            Assert.AreEqual("You do not have permission to delete this shortage", result);
        }

        [TestMethod]
        public void DeleteShortage_AdminUser_ShouldDeleteSuccessfully()
        {
            ShortageService shortageService = new ShortageService();

            Shortage shortage = new Shortage(
                "Chair",
                "Bob",
                "meeting room",
                "Other",
                1,
                DateTime.Today);

            shortageService.AddShortage(shortage);

            User adminUser = new User("Admin", "pass", true);

            string result = shortageService.DeleteShortage("Chair", "meeting room", adminUser);

            Assert.AreEqual("Shortage deleted successfully", result);
        }

        [TestMethod]
        public void DeleteShortage_NotFound_ShouldReturnNotFound()
        {
            ShortageService shortageService = new ShortageService();

            User user = new User("Bob", "123", false);

            string result = shortageService.DeleteShortage("NonExistent", "meeting room", user);

            Assert.AreEqual("Shortage not found", result);
        }
    }
}
