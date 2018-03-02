using System;
using System.Threading.Tasks;
using System.Windows.Input;
namespace MoviesListProject.Helpers
{
    public static class CommandExtension
    {
        public static void TryExecute(this ICommand command, object parameter = null)
        {
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }

        async public static Task<bool> TryExecuteAsync(this ICommand command, object parameter = null)
        {
            if (command == null)
                return false;

            var commandEx = command as ICommandEx ?? AsyncCommand.CreateFromAction<object>(command.Execute, command.CanExecute);

            var canExecute = commandEx.CanExecute(parameter);

            if (canExecute)
                await commandEx.ExecuteAsync(parameter);

            return canExecute;
        }

        public static ICommandEx IsBusyAs(this ICommandEx command, ICommandEx extension, bool isTwoWay = true)
        {
            command.Executed += (_, __) => extension.IsBusy = false;
            command.ExecutionStarted += (_, __) => extension.IsBusy = true;

            if (isTwoWay)
            {
                extension.Executed += (_, __) => command.IsBusy = false;
                extension.ExecutionStarted += (_, __) => command.IsBusy = true;
            }

            return command;
        }

        public static ICommandEx Notifying(this ICommandEx command, Func<ICommandEx> extension)
        {
            command.Executed += (_, __) => extension()?.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => extension()?.InvalidateCanExecute();

            return command;
        }

        public static ICommandEx NotOverlapping(this ICommandEx command, Func<ICommandEx> extension)
        {
            Func<bool> canExecute = () =>
            {
                var cmd = extension();

                return cmd == null || !cmd.IsRunning;
            };

            var wrapper = AsyncCommand.Create<object>(async parameter =>
            {
                extension()?.InvalidateCanExecute();

                if (canExecute())
                    await command.TryExecuteAsync(parameter);

            }, parameter => canExecute() && command.CanExecute(parameter));

            wrapper.Executed += (_, __) => extension()?.InvalidateCanExecute();
            wrapper.ExecutionStarted += (_, __) => extension()?.InvalidateCanExecute();

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }

        public static ICommandEx Append(this ICommandEx command, Action extension, bool skipOnError = true)
        {
            var wrapper = AsyncCommand.Create<object>(async parameter =>
            {
                if (skipOnError)
                {
                    await command.TryExecuteAsync(parameter);
                    extension();
                }
                else
                {
                    try
                    {
                        await command.TryExecuteAsync(parameter);
                    }
                    finally
                    {
                        extension();
                    }
                }

            }, command.CanExecute);

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }

        public static ICommandEx Prepend(this ICommandEx command, Action extension, bool skipOnError = true)
        {
            var wrapper = AsyncCommand.Create<object>(async parameter =>
            {
                if (skipOnError)
                {
                    extension();
                    await command.TryExecuteAsync(parameter);
                }
                else
                {
                    try
                    {
                        extension();
                    }
                    finally
                    {
                        await command.TryExecuteAsync(parameter);
                    }
                }
            }, command.CanExecute);

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }

        public static ICommandEx Append(this ICommandEx command, Func<Task> extension, bool skipOnError = true)
        {
            var wrapper = AsyncCommand.Create<object>(async parameter =>
            {
                if (skipOnError)
                {
                    await command.TryExecuteAsync(parameter);
                    await extension();
                }
                else
                {
                    try
                    {
                        await command.TryExecuteAsync(parameter);
                    }
                    finally
                    {
                        await extension();
                    }
                }
            }, command.CanExecute);

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }

        public static ICommandEx Prepend(this ICommandEx command, Func<Task> extension, bool skipOnError = true)
        {
            var wrapper = AsyncCommand.Create<object>(async parameter =>
            {
                if (skipOnError)
                {
                    await extension();
                    await command.TryExecuteAsync(parameter);
                }
                else
                {
                    try
                    {
                        await extension();
                    }
                    finally
                    {
                        await command.TryExecuteAsync(parameter);
                    }
                }
            }, command.CanExecute);

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }

        public static ICommandEx Append(this ICommandEx command, ICommandEx extension, bool skipOnError = true)
        {
            if (extension == null)
                return command;

            var wrapper = AsyncCommand.Create<object>(async parameter =>
            {
                if (skipOnError)
                {
                    await command.TryExecuteAsync(parameter);
                    await extension.TryExecuteAsync(parameter);
                }
                else
                {
                    try
                    {
                        await command.TryExecuteAsync(parameter);
                    }
                    finally
                    {
                        await extension.TryExecuteAsync(parameter);
                    }
                }

            }, parameter => command.CanExecute(parameter) && extension.CanExecute(parameter));

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            extension.Executed += (_, __) => wrapper.InvalidateCanExecute();
            extension.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }

        public static ICommandEx Prepend(this ICommandEx command, ICommandEx extension, bool skipOnError = true)
        {
            if (extension == null)
                return command;

            var wrapper = AsyncCommand.Create<object>(async parameter =>
            {
                if (skipOnError)
                {
                    await extension.TryExecuteAsync(parameter);
                    await command.TryExecuteAsync(parameter);
                }
                else
                {
                    try
                    {
                        await extension.TryExecuteAsync(parameter);
                    }
                    finally
                    {
                        await command.TryExecuteAsync(parameter);
                    }
                }
            }, parameter => command.CanExecute(parameter) && extension.CanExecute(parameter));

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            extension.Executed += (_, __) => wrapper.InvalidateCanExecute();
            extension.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }

        public static ICommandEx Add(this ICommandEx command, ICommandEx extension)
        {
            if (extension == null)
                return command;

            var wrapper = AsyncCommand.Create<object>(parameter =>
            {
                return Task.WhenAll(
                    command.TryExecuteAsync(parameter),
                    extension.TryExecuteAsync(parameter));

            }, parameter => command.CanExecute(parameter) && extension.CanExecute(parameter));

            command.Executed += (_, __) => wrapper.InvalidateCanExecute();
            command.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            extension.Executed += (_, __) => wrapper.InvalidateCanExecute();
            extension.ExecutionStarted += (_, __) => wrapper.InvalidateCanExecute();

            return wrapper;
        }
    }
}
