using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoviesListProject.Helpers
{

    public interface ICommandEx : ICommand, INotifyPropertyChanged
    {
        string Name { get; }
        bool IsEnabled { get; set; }
        bool IsBusy { get; set; }

        bool AllowParallelExecutions { get; set; }

        bool IsIdle { get; }

        Task CurrentExecutionTask { get; }
        bool IsRunning { get; }

        bool IsConditional { get; }
        bool HasCanExecuteChangedSubscription { get; }
        bool LastRunFaulted { get; }
        bool ExecutedAtLeastOnce { get; }
        bool ExecutedSuccessfullyAtLeastOnce { get; }
        void InvalidateCanExecute();

        event EventHandler<object> Executed;

        event EventHandler<object> ExecutionStarted;
        Task ExecuteAsync(object parameter);

        bool IsRunningFirstTime { get; }
        bool IsRunningOrAwaitingFirstRun { get; }
        string LastErrorMessage { get; }
        bool LastRunSuccessfull { get; }

        ICommandEx Clone(string name = null);
    }

}