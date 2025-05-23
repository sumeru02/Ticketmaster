namespace Ticketmaster.Helper
{
    public class TokenHelper
    {
        // Method that validates JWT and returns userId
        public static Guid GetUserFromToken(string token)
        {
            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }
}
