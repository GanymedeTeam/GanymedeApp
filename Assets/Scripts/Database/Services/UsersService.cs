using SQLite;
using System.Collections.Generic;

public class UsersService
{
    private SQLiteConnection db;

    public UsersService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateUser(Users user)
    {
        db.Insert(user);
    }

    public Users GetUserByEmail(string email)
    {
        return db.Table<Users>().FirstOrDefault(u => u.email == email);
    }

    public Users GetUserById(int id)
    {
        return db.Table<Users>().FirstOrDefault(u => u.id == id);
    }

    public void UpdateUser(Users user)
    {
        db.Update(user);
    }

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

    public List<Users> GetAllUsers()
    {
        return db.Table<Users>().ToList();
    }
}
