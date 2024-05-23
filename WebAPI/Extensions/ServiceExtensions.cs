using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApiVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);

                //si no se especifica la version, con esto la especificara por defecto.
                config.AssumeDefaultVersionWhenUnspecified = true;

                config.ReportApiVersions = true;
            });
        }
    }
}
