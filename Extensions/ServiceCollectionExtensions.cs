namespace EasyClaimsCore.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("EClaimsPolicy", policy =>
                {
                    var origins = configuration["Cors:Origins"]?.Split(',') ?? new[] { "*" };
                    var headers = configuration["Cors:Headers"]?.Split(',') ?? new[] { "*" };
                    var methods = configuration["Cors:Methods"]?.Split(',') ?? new[] { "*" };

                    if (origins.Contains("*"))
                        policy.AllowAnyOrigin();
                    else
                        policy.WithOrigins(origins);

                    if (headers.Contains("*"))
                        policy.AllowAnyHeader();
                    else
                        policy.WithHeaders(headers);

                    if (methods.Contains("*"))
                        policy.AllowAnyMethod();
                    else
                        policy.WithMethods(methods);
                });
            });

            return services;
        }
    }
}