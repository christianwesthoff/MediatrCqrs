using System;
using System.Collections.Generic;

namespace MediatrTest.Commands
{
    public interface ICommandResponse<TResponse>
    {
        TResponse Response { get; set; }
        bool IsSuccess { get; set; }
        public IEnumerable<Exception> Exceptions { get; set; }
    }

    // ReSharper disable once UnusedTypeParameter
    public class CommandResponse<TCommand, TResponse> : ICommandResponse<TResponse>
    {
        public CommandResponse(TResponse response)
        {
            Response = response;
            IsSuccess = true;
        }
        public CommandResponse(IEnumerable<Exception> exceptions)
        {
            Exceptions = exceptions;
            IsSuccess = false;
        }
        
        public TResponse Response { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<Exception> Exceptions { get; set; }
    }
}