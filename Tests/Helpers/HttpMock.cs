﻿using NSubstitute;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcTemplate.Tests.Helpers
{
    public class HttpMock
    {
        public HttpContextBase HttpContextBase
        {
            get;
            private set;
        }

        public HttpContext HttpContext
        {
            get;
            private set;
        }
        static HttpMock()
        {
            RouteTable.Routes
                .MapRoute(
                    "AdministrationMultilingual",
                    "{language}/{area}/{controller}/{action}/{id}",
                    new { area = "Administration", action = "Index", id = UrlParameter.Optional },
                    new { language = "lt", area = "Administration" },
                    new[] { "MvcTemplate.Controllers.Administration" })
                .DataTokens["UseNamespaceFallback"] = false;

            RouteTable.Routes
                .MapRoute(
                    "Administration",
                    "{area}/{controller}/{action}/{id}",
                    new { language = "en", area = "Administration", action = "Index", id = UrlParameter.Optional },
                    new { language = "en", area = "Administration" },
                    new[] { "MvcTemplate.Controllers.Administration" })
                .DataTokens["UseNamespaceFallback"] = false;

            RouteTable.Routes
                .MapRoute(
                    "DefaultMultilingual",
                    "{language}/{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "lt" },
                    new[] { "MvcTemplate.Controllers" })
                .DataTokens["UseNamespaceFallback"] = false;

            RouteTable.Routes
                .MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { language = "en", controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "en" },
                    new[] { "MvcTemplate.Controllers" })
                .DataTokens["UseNamespaceFallback"] = false;
        }

        public HttpMock()
        {
            HttpRequest request = new HttpRequest(String.Empty, "http://localhost:19174/Domain/", String.Empty);
            HttpBrowserCapabilities browser = Substitute.ForPartsOf<HttpBrowserCapabilities>();
            HttpResponse response = new HttpResponse(new StringWriter());
            HttpContext = new HttpContext(request, response);
            browser[Arg.Any<String>()].Returns("true");
            request.Browser = browser;

            HttpRequestBase httpRequestBase = Substitute.ForPartsOf<HttpRequestWrapper>(request);
            httpRequestBase.QueryString.Returns(new NameValueCollection());
            httpRequestBase.ApplicationPath.Returns("/Domain");
            httpRequestBase.QueryString.Add("p", "1");

            HttpContextBase = Substitute.ForPartsOf<HttpContextWrapper>(HttpContext);
            HttpContextBase.Server.Returns(Substitute.For<HttpServerUtilityBase>());
            HttpContextBase.Response.Returns(new HttpResponseWrapper(response));
            HttpContextBase.Request.Returns(httpRequestBase);

            IIdentity identity = Substitute.For<IIdentity>();
            identity.Name.Returns(ObjectFactory.TestId + 1);
            identity.IsAuthenticated.Returns(true);

            IPrincipal user = Substitute.For<IPrincipal>();
            user.Identity.Returns(identity);

            HttpContextBase.User.Returns(user);
            HttpContext.User = user;

            request.RequestContext.RouteData.Values["area"] = "Administration";
            request.RequestContext.RouteData.Values["controller"] = "Accounts";
            request.RequestContext.RouteData.Values["action"] = "Details";
            request.RequestContext.RouteData.Values["language"] = "en";
        }
    }
}
