using System;

namespace GRLibrary
{
    public class SupervisedThread
    {
        public GLog LogObject { get; set; }
        public SupervisedThread(Action action)
        {
            this.Action = action;
            this.Id = Guid.NewGuid();
            this.Name = this.Id.ToString();
            this.LogObject = new GLog();
            this._Thread = new System.Threading.Thread(this.Execute)
            {
                Name = this.Name
            };
        }
        public string Name { get; set; }
        public Guid Id { get; }
        public Action Action { get; }
        private bool _Running = false;
        private readonly System.Threading.Thread _Thread = null;
        private void Execute()
        {
            this._Running = true;
            try
            {
                this.LogObject.LogInformation(string.Format("Start execution of Action of thread with id {0} and name \"{1}\"", this.Id.ToString(), this.Name.ToString()));
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
            this.LogObject.LogInformation(string.Format("Execution of Action of thread with id {0} and name \"{1}\" finished", this.Id.ToString(), this.Name.ToString()));
        }

        public void Start()
        {
            if (!this._Running)
            {
                try
                {
                    this.LogObject.LogInformation(string.Format("Start startprocess of thread with id {0} and name \"{1}\"", this.Id.ToString(), this.Name.ToString()));
                    this._Thread.Start();
                }
                catch (Exception exception)
                {
                    this.LogObject.LogError(string.Format("Error occurred while startprocess of thread with id {0} and name \"{1}\"", this.Id.ToString(), this.Name.ToString()), exception);
                }
                this.LogObject.LogInformation(string.Format("Startprocess of thread with id {0} and name \"{1}\" started", this.Id.ToString(), this.Name.ToString()));
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
