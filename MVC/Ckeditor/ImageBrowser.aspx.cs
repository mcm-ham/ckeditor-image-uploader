using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;

public partial class ImageBrowser : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindDirectoryList();
            ChangeDirectory(null, null);

            NewDirectoryButton.OnClientClick = "var name = prompt('Enter name of folder:'); if (name == null || name == '') return false; document.getElementById('" + NewDirectoryName.ClientID + "').value = name;";
        }

        ResizeMessage.Text = "";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "FocusImageList", "window.setTimeout(\"document.getElementById('" + ImageList.ClientID + "').focus();\", 1);", true);
    }

    protected void BindDirectoryList()
    {
        ImageFolder = "";
        string[] directories = Directory.GetDirectories(FileImageFolder);
        directories = Array.ConvertAll<string, string>(directories, delegate(string directory) { return directory.Substring(directory.LastIndexOf('\\') + 1); });
        DirectoryList.DataSource = directories;
        DirectoryList.DataBind();
        DirectoryList.Items.Insert(0, "Root");
    }

    protected void ChangeDirectory(object sender, EventArgs e)
    {
        DeleteDirectoryButton.Enabled = (DirectoryList.SelectedIndex != 0);
        ImageFolder = (DirectoryList.SelectedIndex == 0) ? "" : DirectoryList.SelectedValue + "/";
        SearchTerms.Text = "";
        BindImageList();
        SelectImage(null, null);
    }

    protected void DeleteFolder(object sender, EventArgs e)
    {
        Directory.Delete(FileImageFolder, true);
        BindDirectoryList();
        ChangeDirectory(null, null);
    }

    protected void CreateFolder(object sender, EventArgs e)
    {
        string name = UniqueDirectory(NewDirectoryName.Value);
        Directory.CreateDirectory(FileImageFolderRoot + name);
        BindDirectoryList();
        DirectoryList.SelectedValue = name;
        ChangeDirectory(null, null);
    }

    protected void BindImageList()
    {
        ImageList.Items.Clear();
        string[] files = Directory.GetFiles(FileImageFolder, "*" + SearchTerms.Text.Replace(" ", "*") + "*");
        files = Array.FindAll(files, delegate(string f) { return IsImage(f); });

        foreach (string file in files)
        {
            if (IsImage(file))
                ImageList.Items.Add(file.Substring(file.LastIndexOf('\\') + 1));
        }

        if (files.Length > 0)
            ImageList.SelectedIndex = 0;
    }

    protected void Search(object sender, EventArgs e)
    {
        BindImageList();
    }

    protected void SelectImage(object sender, EventArgs e)
    {
        RenameImageButton.Enabled = (ImageList.Items.Count != 0);
        DeleteImageButton.Enabled = (ImageList.Items.Count != 0);
        ResizeImageButton.Enabled = (ImageList.Items.Count != 0);
        ResizeWidth.Enabled = (ImageList.Items.Count != 0);
        ResizeHeight.Enabled = (ImageList.Items.Count != 0);

        if (ImageList.Items.Count == 0)
        {
            Image1.ImageUrl = "";
            ResizeWidth.Text = "";
            ResizeHeight.Text = "";
            return;
        }

        Image1.ImageUrl = ImageFolder + ImageList.SelectedValue + "?" + new Random().Next(1000);

        using (Image img = Image.FromFile(FileImageFolder + ImageList.SelectedValue))
        {
            ResizeWidth.Text = img.Width.ToString();
            ResizeHeight.Text = img.Height.ToString();
            ImageAspectRatio.Value = "" + img.Width / (float)img.Height;
        }

        int pos = ImageList.SelectedItem.Text.LastIndexOf('.');
        if (pos == -1)
            return;
        RenameImageButton.OnClientClick = "var name = prompt('Enter new filename:','" + ImageList.SelectedItem.Text.Substring(0, pos) + "'); if (name == null || name == '') return false; document.getElementById('" + NewImageName.ClientID + "').value = name + '" + ImageList.SelectedItem.Text.Substring(pos) + "';";
        OkButton.OnClientClick = "window.top.opener.CKEDITOR.dialog.getCurrent().setValueOf('info', 'txtUrl', encodeURI('" + ImageFolder + ImageList.SelectedValue.Replace("'", "\\'") + "')); window.top.close(); window.top.opener.focus();";
    }

    protected void RenameImage(object sender, EventArgs e)
    {
        string filename = UniqueFilename(NewImageName.Value);
        File.Move(FileImageFolder + ImageList.SelectedValue, FileImageFolder + filename);
        BindImageList();
        ImageList.SelectedValue = filename;
        SelectImage(null, null);
    }

    protected void DeleteImage(object sender, EventArgs e)
    {
        File.Delete(FileImageFolder + ImageList.SelectedValue);
        BindImageList();
        SelectImage(null, null);
    }

    protected void ResizeWidthChanged(object sender, EventArgs e)
    {
        float ratio = Parse<float>(ImageAspectRatio.Value);
        int width = Parse<int>(ResizeWidth.Text);

        ResizeHeight.Text = "" + (int)(width / ratio);
    }

    protected void ResizeHeightChanged(object sender, EventArgs e)
    {
        float ratio = Parse<float>(ImageAspectRatio.Value);
        int height = Parse<int>(ResizeHeight.Text);

        ResizeWidth.Text = "" + (int)(height * ratio);
    }

    protected void ResizeImage(object sender, EventArgs e)
    {
        int width = Parse<int>(ResizeWidth.Text);
        int height = Parse<int>(ResizeHeight.Text);

        byte[] image = File.ReadAllBytes(FileImageFolder + ImageList.SelectedValue);
        image = ResizeImage(image, width, height);
        File.WriteAllBytes(FileImageFolder + ImageList.SelectedValue, image);

        ResizeMessage.Text = "Image successfully resized!";
        SelectImage(null, null);
    }

    protected void Upload(object sender, EventArgs e)
    {
        if (IsImage(UploadedImageFile.FileName))
        {
            string filename = UniqueFilename(UploadedImageFile.FileName);
            byte[] image = ResizeImage(UploadedImageFile.FileBytes, 1024, 1024); //make 1024x1024 the largest image size
            File.WriteAllBytes(FileImageFolder + filename, image);

            BindImageList();
            ImageList.SelectedValue = filename;
            SelectImage(null, null);
        }
    }

    protected void Clear(object sender, EventArgs e)
    {
        Session.Remove("viewstate");
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

        for (int i = 1; File.Exists(FileImageFolder + newfilename); i++)
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

    protected byte[] ResizeImage(byte[] imageData, int desiredWidth, int desiredHeight)
    {
        using (MemoryStream sourceBuffer = new MemoryStream(imageData))
        using (Image image = Image.FromStream(sourceBuffer))
        {
            double scale = Math.Min(desiredWidth / (double)image.Width, desiredHeight / (double)image.Height);
            if (scale >= 1) //don't enlarge photos
                return imageData;

            int width = (int)Math.Round(image.Width * scale);
            int height = (int)Math.Round(image.Height * scale);
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics graphic = Graphics.FromImage(bitmap))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.DrawImage(image, 0, 0, width, height);
            }

            //keep metadata
            foreach (PropertyItem propertyItem in image.PropertyItems ?? new PropertyItem[0])
            {
                try { bitmap.SetPropertyItem(propertyItem); }
                catch { }
            }

            using (MemoryStream destinationStream = new MemoryStream())
            {
                bitmap.Save(destinationStream, image.RawFormat);
                return destinationStream.ToArray();
            }
        }
    }

    public T Parse<T>(object value)
    {
        try { return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value == null ? null : value.ToString()); }
        catch { return default(T); }
    }

    //properties
    protected string ImageFolderRoot
    {
        get { return ResolveUrl(ConfigurationManager.AppSettings["ImageRoot"]); }
    }

    protected string FileImageFolderRoot
    {
        get { return Server.MapPath(ImageFolderRoot); }
    }

    protected string ImageFolder
    {
        get { return ImageFolderRoot + ViewState["folder"]; }
        set { ViewState["folder"] = value; }
    }

    protected string FileImageFolder
    {
        get { return Server.MapPath(ImageFolder); }
    }
}
