using SQLite;
using System.Collections.Generic;

public class UserService
{
    private SQLiteConnection db;

    public UserService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
        db.CreateTable<User>();  // Ensure the User table is created
    }

    // Create a new user
    public void CreateUser(User user)
    {
        db.Insert(user);
    }

    // Retrieve a user by email
    public User GetUserByEmail(string email)
    {
        return db.Table<User>().FirstOrDefault(u => u.Email == email);
    }

    // Retrieve a user by ID
    public User GetUserById(int id)
    {
        return db.Table<User>().FirstOrDefault(u => u.Id == id);
    }

    // Update an existing user
    public void UpdateUser(User user)
    {
        db.Update(user);
    }

    // Delete a user by ID
    public void DeleteUser(int id)
    {
        var user = GetUserById(id);
        if (user != null)
        {
            db.Delete(user);
        }
    }

    // Authenticate a user
    // public bool AuthenticateUser(string email, string password)
    // {
    //     var user = GetUserByEmail(email);
    //     return user != null && user.VerifyPassword(password);
    // }

    // List all users
    public List<User> GetAllUsers()
    {
        return db.Table<User>().ToList();
    }
}
