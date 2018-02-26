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
    public class LinkBrowserModel : PageModel
    {
        public LinkBrowserModel(IConfiguration config, IHostingEnvironment env)
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

        public string FileAspectRatio { get; set; }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }

        [BindProperty]
        public string FileListValue { get; set; } = "";

        [BindProperty]
        public string FileUrl { get; set; }

        [BindProperty]
        public string NewFileName { get; set; }

        public bool IsDeleteDirectoryBtnEnabled => FileFolder != "";
        public bool IsFileBtnsEnabled => FileList.Any();

        /// <summary>
        /// The URL to the currently selected file folder.
        /// </summary>
        [BindProperty]
        public string FileFolder { get; set; } = "";

        /// <summary>
        /// The URL to the root file folder.
        /// </summary>
        private string FileFolderRoot => _config["FilesRoot"] != null ? $"/{_config["FilesRoot"].Trim('/')}/" : "";

        /// <summary>
        /// The file path to the root file folder.
        /// </summary>
        private string FilePathFolderRoot => Path.Combine(_env.WebRootPath, FileFolderRoot.Trim('/', '\\')) + "\\";

        /// <summary>
        /// The file path to the currently selected file folder.
        /// </summary>
        private string FilePathFolder => Path.Combine(FilePathFolderRoot, FileFolder ?? "") + "\\";

        // Methods

        public Task OnGetAsync()
        {
            ResizeMessage = "";
            return OnPostChangeDirectoryAsync();
        }

        public IEnumerable<SelectListItem> DirectoryList =>
            new[] { new SelectListItem { Text = "Root", Value = "" } }.Concat(
                Directory.GetDirectories(FilePathFolderRoot)
                    .Select(d => Path.GetFileName(d))
                    .Select(d => new SelectListItem { Text = d, Value = d })
            );

        public IEnumerable<SelectListItem> FileList {
            get
            {
                var files = Directory.GetFiles(FilePathFolder, "*" + SearchTerms?.Replace(" ", "*") + "*")
                    .Where(i => IsFile(i))
                    .Select(i => Path.GetFileName(i))
                    .Select(i => new SelectListItem { Text = i, Value = i });
                if (FileListValue == "" && files.Any())
                    FileListValue = files.First().Text;
                return files;
            }
        }

        public Task OnPostChangeDirectoryAsync()
        {
            SearchTerms = "";
            FileListValue = "";
            return OnPostSelectFileAsync();
        }

        public Task OnPostDeleteFolder()
        {
            Directory.Delete(FilePathFolder, true);
            FileFolder = "";
            return OnPostChangeDirectoryAsync();
        }

        public Task OnPostCreateFolderAsync()
        {
            string name = UniqueDirectory(NewDirectoryName);
            Directory.CreateDirectory(FilePathFolderRoot + name);
            FileFolder = name;
            return OnPostChangeDirectoryAsync();
        }

        public Task OnPostSelectFileAsync()
        {
            if (!IsFileBtnsEnabled)
            {
                FileUrl = "";
                ResizeWidth = "";
                ResizeHeight = "";
                return Task.CompletedTask;
            }

            FileUrl = FileFolderRoot + (string.IsNullOrEmpty(FileFolder) ? "" : FileFolder + "/") + FileListValue + "?" + new Random().Next(1000);
            return Task.CompletedTask;
        }

        public Task OnPostRenameFileAsync()
        {
            string filename = UniqueFilename(NewFileName);
            System.IO.File.Move(FilePathFolder + FileListValue, FilePathFolder + filename);
            FileListValue = filename;
            return OnPostSelectFileAsync();
        }

        public Task OnPostDeleteFileAsync()
        {
            System.IO.File.Delete(FilePathFolder + FileListValue);
            FileListValue = "";
            return OnPostSelectFileAsync();
        }

        public async Task OnPostUploadAsync()
        {
            if (IsFile(UploadedFile.FileName))
            {
                string filename = UniqueFilename(UploadedFile.FileName);
                var stream = new MemoryStream();
                UploadedFile.CopyTo(stream);
                System.IO.File.WriteAllBytes(FilePathFolder + filename, stream.ToArray());

                FileListValue = filename;
                await OnPostSelectFileAsync();
            }
        }

        //util methods
        private bool IsFile(string file)
        {
            return file.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                file.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase) ||
                file.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase);
        }

        protected string UniqueFilename(string filename)
        {
            string newfilename = filename;

            for (int i = 1; System.IO.File.Exists(FilePathFolder + newfilename); i++)
            {
                newfilename = filename.Insert(filename.LastIndexOf('.'), "(" + i + ")");
            }

            return newfilename;
        }

        protected string UniqueDirectory(string directoryname)
        {
            string newdirectoryname = directoryname;

            for (int i = 1; Directory.Exists(FilePathFolderRoot + newdirectoryname); i++)
            {
                newdirectoryname = directoryname + "(" + i + ")";
            }

            return newdirectoryname;
        }
    }
}