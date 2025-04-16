using Coravel;
using Core.Encryption;
using Core.Models;
using dordle.common.service.Extensions;
using Marten;
using Serilog;
using Weasel.Core;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add JWT authentication supporting the "Client Credentials" flow
            builder.Services.AddClientCredentialsAuthentication(builder.Configuration);
            builder.Services.AddClientCredentialsAuthorization(builder.Configuration);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenWithClientCredentials(builder.Configuration, options =>
            {
                options.IncludeXmlComments(string.Format(@"{0}\Api.xml", System.AppDomain.CurrentDomain.BaseDirectory));
            });

            builder.Services.AddCache();

            // This is the absolute, simplest way to integrate Marten into your
            // .NET application with Marten's default configuration
            builder.Services.AddMarten(options =>
            {
                // Establish the connection string to your Marten database
                options.Connection(builder.Configuration.GetConnectionString("db_connection")!);

                // Specify that we want to use STJ as our serializer
                //options.UseSystemTextJsonForSerialization();
                var encrypt = builder.Configuration.GetSection(EncryptionOptions.ConfigKey).Get<EncryptionOptions>()!;
                options.UseEncryptionRulesForProtectedInformation(new AesEncryptionService(encrypt.Key, encrypt.Salt));

                options.Schema.For<Employment>()
                    .AddEncryptionRuleForProtectedInformation(x => x.Name)
                    .AddEncryptionRuleForProtectedInformation(x => x.Designation)
                    .AddEncryptionRuleForProtectedInformation(x => x.StartDate)
                    .AddEncryptionRuleForProtectedInformation(x => x.TerminationDate)
                    .AddEncryptionRuleForProtectedInformation(x => x.EmploymentTypeCode)
                    .AddEncryptionRuleForProtectedInformation(x => x.ISOA3CountryCode);
                // If we're running in development mode, let Marten just take care
                // of all necessary schema building and patching behind the scenes
                if (builder.Environment.IsDevelopment())
                {
                    options.AutoCreateSchemaObjects = AutoCreate.All;
                }
            });

            builder.Host.UseSerilog((hostingContext, loggerConfig) =>
            {
                loggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
                loggerConfig.Enrich.FromLogContext();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "internal-employment-api.Api");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.UseSerilogRequestLogging();
            app.Run();
        }
    }
}