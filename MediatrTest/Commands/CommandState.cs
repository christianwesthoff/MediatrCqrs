﻿ namespace MediatrTest.Commands
{
    public class CommandState<TCommand, TAggregate>
    {
        public CommandState(TCommand command, TAggregate aggregate)
        {
            Command = command;
            Aggregate = aggregate;
        }
        
        public TCommand Command { get; }
        
        public TAggregate Aggregate { get; }
    }
}