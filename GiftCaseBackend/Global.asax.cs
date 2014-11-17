using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.storage;
using com.shephertz.app42.paas.sdk.csharp.user;
using GiftCaseBackend.Models;
using Newtonsoft.Json;

namespace GiftCaseBackend
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Makes enums serialize as strings instead of integer values
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
            jsonSetting.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = jsonSetting;

            // initializes backend as a service provider
            BaaS.Initialize();
        }
    }
}
