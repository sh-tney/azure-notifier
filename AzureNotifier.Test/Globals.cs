global using Xunit;
global using Moq;
global using System;
global using System.IO;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using AzureNotifier.Functions;
global using AzureNotifier.Services;
global using AzureNotifier.Services.ClickSend;
global using AzureNotifier.Models;
global using Newtonsoft.Json;
global using IO.ClickSend.Client;
global using IO.ClickSend.ClickSend.Model;

namespace AzureNotifier.Test;