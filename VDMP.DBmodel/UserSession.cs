namespace VDMP.DBmodel
{
    /// <summary>Class used for when a user attempts to log in</summary>
    public class UserSession
    {
        public UserSession(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }

        public string Password { get; }
    }
}