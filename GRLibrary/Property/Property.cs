using GRLibrary.Event;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GRLibrary.Property
{
    public interface IProperty
    {
        string PropertyName { get; set; }
        Type PropertyValueType { get; }

    }
    [DataContract]
    public class Property<T> : EventSender<Property<T>, PropertyChangedEvengArgument<T>>, IProperty
    {
        [DataMember]
        private string _PropertyName;
        [DataMember]
        public bool AllowNullAsValue = true;
        [DataMember]
        public bool AddValuesToHistory = true;
        [DataMember]
        public bool NotifyAboutChanges = true;
        [DataMember]
        private T _InitialValue;
        [DataMember]
        private T _Value;
        [DataMember]
        private Stack<KeyValuePair<DateTime, T>> _History;
        public T InitialValue
        {
            get
            {
                return this._InitialValue;
            }
        }
        /// <summary>
        /// The History contains all <see cref="T"/>-values which where set as value for <see cref="Property{T}.Value"/> with the <see cref="DateTime"/> when they were set.
        /// </summary>
        public Stack<KeyValuePair<DateTime, T>> History
        {
            get
            {
                return new Stack<KeyValuePair<DateTime, T>>(new Stack<KeyValuePair<DateTime, T>>(this._History));
            }
        }

        public string PropertyName { get { return this._PropertyName; } set { this._PropertyName = value; } }

        public Type PropertyValueType
        {
            get
            {
                return typeof(T);
            }
        }
        public T Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                if ((value == null) && !this.AllowNullAsValue)
                {
                    throw new InvalidArgumentException("New value can not be null!");
                }
                T oldValue = this.Value;
                T newValue = value;
                this._Value = newValue;
                DateTime changeDate = DateTime.Now;
                if (this.AddValuesToHistory)
                {
                    this._History.Push(new KeyValuePair<DateTime, T>(changeDate, this._Value));
                }
                Argument<Property<T>, PropertyChangedEvengArgument<T>> argument = new Argument<Property<T>, PropertyChangedEvengArgument<T>>(this, new PropertyChangedEvengArgument<T>(oldValue, newValue, changeDate));
                try
                {
                    if (this.NotifyAboutChanges)
                    {
                        Notify(argument);
                    }
                }
                catch
                {
                }
            }
        }
        public Property(T initialValue, string propertyName,bool addValuesToHistory=false)
        {
            this._History = new Stack<KeyValuePair<DateTime, T>>();
            this._InitialValue = initialValue;
            this.PropertyName = propertyName;
            ResetToInitialValue();
            AddValuesToHistory=addValuesToHistory;
            if (!AddValuesToHistory)
            {
                ResetHistory();
            }
        }

        public void ResetToInitialValue()
        {
            this.Value = this._InitialValue;
        }
        public void ResetHistory()
        {
            this._History.Clear();
        }

        public bool EqualsValue(Property<T> obj)
        {
            return EqualsValue(obj.Value);
        }
        public bool EqualsValue(T obj)
        {
            return this.Value.Equals(obj);
        }
        public T GetValueByTimestamp(DateTime dateTime)
        {
            var history = History;
            while (history.Count > 0)
            {
                KeyValuePair<DateTime, T> current = history.Pop();
                if (current.Key < dateTime)
                {
                    return current.Value;
                }
            }
            throw new Exception("Value was not set at moment " + dateTime.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }
}
