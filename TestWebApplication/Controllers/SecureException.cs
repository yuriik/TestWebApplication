namespace TestWebApplication.Controllers
{
    public class SecureException : Exception
    {
        public SecureException(string message) : base(message)
        {
        }
    }
}
