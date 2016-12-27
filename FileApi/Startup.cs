using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: OwinStartup(typeof(FileApi.Startup))]

namespace FileApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}