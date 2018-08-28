﻿namespace GRYLibrary.Event
{
    public interface IObserver<SenderType, EventArgumentType>
    {
        void Update(object sender, Argument<SenderType, EventArgumentType> argument);
    }
}
