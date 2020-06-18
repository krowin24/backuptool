using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApplication.Models;
using MyApplication.Operators;
using MyApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication {

    class InjectionService {

        public static IServiceProvider Configure() {

            Configuration config = new Configuration();

            if (!config.LoadFromJsonFile("appconfig.json"))
                return null;

            var Services = new ServiceCollection();

            Services.AddOptions();
            
            Services.Configure<ApplicationOptions>(config.GetSection("Application"));
            Services.Configure<OpenstackOptions>(config.GetSection("Openstack"));

            Services
                .AddSingleton<IOpenStackService, ConoHaService>()
                .AddSingleton<IApplicationOperator, ApplicationOperator>()
            ;

            return Services.BuildServiceProvider();
        }

    }

}
