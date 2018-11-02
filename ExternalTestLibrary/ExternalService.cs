namespace AspNetCoreDemo.ExternalTestLibrary
{
    public class ExternalService : IExternalService
    {
        public string GetResponseText()
        {
           return "Hello from external service";
        }
    }
}
