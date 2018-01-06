using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CefBox.Extensions
{
    public static class TaskExtensions
    {
        public static void TryStart(this Task task)
        {
            try
            {
                task.Start();
            }
            catch (InvalidOperationException) { }
        }

        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable<TResult> AnyContext<TResult>(this Task<TResult> task)
        {
            return task.ConfigureAwait(continueOnCapturedContext: false);
        }

        [DebuggerStepThrough]
        public static ConfiguredTaskAwaitable AnyContext(this Task task)
        {
            return task.ConfigureAwait(continueOnCapturedContext: false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Causes compiler to optimize the call away
        public static void NoWarning(this Task task)
        {
            /* No code goes in here */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Causes compiler to optimize the call away
        public static void NoWarning(this ConfiguredTaskAwaitable task)
        {
            /* No code goes in here */
        }

        public static async Task Log(this Task task, ILogger logger, string info = null)
        {
            var watch = Stopwatch.StartNew();
            logger.Debug($"begin {info ?? task.Id.ToString()}");

            await task;

            watch.Stop();
            logger.Debug($"finished {info ?? task.Id.ToString()} in {watch.ElapsedMilliseconds}ms");
        }

        public static async Task<T> Log<T>(this Task<T> task, ILogger logger, string info = null)
        {
            var watch = Stopwatch.StartNew();
            logger.Debug($"begin {info ?? task.Id.ToString()}");

            var result = await task;

            watch.Stop();
            logger.Debug($"finished {info ?? task.Id.ToString()} in {watch.ElapsedMilliseconds}ms");

            return result;
        }


    }
}
