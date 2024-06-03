using SQLite;
using System.Collections.Generic;

public class UsersService
{
    private SQLiteConnection db;

    public UsersService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
        db.CreateTable<Users>();  // Ensure the User table is created
    }

    // Create a new user
    public void CreateUser(Users user)
    {
        db.Insert(user);
    }

    // Retrieve a user by email
    public Users GetUserByEmail(string email)
    {
        return db.Table<Users>().FirstOrDefault(u => u.email == email);
    }

    // Retrieve a user by ID
    public Users GetUserById(int id)
    {
        return db.Table<Users>().FirstOrDefault(u => u.id == id);
    }

    // Update an existing user
    public void UpdateUser(Users user)
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
    public List<Users> GetAllUsers()
    {
        return db.Table<Users>().ToList();
    }
}
