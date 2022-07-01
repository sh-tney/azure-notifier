global using System;
global using System.IO;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using Newtonsoft.Json;
global using IO.ClickSend.Client;
global using IO.ClickSend.ClickSend.Api;
global using IO.ClickSend.ClickSend.Model;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Azure.WebJobs;
global using Microsoft.Azure.WebJobs.Extensions.Http;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging;
global using sh.tney.Models;

namespace sh.tney;

/// <summary>
/// Static class to contain global constants/shared objects, as well as house global using statements.
/// </summary>
public static class Globals {
    public static readonly Configuration clickSendConfig = new Configuration {
        Username = Environment.GetEnvironmentVariable("CLICKSEND_API_USERNAME"),
        Password = Environment.GetEnvironmentVariable("CLICKSEND_API_PASSWORD")
    };

    public static readonly SMSApi clickSendSMSApi = new SMSApi(clickSendConfig);
}