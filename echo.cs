using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace NeedsSoySauce.Functions
{

    public static class echo
    {

        // RFC 2616 suggests using carriage return and newline in HTTP requests
        const string CRLF = "\r\n";

        [FunctionName("echo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, Route = null)] HttpRequest req,
            ILogger log)
        {   

            var builder = new StringBuilder();

            // Method Host:Post/PathBase/Path?QueryParams Protocol
            builder.Append(req.Method + " " + req.Host + req.PathBase + req.Path + req.QueryString + " " + req.Protocol);
            builder.Append(CRLF);

            // Header: Value
            List<string> headers = req.Headers.Select(header => $"{header.Key}: {header.Value}").ToList();
            headers.Sort();
            builder.AppendJoin(CRLF, headers);
            builder.Append(CRLF);
            
            // Empty line between headers and body
            builder.Append(CRLF); 

            // Body
            builder.Append(await new StreamReader(req.Body).ReadToEndAsync());

            string responseMessage = builder.ToString();

            log.LogInformation(responseMessage);

            return new OkObjectResult(responseMessage);
        }
    }
}
