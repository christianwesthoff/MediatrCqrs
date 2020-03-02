using System;
using System.Collections.Generic;
using MediatR;

namespace MediatrTest.Commands
{
    
    public class CommandSuccess<TCommand, TResult> : INotification
    {
        public CommandSuccess(TCommand command, TResult result)
        {
            Result = result;
            Command = command;
        }
        
        public TCommand Command { get; set; }
        
        public TResult Result { get; set; }
    }
    
    public class CommandError<TCommand> : INotification
    {
        public CommandError(TCommand command, IEnumerable<Exception> exceptions)
        {
            Command = command;
            Exceptions = exceptions;
        }
        
        public TCommand Command { get; set; }
        public IEnumerable<Exception> Exceptions { get; set; }
    } 
}