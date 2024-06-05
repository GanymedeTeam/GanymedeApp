using SQLite;

public class Users
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [Unique, MaxLength(255)]
    public string email { get; set; }

    public string password { get; set; }
}
