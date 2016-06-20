using System.Web;
using System.Web.Routing;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Plugin.Misc.Watermark.Core;
using Nop.Plugin.Misc.Watermark.Models;

namespace Nop.Plugin.Misc.Watermark
{
    public class WatermarkPlugin : BasePlugin, IMiscPlugin
    {
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly HttpContextBase _httpContext;

        public WatermarkPlugin(ISettingService settingService,
            ILogger logger,
            HttpContextBase httpContext)
        {
            _settingService = settingService;
            _logger = logger;
            _httpContext = httpContext;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "VJeekWatermark";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.Watermark.Controllers" }, { "area", null } };
        }

        void IPlugin.Install()
        {
            // plugin settings
            var settings = new WatermarkSettings()
            {
                Positions = (int)WatermarkPositions.Center,
                Enable = false
            };

            _settingService.SaveSetting(settings);

            // locale string resources
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.PictureId", "Image for watermark");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.PictureId.Hint", "Upload watermark image for place on images");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.Positions", "Positions of watermark image");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.Positions.Hint", "Select positions where watermark will be placed on image");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.Enable", "Enable watermark");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.Scale", "Image scaling (percents)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.Transparency", "Transparency of watermark image");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.OnlyLargerThen", "Use only for photos larger then Xpx in one dimension");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.ApplyOnProductPictures", "Should apply watermark on products pictures");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.ApplyOnCategoryPictures", "Should apply watermark on category pictures");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Watermark.ApplyOnProductAttributeValuePictures", "Should apply watermark on product attribute value pictures");

            base.Install();
        }

        void IPlugin.Uninstall()
        {
            // delete settings
            _settingService.DeleteSetting<WatermarkSettings>();

            // delete locale string resources
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.PictureId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.PictureId.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.Positions");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.Positions.Hint");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.Enable");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.Scale");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.Transparency");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.OnlyLargerThen");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.ApplyOnProductPictures");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.ApplyOnCategoryPictures");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Watermark.ApplyOnProductAttributeValuePictures");

            ((VJeekPictureService)EngineContext.Current.Resolve<IPictureService>()).ClearThumbs();

            base.Uninstall();
        }
    }
}
