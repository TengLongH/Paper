using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using PaperRecognize.AutoMapperConfig;

[assembly: OwinStartup(typeof(PaperRecognize.Startup))]

namespace PaperRecognize
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            new MapperConfig().config();
        }
    }
}
