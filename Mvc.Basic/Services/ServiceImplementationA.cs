﻿using AspNetCoreDemo.Mvc.Basic.Contracts;
using Microsoft.Extensions.Logging;

namespace AspNetCoreDemo.Mvc.Basic.Implementations
{
    internal class ServiceImplementationA : IExampleService
    {
        private readonly ILogger _logger;

        public ServiceImplementationA(ILogger<ServiceImplementationA> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public ServiceImplementationA()
        {
            _logger.LogInformation($"{nameof(ServiceImplementationA)} created");
        }

        public void Execute()
        {
            _logger.LogInformation($"{nameof(ServiceImplementationA)} invoked");
        }
    }
}
