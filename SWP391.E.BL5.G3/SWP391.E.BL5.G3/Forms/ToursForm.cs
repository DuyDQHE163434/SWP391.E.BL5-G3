using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Forms
{
    public class ToursForm : Tour
    {

        private IFormFile _imageFile;
        [BindProperty]
        public virtual IFormFile ImageFile
        {
            get
            {
                if (_imageFile == null && !string.IsNullOrEmpty(Image) && System.IO.File.Exists(Image))
                {
                    var fileStream = new FileStream(Image, FileMode.Open, FileAccess.Read);
                    _imageFile = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(Image))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/octet-stream" // Bạn có thể thay đổi loại MIME cho phù hợp
                    };
                }
                return _imageFile;
            }
            set
            {
                _imageFile = value;
            }
        }
    }
}
