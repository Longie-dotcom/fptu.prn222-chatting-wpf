using Application.Interface;
using Infrastructure.ExternalService.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public static class InfrastructureModule
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static void InfrastructureLoggerBase(
            ILogger? logger, string message, Exception? ex = null)
        {
            if (ex == null)
                logger?.LogInformation($"[Infrastructure]: {message}");
            else
                logger?.LogError(ex, $"[Infrastructure]: Error - {message}");
        }

        public static IServiceCollection AddInfrastructure(
                this IServiceCollection services,
                ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new Exception("No logger");

            var logger = loggerFactory.CreateLogger("Infrastructure");

             //======================
             //1.Storage Service
             //======================
            try
            {
                services.AddSingleton<IImageStorage, LocalImageStorage>();
                InfrastructureLoggerBase(logger, "Storage Service successfully configured");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(logger, "Storage Service configuration failed", ex);
                throw;
            }

            return services;
        }
        #endregion
    }
}
