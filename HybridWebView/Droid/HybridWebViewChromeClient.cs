using System;
using Android.App;
using Android.Content;
using Android.Webkit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Plugin.HybridWebView.Droid
{
    public class HybridWebViewChromeClient : WebChromeClient
    {
        private static int FILECHOOSER_RESULTCODE = 1;
        private readonly WeakReference<HybridWebViewRenderer> _reference;

        public HybridWebViewChromeClient(HybridWebViewRenderer renderer)
        {
            _reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        // For Android < 5.0
        [Java.Interop.Export]
        public void openFileChooser(IValueCallback filePathCallback, string acceptType, string capture)
        {
            Intent chooserIntent = new Intent(Intent.ActionGetContent);
            chooserIntent.AddCategory(Intent.CategoryOpenable);
            chooserIntent.SetType("*/*");
            RegisterCustomFileUploadActivity(filePathCallback, chooserIntent);
        }

        // For Android > 5.0
        [Android.Runtime.Register("onShowFileChooser", "(Landroid/webkit/WebView;Landroid/webkit/ValueCallback;Landroid/webkit/WebChromeClient$FileChooserParams;)Z", "GetOnShowFileChooser_Landroid_webkit_WebView_Landroid_webkit_ValueCallback_Landroid_webkit_WebChromeClient_FileChooserParams_Handler")]
        public override bool OnShowFileChooser(global::Android.Webkit.WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            base.OnShowFileChooser(webView, filePathCallback, fileChooserParams);

            var chooserIntent = fileChooserParams.CreateIntent();
            RegisterCustomFileUploadActivity(filePathCallback, chooserIntent, fileChooserParams.Title);

            return true;
        }

        private void RegisterCustomFileUploadActivity(IValueCallback filePathCallback, Intent chooserIntent, string title = "File Chooser")
        {
            HybridWebViewRenderer renderer;
            if (!_reference.TryGetTarget(out renderer))
                return;
            var appActivity = renderer.Context as FormsAppCompatActivity;
            if (appActivity == null)
                return;

            Action<Result, Intent> callback = (resultCode, intentData) =>
            {
                if (filePathCallback == null)
                    return;

                var result = FileChooserParams.ParseResult((int)resultCode, intentData);
                filePathCallback.OnReceiveValue(result);

                appActivity.HybridWebView_UnregisterIntentCallback(FILECHOOSER_RESULTCODE);
            };

            appActivity.HybridWebView_RegisterIntentCallback(FILECHOOSER_RESULTCODE, callback);

            ((FormsAppCompatActivity)renderer.Context).StartActivityForResult(Intent.CreateChooser(chooserIntent, title), FILECHOOSER_RESULTCODE);
        }
    }
}
