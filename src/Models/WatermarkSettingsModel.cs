using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.Watermark.Models
{
    public partial class WatermarkSettingsModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.PictureId")]
        [UIHint("Picture")]
        public virtual int PictureId { get; set; }
        public bool PictureId_Override { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.Positions")]
        public int Positions { get; set; }
        public bool Positions_Override { get; set; }
        public List<string> PositionsSelectedValues { get; set; }

        public IList<SelectListItem> PositionsValues { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.Enable")]
        public bool Enable { get; set; }
        public bool Enable_Override { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.Scale")]
        public int Scale { get; set; }
        public bool Scale_Override { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.Transparency")]
        public int Transparency { get; set; }
        public bool Transparency_Override { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.OnlyLargerThen")]
        public int OnlyLargerThen { get; set; }
        public bool OnlyLargerThen_Override { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.ApplyOnProductPictures")]
        public bool ApplyOnProductPictures { get; set; }
        public bool ApplyOnProductPictures_Override { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.ApplyOnCategoryPictures")]
        public bool ApplyOnCategoryPictures { get; set; }
        public bool ApplyOnCategoryPictures_Override { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Watermark.ApplyOnProductAttributeValuePictures")]
        public bool ApplyOnProductAttributeValuePictures { get; set; }
        public bool ApplyOnProductAttributeValuePictures_Override { get; set; }

        public WatermarkSettingsModel()
            : base()
        {
            this.PositionsValues = new List<SelectListItem>();
        }
    }
}
