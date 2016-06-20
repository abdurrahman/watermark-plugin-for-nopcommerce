using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Media;
using Nop.Plugin.Misc.Watermark.Core;

namespace Nop.Plugin.Misc.Watermark
{
    public partial class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1000;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<VJeekPictureService>().As<IPictureService>().InstancePerHttpRequest();
        }
    }
}
