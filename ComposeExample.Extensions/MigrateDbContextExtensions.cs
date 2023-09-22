using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension class used for migration related methods
    /// </summary>
    public static class MigrateDbContextExtensions
    {
        /// <summary>
        /// Migrate and seed the DbContext using the seed delegate.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="hostTask">The task that can be awaited to get the host.</param>
        /// <param name="seeder">The seed delegate.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public static async Task<IHost> MigrateDbContext<TContext>(this Task<IHost> hostTask, Func<TContext, IServiceProvider, Task> seeder)
            where TContext : DbContext
        {
            var host = await hostTask;
            return await host.MigrateDbContext(seeder);
        }

        /// <summary>
        /// Migrate and seed the DbContext using the seed delegate.
        /// </summary>
        /// <typeparam name="THost">The <see cref="IHost"/> type</typeparam>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="hostTask">The task that can be awaited to get the host.</param>
        /// <param name="seeder">The seed delegate.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public static async Task<IHost> MigrateDbContext<TContext>(this Task<IHost> hostTask, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            var host = await hostTask;
            return await host.MigrateDbContext(seeder);
        }

        /// <summary>
        /// Migrate the DbContext.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="hostTask">The task that can be awaited to get the host.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public static async Task<IHost> MigrateDbContext<TContext>(this Task<IHost> hostTask)
            where TContext : DbContext
        {
            var host = await hostTask;
            return await host.MigrateDbContext<TContext>();
        }

        /// <summary>
        /// Migrate the DbContext
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="host">The host that will execute the migration.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public static Task<IHost> MigrateDbContext<TContext>(this IHost host)
            where TContext : DbContext
        {
            return host.MigrateDbContext<TContext>((p1, p2) => Task.CompletedTask);
        }

        /// <summary>
        /// Migrate and seed the DbContext using the seed delegate.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="host">The host that will execute the migration.</param>
        /// <param name="seeder">The seed delegate.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public static Task<IHost> MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            Task asyncSeeder(TContext c, IServiceProvider s)
            {
                seeder(c, s);
                return Task.CompletedTask;
            }
            return host.MigrateDbContext<TContext>(asyncSeeder);
        }

        /// <summary>
        /// Migrate and seed the DbContext using the seed delegate.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type</typeparam>
        /// <param name="host">The host that will execute the migration.</param>
        /// <param name="seeder">The seed delegate.</param>
        /// <returns>A task that represents the asynchronous migration operation.</returns>
        public static Task<IHost> MigrateDbContext<TContext>(this IHost host, Func<TContext, IServiceProvider, Task> seeder)
            where TContext : DbContext
        {
            var retry = Policy.Handle<SqlException>()
                .WaitAndRetryAsync(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(8),
                });

            return _migrateDbContext(host, retry, seeder);
        }

        private static async Task<IHost> _migrateDbContext<TContext>(this IHost host, IAsyncPolicy retryPolicy, Func<TContext, IServiceProvider, Task> seeder)
            where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {contextType}", typeof(TContext));

                    await retryPolicy
                        .ExecuteAsync(async () =>
                        {
                            await context.Database.MigrateAsync().ConfigureAwait(false);
                            await seeder(context, services).ConfigureAwait(false);
                        })
                        .ConfigureAwait(false);


                    logger.LogInformation("Migrated database associated with context {contextType}", typeof(TContext));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {contextType}", typeof(TContext));
                }
            }

            return host;
        }
    }
}
