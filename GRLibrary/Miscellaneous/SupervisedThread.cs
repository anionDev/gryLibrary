using System;

namespace GRLibrary
{
    public class SupervisedThread
    {
        public GLog LogObject { get; set; }
        public SupervisedThread(Action action)
        {
            Action = action;
            Id = Guid.NewGuid();
            Name = Id.ToString();
            LogObject = new GLog();
        }
        public string Name { get; set; }
        public Guid Id { get; }
        public Action Action { get; }
        private void Execute()
        {
            _Running = true;
            try
            {
                LogObject.LogInformation(string.Format("Start execution of Action of thread with id {0} and name \"{1}\"", Id.ToString(), Name.ToString()));
                Action();
            }
            catch (Exception exception)
            {
                LogObject.LogError(string.Format("Error occurred while executing Action of thread with id {0} and name \"{1}\"", Id.ToString(), Name.ToString()), exception);
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
            }
            finally
            {
                _Running = false;
            }
            LogObject.LogInformation(string.Format("Execution of Action of thread with id {0} and name \"{1}\" finished", Id.ToString(), Name.ToString()));
        }
        private bool _Running = false;
        private System.Threading.Thread _Thread = null;

        public void Start()
        {
            if (!_Running)
            {
                try
                {
                    LogObject.LogInformation(string.Format("Start startprocess of thread with id {0} and name \"{1}\"", Id.ToString(), Name.ToString()));
                    _Thread = new System.Threading.Thread(Execute)
                    {
                        Name = this.Name
                    };
                    _Thread.Start();
                }
                catch (Exception exception)
                {
                    LogObject.LogError(string.Format("Error occurred while startprocess of thread with id {0} and name \"{1}\"", Id.ToString(), Name.ToString()), exception);
#if DEBUG
                    System.Diagnostics.Debugger.Break();
#endif
                }
                LogObject.LogInformation(string.Format("Startprocess of thread with id {0} and name \"{1}\" started", Id.ToString(), Name.ToString()));
            }
        }
        public void Abort()
        {
            if (_Running)
            {
                _Thread.Abort();
            }
        }
    }
}
