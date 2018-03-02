using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MoviesListProject.Helpers
{
    public class AsyncCommand : PropertyChangedNotifier, ICommandEx
    {
        static AsyncCommand()
        {
            DisabledCommand = new AsyncCommand { IsEnabled = false };
            DummyCommand = new AsyncCommand();
        }
        protected AsyncCommand()
            : this(string.Empty, _ => Task.Delay(0))
        { }

        public AsyncCommand(string name, Func<object, Task> execute, Func<object, bool> canExecute = null)
        {
            name = (name ?? "").Trim();
            this.HasName = !string.IsNullOrWhiteSpace(name);
            this.Name = (!this.HasName || name.EndsWith("Command")) ? name : name + " Command";
            this.IsEnabled = true;

            this.ExecutionAction = execute;
            this.CanExecutePredicate = canExecute;
            this.IsConditional = canExecute != null;
            this.CurrentExecutionTask = Task.Delay(0);
        }

        protected Func<object, Task> ExecutionAction { get; }
        protected Func<object, bool> CanExecutePredicate { get; }

        public bool IsRunning { get; private set; }
        /// <summary>
        /// Inverts it value each time the command is executed
        /// </summary>
        public bool Toggle { get; private set; }

        public bool IsConditional { get; }

        public bool HasCanExecuteChangedSubscription => this.CanExecuteChanged != null;
        public bool IsRunningFirstTime => this.IsRunning && !this.ExecutedAtLeastOnce;
        public bool IsRunningOrAwaitingFirstRun => this.IsRunning || !this.ExecutedAtLeastOnce;
        public bool IsIdle => !(this.IsBusy || this.IsRunning);
        private bool _isBusy;
        public bool IsBusy
        {
            get => this._isBusy || this.IsRunning;
            set => this._isBusy = value;
        }

        public string LastErrorMessage { get; protected set; }
        public bool LastRunFaulted { get; protected set; }
        public bool LastRunSuccessfull => !this.LastRunFaulted && this.ExecutedAtLeastOnce;
        public bool ExecutedAtLeastOnce { get; protected set; }
        public bool ExecutedSuccessfullyAtLeastOnce { get; protected set; }
        public bool IsEnabled { get; set; }
        public bool AllowParallelExecutions { get; set; }
        public Task CurrentExecutionTask { get; private set; }
        public string Name { get; protected set; }
        public bool HasName { get; private set; }
        public static ICommandEx DisabledCommand { get; private set; }
        public static ICommandEx DummyCommand { get; private set; }

        public override string ToString() =>
            $"{{Name: {Name}, IsBusy: {IsBusy}, IsEnabled: {IsEnabled}, ExecutedAtLeastOnce: {ExecutedAtLeastOnce}, LastRunFaulted: '{LastRunFaulted}', LastErrorMessage: '{LastErrorMessage}'}}";

        public virtual bool CanExecute(object parameter)
        {
            return this.IsEnabled && (this.AllowParallelExecutions || this.IsIdle) && (this.CanExecutePredicate?.Invoke(parameter) ?? true);
        }

        public event EventHandler CanExecuteChanged;
        public event EventHandler<object> ExecutionStarted;
        public event EventHandler<object> Executed;



        public void InvalidateCanExecute()
        {
            this.RaiseOnUI(this.CanExecuteChanged);
        }

        async public void Execute(object parameter)
        {
            await (this.CurrentExecutionTask = this.ExecuteAsync(parameter));
        }

        async public Task ExecuteAsync(object parameter)
        {
            if (!this.CanExecute(parameter))
                return;

            try
            {
                this.Raise(ExecutionStarted, parameter);

                this.IsRunning = true;
                this.LastRunFaulted = false;
                this.Toggle = !this.Toggle;

                var name = $"{this.Name} - {parameter}".TrimEnd('-', ' ');

                await ExecuteAsync(name, parameter);
            }
            finally
            {
                this.IsRunning = false;
                this.ExecutedAtLeastOnce = true;
                this.Raise(Executed, parameter);
            }
        }

        async Task ExecuteAsync(string name, object parameter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(parameter as string))
                    Track(this.Name);


                await this.OnExecute(parameter);

                this.ExecutedSuccessfullyAtLeastOnce = true;
                this.LastErrorMessage = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                this.LastRunFaulted = true;

                this.LastErrorMessage = ex.Message;

                Report(name, "Error", ex);
            }
        }

        async protected virtual Task OnExecute(object parameter)
        {
            await this.ExecutionAction(parameter);
        }

        public virtual ICommandEx Clone(string name = null)
        {
            var cmd = (AsyncCommand)this.MemberwiseClone();
            cmd.Name = name;
            return cmd;
        }

        public override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            this.InvalidateCanExecute();
        }

        protected static void Track(string name, string extra = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var id = (name + " - " + extra).Trim('-', ' ');
        }

        protected static void Report(string name, string extra, Exception ex)
        {
            Track(name, extra);
        }

        public static AsyncCommand CreateFromAction(Action execute)
        {
            return new AsyncCommand(name: null, execute: _ => Task.Run(execute));
        }

        public static AsyncCommand CreateFromAction(string name, Action execute)
        {
            return new AsyncCommand(name, execute: _ => Task.Run(execute));
        }

        public static AsyncCommand Create(Func<Task> execute)
        {
            return new AsyncCommand(name: null, execute: _ => execute?.Invoke());
        }

        public static AsyncCommand Create(string name, Func<Task> execute)
        {
            return new AsyncCommand(name, execute: _ => execute?.Invoke());
        }

        public static AsyncCommand Create(Func<Task> execute, Func<bool> canExecute)
        {
            return canExecute == null
                ? new AsyncCommand(name: null, execute: _ => execute?.Invoke())
                : new AsyncCommand(name: null, execute: _ => execute?.Invoke(), canExecute: _ => canExecute());
        }

        public static AsyncCommand Create(string name, Func<Task> execute, Func<bool> canExecute)
        {
            return canExecute == null
                ? new AsyncCommand(name: null, execute: _ => execute?.Invoke())
                : new AsyncCommand(name, execute: _ => execute?.Invoke(), canExecute: _ => canExecute());
        }


