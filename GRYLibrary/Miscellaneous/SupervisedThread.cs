using System;

namespace GRYLibrary
{
    public class SupervisedThread
    {
        public GRYLog LogObject { get; set; }
        public SupervisedThread(Action action, string name = "")
        {
            this.Action = action;
            this.Id = Guid.NewGuid();
            this.Name = $"{nameof(SupervisedThread)} {this.Id.ToString()} " + (string.IsNullOrEmpty(name) ? string.Empty : $"({name})");
            this.LogObject = new GRYLog();
        }
        public bool LogOverhead { get; set; } = false;
        public string Name { get; set; }
        public Guid Id { get; }
        public Action Action { get; }
        private bool _Running = false;
        private System.Threading.Thread _Thread = null;
        private void Execute()
        {
            this._Running = true;
            if (this.LogOverhead)
            {
                this.LogObject.LogInformation(string.Format("Start Action of thread with id {0} and name \"{1}\"", this.Id.ToString(), this.Name.ToString()));
            }
            try
            {
                this.Action();
            }
            catch (Exception exception)
            {
                this.LogObject.LogError(string.Format("Error occurred while executing Action of thread with id {0} and name \"{1}\"", this.Id.ToString(), this.Name.ToString()), exception);
            }
            finally
            {
                this._Running = false;
            }
            if (this.LogOverhead)
            {
                this.LogObject.LogInformation(string.Format("Startprocess of thread with id {0} and name \"{1}\" started", this.Id.ToString(), this.Name.ToString()));
            }
        }

        public void Start()
        {
            if (!this._Running)
            {
                this._Thread = new System.Threading.Thread(this.Execute)
                {
                    Name = this.Name
                };
                this._Thread.Start();
            }
        }
        public void Abort()
        {
            if (this._Running)
            {
                this._Thread.Abort();
            }
        }
    }
}
