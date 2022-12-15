using Android.App;
using Android.Content;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Platform.Android;

namespace Plugin.HybridWebView.Droid
{
    public static class Extender
    {
        private static ConcurrentDictionary<int, Action<Result, Intent>> _activityResultCallbacks = new ConcurrentDictionary<int, Action<Result, Intent>>();

        /// <summary>
        /// Do not call this, see HybridWebView_ReportActivityResult on how to use.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="requestCode"></param>
        /// <param name="callback"></param>
        public static void HybridWebView_RegisterIntentCallback(this FormsAppCompatActivity activity, int requestCode, Action<Result, Intent> callback)
        {
            _activityResultCallbacks.TryAdd(requestCode, callback);
        }

        /// <summary>
        /// Do not call this, see HybridWebView_ReportActivityResult on how to use.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="requestCode"></param>
        public static void HybridWebView_UnregisterIntentCallback(this FormsAppCompatActivity activity, int requestCode)
        {
            Action<Result, Intent> callback;
            _activityResultCallbacks.TryRemove(requestCode, out callback);
            callback = null;
        }

        /// <summary>Call this Method in your OnActivityResult handler passing all arguments.</summary>
        /// <param name="activity"></param>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        public static void HybridWebView_ReportActivityResult(this FormsAppCompatActivity activity, int requestCode, Result resultCode, Intent data)
        {
            Action<Result, Intent> callback;
            if (_activityResultCallbacks.TryGetValue(requestCode, out callback))
                callback(resultCode, data);
        }
    }
}
