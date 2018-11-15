using AspNetCoreDemo.Middleware.Basic.Contracts;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.Middleware.Basic.Services
{
    internal class ExampleService : IExampleService
    {
        private readonly ILogger _logger;

        public ExampleService(ILogger<ExampleService> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public ExampleService()
        {
            _logger.LogInformation($"{nameof(ExampleService)} created");
        }

        public void Execute()
        {
            _logger.LogInformation($"{nameof(ExampleService)} invoked");
        }
    }
}
