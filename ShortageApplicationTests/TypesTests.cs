using ShortageApplication.Types;

namespace ShortageApplicationTests.TypesTests
{
    
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void CreateUser_ValidData_ShouldSetProperties()
        {
            string name = "Alice";
            string password = "123";
            bool isAdmin = true;
            var user = new User(name, password, isAdmin);

            Assert.AreEqual(name, user.Name);
            Assert.AreEqual(isAdmin, user.IsAdmin);
            Assert.IsTrue(user.CheckPassword(password));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateUser_EmptyName_ShouldThrow()
        {
            var user = new User("", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateUser_EmptyPassword_ShouldThrow()
        {
            var user = new User("Name", "");
        }

        [TestMethod]
        public void CheckPassword_WrongPassword_ShouldReturnFalse()
        {
            var user = new User("Bob", "correctpass");
            Assert.IsFalse(user.CheckPassword("wrongpass"));
        }
    }

    [TestClass]
    public class ShortageTests
    {
        [TestMethod]
        public void CreateShortage_ValidData_ShouldSetProperties()
        {
            var shortage = new Shortage(
                "wireless speaker",
                "Alice",
                "meeting room",
                "Electronics",
                5,
                DateTime.Today
            );

            Assert.AreEqual("wireless speaker", shortage.Title);
            Assert.AreEqual("Alice", shortage.Name);
            Assert.AreEqual("meeting room", shortage.Room);
            Assert.AreEqual("Electronics", shortage.Category);
            Assert.AreEqual(5, shortage.Priority);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateShortage_EmptyTitle_ShouldThrow()
        {
            var shortage = new Shortage(
                "",
                "Alice",
                "kitchen",
                "Electronics",
                5,
                DateTime.Today
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateShortage_EmptyName_ShouldThrow()
        {
            var shortage = new Shortage(
                "wireless speaker",
                "",
                "kitchen",
                "Electronics",
                5,
                DateTime.Today
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateShortage_InvalidRoom_ShouldThrow()
        {
            var shortage = new Shortage(
                "wireless speaker",
                "Alice",
                "garage",
                "Electronics",
                5,
                DateTime.Today
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateShortage_InvalidCategory_ShouldThrow()
        {
            var shortage = new Shortage(
                "wireless speaker",
                "Alice",
                "kitchen",
                "Clothing",
                5,
                DateTime.Today
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateShortage_PriorityTooLow_ShouldThrow()
        {
            var shortage = new Shortage(
                "wireless speaker",
                "Alice",
                "kitchen",
                "Electronics",
                0,
                DateTime.Today
            );
        }
    }


}
