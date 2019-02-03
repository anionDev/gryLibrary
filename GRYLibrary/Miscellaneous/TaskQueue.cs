using System;
using System.Collections.Generic;
using System.Threading;

namespace GRYLibrary.Miscellaneous
{
    public class TaskQueue
    {
        private readonly Queue<Tuple<string, Action>> _ActionQueue = new Queue<Tuple<string, Action>>();
        public TaskQueue(bool infiniteMode = false)
        {
            this.CurrentAmountOfThreads = new Semaphore(nameof(this.CurrentAmountOfThreads));
            this.IsRunning = false;
            this.InfiniteMode = infiniteMode;
            this.MaxDegreeOfParallelism = 10;
        }

        /// <remarks>The string-value is supposed to be the name of the action.</remarks>
        public void Queue(Tuple<string, Action> action)
        {
            this._ActionQueue.Enqueue(action);
        }
        public Semaphore CurrentAmountOfThreads { get; private set; }
        public bool IsRunning { get; private set; }
        public bool InfiniteMode { get; }
        public int MaxDegreeOfParallelism { get; set; }
        public GRYLog LogObject { get; set; } = null;
        public void Start()
        {
            if (!this.IsRunning)
            {
                this.IsRunning = true;
                new Thread(this.Manage).Start();
            }
        }
        private void Manage()
        {
            try
            {
                LogObject?.LogInformation($"Start executing queued actions.");
                bool enabled = true;
                while (enabled)
                {
                    if (!this.InfiniteMode)
                    {
                        enabled = false;
                    }
                    while (!this.IsFinished())
                    {
                        while (this.NewThreadCanBeStarted())
                        {
                            Tuple<string, Action> dequeuedAction = this._ActionQueue.Dequeue();
                            Thread thread = new Thread(() => this.ExecuteTask(dequeuedAction))
                            {
                                Name = $"{nameof(TaskQueue)}-Thread for action \"{dequeuedAction.Item1}\""
                            };
                            this.CurrentAmountOfThreads.Increment();
                            thread.Start();
                        }
                    }
                }
            }
            finally
            {
                this.IsRunning = false;
                LogObject?.LogInformation($"Finished executing queued actions.");
            }
        }

        private bool IsFinished()
        {
            return 0 == this._ActionQueue.Count && this.CurrentAmountOfThreads.Value == 0;
        }

        private bool NewThreadCanBeStarted()
        {
            return 0 < this._ActionQueue.Count && this.CurrentAmountOfThreads.Value < this.MaxDegreeOfParallelism;
        }

        private void ExecuteTask(Tuple<string, Action> action)
        {
            LogObject?.LogInformation($"Start action {action.Item1}. {CurrentAmountOfThreads.Value} Threads are now running.");
            try
            {
                action.Item2();
            }
            catch (Exception exception)
            {
                LogObject?.LogError($"Error in action {action.Item1}.", exception);
            }
            finally
            {
                this.CurrentAmountOfThreads.Decrement();
                LogObject?.LogInformation($"Finished action {action.Item1}. {CurrentAmountOfThreads.Value} Threads are still running.");
            }
        }
    }
}
