using System;


namespace MoviesListProject.Helpers
{
    public static class DelegateExtensions
    {
        public static void RaiseOnUI<T>(this object sender, EventHandler<T> handler, T arg)
        {
            if (handler != null)
                PropertyChangedNotifier.RunOnUiThread(() => handler(sender, arg));
        }

        public static void RaiseOnUI(this object sender, EventHandler handler)
        {
            if (handler != null)
                PropertyChangedNotifier.RunOnUiThread(() => handler(sender, EventArgs.Empty));
        }


        public static void Raise<T>(this object sender, EventHandler<T> handler, T arg)
        {
            if (handler != null)
                PropertyChangedNotifier.ExecuteSafely(() => handler(sender, arg));
        }

        public static void Raise(this object sender, EventHandler handler)
        {
            if (handler != null)
                PropertyChangedNotifier.ExecuteSafely(() => handler(sender, EventArgs.Empty));
        }
    }
}
