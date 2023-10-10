namespace BlazinRoleGame.Data;

public class User
{
    public User(){}

    public User(int UserID, string FirstName, string? LastName)
    {
        this.UserID = UserID;
        this.FirstName = FirstName;
        this.LastName = LastName;
    }

    public int UserID {get; set;}

    public string FirstName {get; set;} = string.Empty;

    public string? LastName {get; set;}
}