#pragma warning disable 1998
        public static AsyncCommand CreateFromAction<T>(string name, Action<T> execute, Func<T, bool> canExecute = null,
                                                       bool runAsync = true)
        {
            if (runAsync)
                return Create<T>(name, execute: arg => Task.Run(() => execute(arg)), canExecute: canExecute);

            return Create<T>(name, execute: async arg => execute(arg), canExecute: canExecute);
        }

        public static AsyncCommand CreateFromAction<T>(Action<T> execute, Func<T, bool> canExecute = null, bool runAsync = true)
        {
            return CreateFromAction<T>(name: null, execute: execute, canExecute: canExecute, runAsync: runAsync);
        }

        public static AsyncCommand CreateFromAction<T>(Action<T> execute, Func<bool> canExecute, bool runAsync = true)
        {
            return canExecute == null
                ? CreateFromAction(name: null, execute: execute, canExecute: null, runAsync: runAsync)
                : CreateFromAction(name: null, execute: execute, canExecute: _ => canExecute(), runAsync: runAsync);
        }

        public static AsyncCommand CreateFromAction(Action execute, Func<bool> canExecute, bool runAsync = true)
        {
            return CreateFromAction(name: null, execute: execute, canExecute: canExecute, runAsync: runAsync);
        }

        public static AsyncCommand CreateFromAction(string name, Action execute, Func<bool> canExecute, bool runAsync = true)
        {
            if (runAsync)
            {
                return canExecute == null
                    ? Create<object>(name: null, execute: arg => Task.Run(execute))
                    : Create<object>(name: null, execute: arg => Task.Run(execute), canExecute: _ => canExecute());
            }

            return canExecute == null
                ? Create<object>(name: null, execute: async arg => execute())
                : Create<object>(name: null, execute: async arg => execute(), canExecute: _ => canExecute());
        }

#pragma warning restore 1998
        public static AsyncCommand Create<T>(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            return Create<T>(name: null, execute: execute, canExecute: canExecute);
        }

        public static AsyncCommand Create<T>(string name, Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            Func<object, T> handleArg = arg => object.ReferenceEquals(arg, null) ? default(T) : (T)arg;

            return canExecute == null
                ? new AsyncCommand(name, arg => execute(handleArg(arg)))
                : new AsyncCommand(name, arg => execute(handleArg(arg)), arg => canExecute(handleArg(arg)));
        }

        /// <summary>
        /// Creates a named Dummy <see cref="AsyncCommand"/> 
        /// </summary>
        /// <param name="name">Name of the the <seealso cref="ICommandEx"/></param>
        /// <returns></returns>
        public static ICommandEx CreateDummy(string name) => new AsyncCommand() { Name = name?.Trim() };
    }
}
