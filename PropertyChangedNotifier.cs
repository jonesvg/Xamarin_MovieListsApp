using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MoviesListProject.Helpers
{
    public abstract class PropertyChangedNotifier : INotifyPropertyChanged
    {

        private static Action<Action> _runOnUIThread;
        public static Action<Action> RunOnUiThread
        {
            get
            {
                return _runOnUIThread;
            }
            private set
            {
                _runOnUIThread = ExecuteSafely;
            }
        }

        private static Func<Func<Task>, string, Task> _safeExecutor = async (Func<Task> work, string description) =>
        {
            try
            {
                if (work != null)
                    await work();
            }
            catch (Exception e)
            {
                Debug.WriteLine(description ?? "Error Safely swallowing exception");
                Debug.WriteLine(e);
            }
        };
        protected void SetProperty<U>(
            ref U backingField, U value,
            [CallerMemberName]string propertyName = null,
            Action onChanged = null,
            EventHandler @event = null)
        {
            if (this.ThrowOnNullIfDebugging(propertyName))
                return;

            if (EqualityComparer<U>.Default.Equals(backingField, value))
                return;

            backingField = value;

            onChanged?.Invoke();

            @event?.Invoke(this, EventArgs.Empty);

            this.OnPropertyChanged(propertyName);
        }

        [IgnoreDataMember, EditorBrowsable(EditorBrowsableState.Never)]
        public bool HasPropertyChangeSubscription => this.PropertyChanged != null;

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (!HasPropertyChangeSubscription)
                return;

            if (this._isPropertyChangedNotificationsPaused && !string.IsNullOrWhiteSpace(propertyName))
            {
                lock (this)
                    this.PausedPropertyChangedNotifications.Enqueue(propertyName);

                //Debug.WriteLine($"{this.GetHashCode()}:{this.GetType().Name}.{propertyName} - Enqueued");
                return;
            }

            if (this.ThrowOnNullIfDebugging(propertyName))
                return;

            //Debug.WriteLine($"{this.GetHashCode()}:{this.GetType().Name}.{propertyName} - Notified");

            NotifyAllPropertiesChanged(propertyName);
        }

        private string[] _propertyNames;
        private string[] PropertyNames => _propertyNames ?? (_propertyNames = this.GetType().GetRuntimeProperties().Select(p => p.Name).ToArray());


        public void NotifyAllPropertiesChanged()
        {
            NotifyAllPropertiesChanged(PropertyNames);
        }

        public void NotifyAllPropertiesChanged(params string[] properties)
        {
            if (!HasPropertyChangeSubscription)
                return;

            RunOnUiThread(() => this.InnerNotifyAllPropertiesChanged(properties));
        }

        protected virtual void InnerNotifyAllPropertiesChanged(params string[] properties)
        {
            foreach (var propertyName in (properties ?? new string[0]).Where(p => !string.IsNullOrWhiteSpace(p)))
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool ThrowOnNullIfDebugging(string propertyName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
                return false;

#if DEBUG
            var message = string.Format("OnPropertyChanged - property name not specified at {0}, File Path: {1}, Line Number: {2}",
                    memberName, sourceFilePath, sourceLineNumber);
            throw new Exception(message);
#else
            //TODO: Log to xamarin insights
            return true;
#endif
        }

        private Queue<string> pausedPropertyChangedNotifications;
        private Queue<string> PausedPropertyChangedNotifications => pausedPropertyChangedNotifications ?? (pausedPropertyChangedNotifications = new Queue<string>());
        private bool _isPropertyChangedNotificationsPaused;

        /// <summary>
        /// Pauses raising <see cref="PropertyChanged"/> event and optionally replays
        /// them after the pause is disposed,
        /// </summary>
        /// <remarks>
        /// This doesn´t currently support nested pause
        /// </remarks>
        /// <param name="replayOnDisposed"></param>
        /// <returns>A <see cref="IDisposable"/> that re-enables Property-Notifications </returns>
        protected IDisposable SuspendChangeNotifications(bool replayOnDisposed = true)
        {
            lock (this)
            {
                this._isPropertyChangedNotificationsPaused = true;
                return replayOnDisposed
                    ? new Disposable(this.ClearPausedPropertyNotifications)
                    : new Disposable(() => this._isPropertyChangedNotificationsPaused = false);
            }
        }

        void ClearPausedPropertyNotifications()
        {
            if (!HasPropertyChangeSubscription)
                return;

            lock (this)
            {
                this._isPropertyChangedNotificationsPaused = false;

                if (!HasPropertyChangeSubscription)
                    return;


                if (!this.HasPropertyChangeSubscription)
                {
                    this.PausedPropertyChangedNotifications.Clear();
                }
                else
                {
                    var props = this.PausedPropertyChangedNotifications.Distinct().ToArray();
                    this.PausedPropertyChangedNotifications.Clear();
                    this._isPropertyChangedNotificationsPaused = false;

                    this.NotifyAllPropertiesChanged(props);
                }

            }

        }

        public static void ExecuteSafely(Action action)
        {
            ExecuteSafely(null, action);
        }

        public static void ExecuteSafely(string description, Action action)
        {
            if (action == null)
                return;

            ExecuteSafely(description, () =>
            {
                action();
                return Task.Delay(0);
            }).Wait();
        }
        public static Task ExecuteSafely(Func<Task> action)
        {
            return ExecuteSafely(null, action);
        }

        public static Task ExecuteSafely(string description, Func<Task> action)
        {
            if (action == null)
                return Task.Delay(0);
            return SafeExecutor(action, description);
        }

        public static Func<Func<Task>, string, Task> SafeExecutor
        {
            get { return _safeExecutor; }
            set
            {
                if (_safeExecutor == null || _safeExecutor == value)
                    return;

                _safeExecutor = value;
            }
        }
        public class Disposable : IDisposable
        {

            public Disposable(Action disposer, string description = null)
            {
                this._description = description;
                this._disposer = disposer;
            }

            private readonly string _description;
            private readonly Action _disposer;

            public static IDisposable Empty { get; internal set; }

            public void Dispose()
            {
                ExecuteSafely($"{_description} Disposal", this._disposer);
            }


        }

    }
}
