using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GRLibrary.Event
{
    [DataContract]
    public abstract class EventSender<SenderType, EventArgumentType>
    {
        public EventSender() : this(null)
        {
        }
        public EventSender(GLog logObject)
        {
            this.LogObject = logObject;
        }
        private List<IObserver<SenderType, EventArgumentType>> _Observer = null;
        private void InitializeIfRequired()
        {
            if (this._Observer == null)
            {
                this._Observer = new List<IObserver<SenderType, EventArgumentType>>();
            }
        }
        public void Register(IObserver<SenderType, EventArgumentType> observer)
        {
            InitializeIfRequired();
            if (!this._Observer.Contains(observer))
            {
                this._Observer.Add(observer);
            }
        }
        public void Deregister(IObserver<SenderType, EventArgumentType> observer)
        {
            InitializeIfRequired();
            if (this._Observer.Contains(observer))
            {
                this._Observer.Remove(observer);
            }
        }
        public GLog LogObject { get; set; }
        protected void Notify(Argument<SenderType, EventArgumentType> argument)
        {
            InitializeIfRequired();
            foreach (IObserver<SenderType, EventArgumentType> observer in this._Observer)
            {
                try
                {
                    observer.Update(this, argument);
                }
                catch(Exception exception)
                {
                    if (this.LogObject != null)
                    {
                        this.LogObject.LogError("Error occurred in observer.Update", exception,"b586472c");
                    }
                }
            }
        }
    }
}
