using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class SystemTextJsonResult : ContentResult
    {
        private const string ContentTypeApplicationJson = "application/json";

        public SystemTextJsonResult(object value, JsonSerializerOptions options = null)
        {
            ContentType = ContentTypeApplicationJson;
            Content = options == null ? JsonSerializer.Serialize(value) : JsonSerializer.Serialize(value, options);
        }
    }
}
