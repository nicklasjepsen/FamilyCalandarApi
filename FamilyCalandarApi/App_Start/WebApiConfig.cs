﻿using System.Web.Http;
using Microsoft.Practices.Unity;

namespace SystemOut.CalandarApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            // Web API configuration and services
            var container = new UnityContainer();
            container.RegisterInstance<ICalendarCache>(new CalendarCache(), new ContainerControlledLifetimeManager());
            container.RegisterType<ICredentialProvider, CredentialProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<IIcsService, IcsService>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
