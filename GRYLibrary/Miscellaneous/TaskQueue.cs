using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GRYLibrary.Miscellaneous
{
    public class TaskQueue
    {
        //TODO add required locks

        public TaskQueue(bool infiniteMode = false)
        {
            this.InfiniteMode = infiniteMode;
            this.MaxDegreeOfParallelism = 10;
        }
        private readonly Queue<Action> _ActionQueue = new Queue<Action>();
        public void Queue(Action action)
        {
            this._ActionQueue.Enqueue(action);
        }
        public int CurrentAmountOfThreads { get; private set; }
        public bool IsRunning { get; private set; }
        public bool InfiniteMode { get; }
        public int MaxDegreeOfParallelism { get; set; }
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
                bool enabled = true;
                while (enabled)
                {
                    if (!this.InfiniteMode)
                    {
                        enabled = false;
                    }
                    while (!this.IsFinished())
                    {
                        while (this.StartnewThread())
                        {
                            new Thread(() => this.ExecuteTask(this._ActionQueue.Dequeue())).Start();
                        }
                    }
                }
            }
            finally
            {
                this.IsRunning = false;
            }

        }

        private bool IsFinished()
        {
            return 0 == this._ActionQueue.Count && this.CurrentAmountOfThreads == 0;
        }

        private bool StartnewThread()
        {
            return 0 < this._ActionQueue.Count && this.CurrentAmountOfThreads < this.MaxDegreeOfParallelism;
        }

        private void ExecuteTask(Action action)
        {
            this.CurrentAmountOfThreads = this.CurrentAmountOfThreads + 1;
            try
            {
                Task.Run(action).Wait();
            }
            catch
            {
                Utilities.NoOperation();
            }
            finally
            {
                this.CurrentAmountOfThreads = this.CurrentAmountOfThreads - 1;
            }
        }
    }
}
