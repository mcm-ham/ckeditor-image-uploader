# ckeditor-image-uploader

This repo contains a custom written image uploader form for CKeditor. This has been added to example projects for Webforms and MVC.

### Notes

* In Webforms example Ckeditor in Controls class is setup to use ScriptManager to allow control to work in AJAX environments. This is not available in ASP.NET 3.0 or earlier unless ASP.NET AJAX Extensions are installed. If not available you can replace ScriptManager with Page.ClientScript.
* Note the image and file browser is configured in config.js under the ckeditor folder. The CKEditor control automatically enables htmlEncodeOutput to get around ASP.NET Request Validation without resorting to disabling it, which would allow other textboxs to contain html like script tags for attacks. To prevent script tags being posted in ckeditor control you can use the Microsoft AntiXss library (available at http://wpl.codeplex.com/) in the LoadPostData method under Controls.cs. If you trust all the users using this control, and want to allow scripts to be posted to allow things like embedding of google maps, this can be ignored.
