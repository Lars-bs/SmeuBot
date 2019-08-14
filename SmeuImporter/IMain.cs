using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmeuImporter.Services;
using SmeuImporter.Services.Implementation;

namespace SmeuImporter
{
    internal interface IMain
    {
        Task ExecuteAsync(string[] args, IConfiguration configuration);
    }

    internal class Main:IMain
    {
        private readonly ILogger<Main> logger;
        private readonly ISmeuEvaluationService smeuEvaluationService;
        private IEnumerable<Error>? parseErrors;
        private CommandLineOptions? options;

        public Main(ILogger<Main> logger, ISmeuEvaluationService smeuEvaluationService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.smeuEvaluationService = smeuEvaluationService 
                                         ?? throw new ArgumentNullException(nameof(smeuEvaluationService));
        }
        public async Task ExecuteAsync(string[] args, IConfiguration configuration)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            Console.WriteLine("---------- SmeuImporter ----------");
            Console.WriteLine("Action: Parsing CommandLine Arguments.");
            
            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithNotParsed(errors => this.parseErrors = errors)
                .WithParsed(parsedOptions => this.options = parsedOptions );
            if ( options is null || parseErrors != null && parseErrors.Any())
            {
                PrintParseErrors();
                return;
            }
            
            Console.WriteLine("Selected WhatsApp log directory: {0}", options.WhatsappLogDirectory);
            Console.WriteLine("Selected database connection: {0}", configuration.GetValue<string>("ConnectionString") );
            Console.WriteLine("Starting logging");
            
            logger.LogInformation("---- LOGGING STARTED ----");
            await smeuEvaluationService.EvaluateChat(options.WhatsappLogDirectory);
            logger.LogInformation("---- LOGGING STOPPED ----");
            
            Console.WriteLine("Logging stopped");
            Console.WriteLine("---- SmeuImporter is finished ----");
        }

        private void PrintParseErrors()
        {
            Console.WriteLine("Not all commandline arguments where provided correctly, please review the errors and restart the program.");
            if(parseErrors is null) return;
            foreach (var error in parseErrors)
            {
                Console.WriteLine(error);
            }
        }
    }
}