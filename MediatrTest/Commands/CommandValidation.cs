using System.Collections.Generic;
using System.Runtime.Serialization;
using FluentValidation;
using FluentValidation.Results;

namespace MediatrTest.Commands
{
    public interface IPreCommandExecutionValidator<TCommand, TAggregate>: IValidator<CommandState<TCommand, TAggregate>> { }
    
    public interface IPostCommandExecutionValidator<TCommand, TAggregate>: IValidator<CommandState<TCommand, TAggregate>> { }
    
    public class PreCommandHandleValidationException : ValidationException
    {
        public PreCommandHandleValidationException(string message) : base(message)
        {
        }

        public PreCommandHandleValidationException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors)
        {
        }

        public PreCommandHandleValidationException(IEnumerable<ValidationFailure> errors) : base(errors)
        {
        }

        public PreCommandHandleValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    
    public class PostCommandHandleValidationException : ValidationException
    {
        public PostCommandHandleValidationException(string message) : base(message)
        {
        }

        public PostCommandHandleValidationException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors)
        {
        }

        public PostCommandHandleValidationException(IEnumerable<ValidationFailure> errors) : base(errors)
        {
        }

        public PostCommandHandleValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}