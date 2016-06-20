using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Plugin.Misc.Watermark.Models;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Nop.Plugin.Misc.Watermark.Core
{
    public partial class VJeekPictureService : PictureService
    {
        private static readonly object syncRoot = new object();
        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;
        private readonly IStoreContext _storeContext;
        private readonly WatermarkSettings _watermarkSettings;
        private readonly Image _watermarkImage;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly IRepository<Category> _categoryRepository;

        static VJeekPictureService()
        {

        }

        public VJeekPictureService(IRepository<Picture> pictureRepository,
            IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService, 
            IWebHelper webHelper,
            ILogger logger,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings, 
            IStoreContext storeContext, 
            IRepository<ProductAttributeValue> productAttributeValueRepository, 
            IRepository<Category> categoryRepository)
            : base(pictureRepository, productPictureRepository, settingService, webHelper, logger, dbContext, eventPublisher, mediaSettings)
        {
            _pictureRepository = pictureRepository;
            _productPictureRepository = productPictureRepository;
            _settingService = settingService;
            _webHelper = webHelper;
            _logger = logger;
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
            _mediaSettings = mediaSettings;
            _storeContext = storeContext;
            _productAttributeValueRepository = productAttributeValueRepository;
            _categoryRepository = categoryRepository;

            this._watermarkSettings = (WatermarkSettings)this._settingService.LoadSetting<WatermarkSettings>(_storeContext.CurrentStore.Id);

            var watermarkPicture = this.GetPictureById(this._watermarkSettings.PictureId);
            if (watermarkPicture == null || watermarkPicture.PictureBinary == null)
                return;
            using (MemoryStream ms = new MemoryStream(watermarkPicture.PictureBinary))
            {
                if (ms.Length <= 0)
                    return;
                this._watermarkImage = Image.FromStream(ms);
            }
        }

        private Rectangle Rectangle(int imageWidth, int imageHeight, int watermarkWidth, int watermarkHeight, WatermarkPositions position)
        {
            imageWidth = imageWidth > 0 ? imageWidth : 100;
            imageHeight = imageHeight > 0 ? imageHeight : 100;
            watermarkWidth = watermarkWidth > 0 ? watermarkWidth : 100;
            watermarkHeight = watermarkHeight > 0 ? watermarkHeight : 100;
            int left = 0, top = 0;
            int width = imageWidth / 100 * (int)this._watermarkSettings.Scale;
            int height = watermarkHeight * width / watermarkWidth;

            switch (position)
            {
                case WatermarkPositions.TopLeft:
                    left = 10;
                    top = 10;
                    break;
                case WatermarkPositions.TopRight:
                    left = imageWidth - width - 10;
                    top = 10;
                    break;
                case WatermarkPositions.TopMiddle:
                    left = (imageWidth - width - 10) / 2;
                    top = 10;
                    break;
                case WatermarkPositions.BottomLeft:
                    left = 10;
                    top = imageHeight - height - 10;
                    break;
                case WatermarkPositions.BottomRight:
                    left = imageWidth - width - 10;
                    top = imageHeight - height - 10;
                    break;
                case WatermarkPositions.BottomMiddle:
                    left = (imageWidth - width - 10) / 2;
                    top = imageHeight - height - 10;
                    break;
                case WatermarkPositions.MiddleLeft:
                    left = 10;
                    top = (imageHeight - height - 10) / 2;
                    break;
                case WatermarkPositions.MiddleRight:
                    left = imageWidth - width - 10;
                    top = (imageHeight - height - 10) / 2;
                    break;
                case WatermarkPositions.Center:
                    left = (imageWidth - width - 10) / 2;
                    top = (imageHeight - height - 10) / 2;
                    break;
            }
            return new Rectangle(left > 0 ? left : 10, top > 0 ? top : 10, width > 0 ? width : 100, height > 0 ? height : 100);
        }

        public virtual void PlaceWatermark(ref Graphics g, int width, int height, WatermarkPositions position)
        {
            if (g.DpiX > this._watermarkImage.Width)
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            byte transparency = (byte)this._watermarkSettings.Transparency;
            int watermarkWidth = this._watermarkImage.Width;
            int watermarkHeight = this._watermarkImage.Height;
            Rectangle destRect = Rectangle(width, height, watermarkWidth, watermarkHeight, position);
            float num4 = (float)transparency / 100f;
            if ((double)num4 < 0.0 || (double)num4 > 1.0)
                num4 = 1f;
            float[][] newColorMatrix1 = new float[5][];
            float[][] numArray1 = newColorMatrix1;
            int index1 = 0;
            float[] numArray2 = new float[5];
            numArray2[0] = 1f;
            float[] numArray3 = numArray2;
            numArray1[index1] = numArray3;
            float[][] numArray4 = newColorMatrix1;
            int index2 = 1;
            float[] numArray5 = new float[5];
            numArray5[1] = 1f;
            float[] numArray6 = numArray5;
            numArray4[index2] = numArray6;
            float[][] numArray7 = newColorMatrix1;
            int index3 = 2;
            float[] numArray8 = new float[5];
            numArray8[2] = 1f;
            float[] numArray9 = numArray8;
            numArray7[index3] = numArray9;
            float[][] numArray10 = newColorMatrix1;
            int index4 = 3;
            float[] numArray11 = new float[5];
            numArray11[3] = num4;
            float[] numArray12 = numArray11;
            numArray10[index4] = numArray12;
            float[][] numArray13 = newColorMatrix1;
            int index5 = 4;
            float[] numArray14 = new float[5];
            numArray14[4] = 1f;
            float[] numArray15 = numArray14;
            numArray13[index5] = numArray15;
            ColorMatrix newColorMatrix2 = new ColorMatrix(newColorMatrix1);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(newColorMatrix2);
            imageAttr.SetColorKey(Color.White, Color.White);
            g.DrawImage(this._watermarkImage, destRect, 0, 0, watermarkWidth, watermarkHeight, GraphicsUnit.Pixel);
        }

        public bool ShouldApply(int pictureId)
        {
            bool shouldGenerateWatermark = this._watermarkSettings.Enable;

            if (_categoryRepository.Table.Any(x => x.PictureId == pictureId))
            {
                if (this._watermarkSettings.ApplyOnCategoryPictures)
                    shouldGenerateWatermark = true;
                else return false;
            }
            else if (_productPictureRepository.Table.Any(x => x.PictureId == pictureId))
            {
                if (this._watermarkSettings.ApplyOnProductPictures)
                    shouldGenerateWatermark = true;
                else return false;
            }
            else if (_productAttributeValueRepository.Table.Any(x => x.PictureId == pictureId))
            {
                if (this._watermarkSettings.ApplyOnProductAttributeValuePicturess)
                    shouldGenerateWatermark = true;
                else return false;
            }
            else
            {
                shouldGenerateWatermark = false;
            }
            return shouldGenerateWatermark;
        }

        public override string GetPictureUrl(Picture picture, int targetSize = 0, bool showDefaultPicture = true, string storeLocation = null, PictureType defaultPictureType = PictureType.Entity)
        {
            if (picture != null)
            {
                bool shouldGenerateWatermark = ShouldApply(picture.Id);

                if (!shouldGenerateWatermark)
                    return base.GetPictureUrl(picture, targetSize, showDefaultPicture, storeLocation, defaultPictureType);
            }

            string url = string.Empty;
            byte[] pictureBinary = null;
            if (picture != null)
                pictureBinary = LoadPictureBinary(picture);
            if (picture == null || pictureBinary == null || pictureBinary.Length == 0)
            {
                if (showDefaultPicture)
                    url = GetDefaultPictureUrl(targetSize, defaultPictureType, storeLocation);
                return url;
            }

            string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            string thumbFileName;
            if (picture.IsNew)
            {
                DeletePictureThumbs(picture);

                //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                picture = UpdatePicture(picture.Id,
                    pictureBinary,
                    picture.MimeType,
                    picture.SeoFilename,
                    string.Empty,
                    string.Empty,
                    false,
                    false);
            }
            lock (syncRoot)
            {
                string seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure

                if (targetSize == 0)
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), seoFileName, lastPart) :
                        string.Format("{0}.{1}", picture.Id.ToString("0000000"), lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!File.Exists(thumbFilePath))
                    {
                        if (pictureBinary != null)
                        {
                            using (MemoryStream ms = new MemoryStream(pictureBinary))
                            {
                                if (ms.Length > 0)
                                {
                                    Image img = Image.FromStream(ms);
                                    if (img.Width >= this._watermarkSettings.OnlyLargerThen)
                                    {
                                        Graphics g = Graphics.FromImage(img);

                                        foreach (WatermarkPositions position in Enum.GetValues(typeof(WatermarkPositions)))
                                        {
                                            if (((int)position & (int)this._watermarkSettings.Positions) == (int)position)
                                            {
                                                PlaceWatermark(ref g, img.Width, img.Height, position);
                                            }
                                        }
                                        if (g != null)
                                            g.Dispose();
                                    }
                                    
                                    EncoderParameters encoderParameters = new EncoderParameters();
                                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, this._mediaSettings.DefaultImageQuality);
                                    ImageCodecInfo imageCodecInfo = this.GetImageCodecInfoFromExtension(lastPart) ??
                                                                    this.GetImageCodecInfoFromMimeType(picture.MimeType);
                                    img.Save(thumbFilePath, imageCodecInfo, encoderParameters);
                                }
                            }
                        }
                        //File.WriteAllBytes(thumbFilePath, pictureBinary);
                    }
                }
                else
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}_{2}.{3}", picture.Id.ToString("0000000"), seoFileName, targetSize, lastPart) :
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), targetSize, lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!File.Exists(thumbFilePath))
                    {
                        using (var stream = new MemoryStream(pictureBinary))
                        {
                            Bitmap b = null;
                            try
                            {
                                //try-catch to ensure that picture binary is really OK. Otherwise, we can get "Parameter is not valid" exception if binary is corrupted for some reasons
                                b = new Bitmap(stream);
                            }
                            catch (ArgumentException exc)
                            {
                                _logger.Error(string.Format("Error generating picture thumb. ID={0}", picture.Id), exc);
                            }
                            if (b == null)
                            {
                                //bitmap could not be loaded for some reasons
                                return url;
                            }
                            var newSize = CalculateDimensions(b.Size, targetSize);

                            if (newSize.Width < 1)
                                newSize.Width = 1;
                            if (newSize.Height < 1)
                                newSize.Height = 1;

                            using (var newBitMap = new Bitmap(newSize.Width, newSize.Height))
                            {
                                var g = Graphics.FromImage(newBitMap);

                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.CompositingQuality = CompositingQuality.HighQuality;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);
                                var ep = new EncoderParameters();
                                ep.Param[0] = new EncoderParameter(Encoder.Quality, _mediaSettings.DefaultImageQuality);
                                ImageCodecInfo ici = GetImageCodecInfoFromExtension(lastPart);
                                if (ici == null)
                                    ici = GetImageCodecInfoFromMimeType("image/jpeg");

                                if (newSize.Width >= this._watermarkSettings.OnlyLargerThen)
                                {
                                    foreach (WatermarkPositions position in Enum.GetValues(typeof(WatermarkPositions)))
                                    {
                                        if (((int)position & (int)this._watermarkSettings.Positions) == (int)position)
                                        {
                                            PlaceWatermark(ref g, newSize.Width, newSize.Height, position);
                                        }
                                    }
                                }

                                newBitMap.Save(thumbFilePath, ici, ep);
                                if (g != null)
                                    g.Dispose();
                            }
                            b.Dispose();
                        }
                    }
                }
            }
            url = GetThumbUrl(thumbFileName, storeLocation);
            return url;
        }

        public void ClearThumbs()
        {
            string searchPattern = "*.*";
            string path = this._webHelper.MapPath("~/Content/Images/Thumbs");
            if (!Directory.Exists(path))
                return;

            foreach (string item in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(this.GetThumbLocalPath(item));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        /// <summary>
		/// Returns the first ImageCodecInfo instance with the specified mime type.
		/// </summary>
		/// <param name="mimeType">Mime type</param>
		/// <returns>ImageCodecInfo</returns>
        protected virtual ImageCodecInfo GetImageCodecInfoFromMimeType(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            foreach (var item in info)
                if (item.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                    return item;
            return null;
        }

        /// <summary>
        /// Returns the first ImageCodecInfo instance with the specified extension.
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>ImageCodecInfo</returns>
        protected virtual ImageCodecInfo GetImageCodecInfoFromExtension(string fileExtension)
        {
            fileExtension = fileExtension.TrimStart(".".ToCharArray()).ToLower().Trim();
            switch (fileExtension)
            {
                case "jpg":
                case "jpeg":
                    return GetImageCodecInfoFromMimeType("image/jpeg");
                case "png":
                    return GetImageCodecInfoFromMimeType("image/png");
                case "gif":
                    //use png codec for gif to preserve transparency
                    //return GetImageCodecInfoFromMimeType("image/gif");
                    return GetImageCodecInfoFromMimeType("image/png");
                default:
                    return GetImageCodecInfoFromMimeType("image/jpeg");
            }
        }
    }
}
