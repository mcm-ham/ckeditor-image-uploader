using Imageflow.Fluent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Ckeditor.Pages
{
    public class ImageBrowserModel : PageModel
    {
        public ImageBrowserModel(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }

        //properties
        private IConfiguration _config;
        private IHostingEnvironment _env;

        [BindProperty]
        public string ResizeMessage { get; set; }

        [BindProperty]
        public string NewDirectoryName { get; set; }

        [BindProperty]
        public string SearchTerms { get; set; }

        [BindProperty]
        public string ResizeWidth { get; set; }

        [BindProperty]
        public string ResizeHeight { get; set; }

        public string ImageAspectRatio { get; set; }

        [BindProperty]
        public IFormFile UploadedImageFile { get; set; }

        [BindProperty]
        public string ImageListValue { get; set; } = "";

        [BindProperty]
        public string ImageUrl { get; set; }

        [BindProperty]
        public string NewImageName { get; set; }

        public bool IsDeleteDirectoryBtnEnabled => ImageFolder != "";
        public bool IsImageBtnsEnabled => ImageList.Any();

        /// <summary>
        /// The URL to the currently selected image folder.
        /// </summary>
        [BindProperty]
        public string ImageFolder { get; set; } = "";

        /// <summary>
        /// The URL to the root image folder.
        /// </summary>
        private string ImageFolderRoot => _config["FilesRoot"] != null ? $"/{_config["ImageRoot"].Trim('/')}/" : "";

        /// <summary>
        /// The file path to the root image folder.
        /// </summary>
        private string FileImageFolderRoot => Path.Combine(_env.WebRootPath, ImageFolderRoot.Trim('/', '\\')) + "\\";

        /// <summary>
        /// The file path to the currently selected image folder.
        /// </summary>
        private string FileImageFolder => Path.Combine(FileImageFolderRoot, ImageFolder ?? "") + "\\";

        // Methods

        public Task OnGetAsync()
        {
            ResizeMessage = "";
            return OnPostChangeDirectoryAsync();
        }

        public IEnumerable<SelectListItem> DirectoryList =>
            new[] { new SelectListItem { Text = "Root", Value = "" } }.Concat(
                Directory.GetDirectories(FileImageFolderRoot)
                    .Select(d => Path.GetFileName(d))
                    .Select(d => new SelectListItem { Text = d, Value = d })
            );

        public IEnumerable<SelectListItem> ImageList {
            get
            {
                var files = Directory.GetFiles(FileImageFolder, "*" + SearchTerms?.Replace(" ", "*") + "*")
                    .Where(i => IsImage(i))
                    .Select(i => Path.GetFileName(i))
                    .Select(i => new SelectListItem { Text = i, Value = i });
                if (ImageListValue == "" && files.Any())
                    ImageListValue = files.First().Text;
                return files;
            }
        }

        public Task OnPostChangeDirectoryAsync()
        {
            SearchTerms = "";
            ImageListValue = "";
            return OnPostSelectImageAsync();
        }

        public Task OnPostDeleteFolder()
        {
            Directory.Delete(FileImageFolder, true);
            ImageFolder = "";
            return OnPostChangeDirectoryAsync();
        }

        public Task OnPostCreateFolderAsync()
        {
            string name = UniqueDirectory(NewDirectoryName);
            Directory.CreateDirectory(FileImageFolderRoot + name);
            ImageFolder = name;
            return OnPostChangeDirectoryAsync();
        }

        public async Task OnPostSelectImageAsync()
        {
            if (!IsImageBtnsEnabled)
            {
                ImageUrl = "";
                ResizeWidth = "";
                ResizeHeight = "";
                return;
            }

            ImageUrl = ImageFolderRoot + (string.IsNullOrEmpty(ImageFolder) ? "" : ImageFolder + "/") + ImageListValue + "?" + new Random().Next(1000);
            var img = await GetImageSize(FileImageFolder + ImageListValue);
            ResizeWidth = img.Width.ToString();
            ResizeHeight = img.Height.ToString();
            ImageAspectRatio = "" + img.Width / (float)img.Height;
        }

        public Task OnPostRenameImageAsync()
        {
            string filename = UniqueFilename(NewImageName);
            System.IO.File.Move(FileImageFolder + ImageListValue, FileImageFolder + filename);
            ImageListValue = filename;
            return OnPostSelectImageAsync();
        }

        public Task OnPostDeleteImageAsync()
        {
            System.IO.File.Delete(FileImageFolder + ImageListValue);
            ImageListValue = "";
            return OnPostSelectImageAsync();
        }

        public async Task OnPostResizeImageAsync()
        {
            uint width = Convert.ToUInt32(ResizeWidth);
            uint height = Convert.ToUInt32(ResizeHeight);

            byte[] image = System.IO.File.ReadAllBytes(FileImageFolder + ImageListValue);
            image = await ResizeImageBytes(image, width, height);
            System.IO.File.WriteAllBytes(FileImageFolder + ImageListValue, image);

            ResizeMessage = "Image successfully resized!";
            await OnPostSelectImageAsync();
        }

        public async Task OnPostUploadAsync()
        {
            if (IsImage(UploadedImageFile.FileName))
            {
                string filename = UniqueFilename(UploadedImageFile.FileName);
                var stream = new MemoryStream();
                UploadedImageFile.CopyTo(stream);
                byte[] image = await ResizeImageBytes(stream.ToArray(), 1024, 1024); //make 1024x1024 the largest image size
                System.IO.File.WriteAllBytes(FileImageFolder + filename, image);

                ImageListValue = filename;
                await OnPostSelectImageAsync();
            }
        }

        //util methods
        private bool IsImage(string file)
        {
            return file.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                file.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase) ||
                file.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase);
        }

        protected string UniqueFilename(string filename)
        {
            string newfilename = filename;

            for (int i = 1; System.IO.File.Exists(FileImageFolder + newfilename); i++)
            {
                newfilename = filename.Insert(filename.LastIndexOf('.'), "(" + i + ")");
            }

            return newfilename;
        }

        protected string UniqueDirectory(string directoryname)
        {
            string newdirectoryname = directoryname;

            for (int i = 1; Directory.Exists(FileImageFolderRoot + newdirectoryname); i++)
            {
                newdirectoryname = directoryname + "(" + i + ")";
            }

            return newdirectoryname;
        }

        protected async Task<byte[]> ResizeImageBytes(byte[] imageData, uint? desiredWidth, uint? desiredHeight)
        {
            using (var job = new FluentBuildJob())
            {
                var res = await job.Decode(imageData).ConstrainWithin(desiredWidth, desiredHeight)
                    .EncodeToBytes(new LibJpegTurboEncoder()).FinishAsync();
                var bytes = res.First.TryGetBytes();
                return bytes.HasValue ? bytes.Value.Array : new byte[] {};
            }
        }

        protected async Task<(int Width, int Height)> GetImageSize(string filename)
        {
            using (var job = new FluentBuildJob())
            {
                var imageData = System.IO.File.ReadAllBytes(filename);
                var res = await job.Decode(imageData)
                    .EncodeToBytes(new LibJpegTurboEncoder()).FinishAsync();
                return (res.First.Width, res.First.Height);
            }
        }
    }
}