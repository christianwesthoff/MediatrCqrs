using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatrTest.Extensions;
using MediatrTest.Types;

namespace MediatrTest.Infrastructure
{

    public interface ICommandExecutionBehavior<in TCommand, TAggregate>
    {
        Task<TAggregate> ExecuteAsync(TCommand command, TAggregate state, CancellationToken cancellationToken = default);
    }
    
    public interface ICommandContextBehavior<in TCommand, TAggregate>
    {
        Task<TAggregate> LoadAsync(TCommand command, CancellationToken cancellationToken = default);
    }
    
    public interface ICommandStoreBehavior<in TAggregate, TResponse>
    {
        Task<TResponse> StoreAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    }
    
    public interface IBaseCommand {}
    
    // ReSharper disable once UnusedTypeParameter
    public interface ICommand<TAggregate, TResponse> : IRequest<ICommandResponse<TResponse>>, IBaseCommand
    {
        
    }

    public class CommandTransactionHandler<TCommand, TAggregate, TResponse> : IRequestHandler<TCommand, ICommandResponse<TResponse>>
        where TCommand: ICommand<TAggregate, TResponse>
    {
        private readonly IEnumerable<IPreCommandExecutionValidator<TCommand, TAggregate>> _preHandleValidators;
        private readonly IEnumerable<IPostCommandExecutionValidator<TCommand, TAggregate>> _postHandleValidators;
        private readonly ICommandContextBehavior<TCommand, TAggregate> _contextBehavior;
        private readonly ICommandExecutionBehavior<TCommand, TAggregate> _executionBehavior;
        private readonly ICommandStoreBehavior<TAggregate, TResponse> _storeBehavior;
        private readonly IMediator _mediator;

        public CommandTransactionHandler(IMediator mediator,
            IEnumerable<IPreCommandExecutionValidator<TCommand, TAggregate>> preHandleValidators, 
            IEnumerable<IPostCommandExecutionValidator<TCommand, TAggregate>> postHandleValidators, 
            IOptional<ICommandContextBehavior<TCommand, TAggregate>> contextBehavior, 
            IOptional<ICommandExecutionBehavior<TCommand, TAggregate>> executionBehavior, 
            IOptional<ICommandStoreBehavior<TAggregate, TResponse>> storeBehavior)
        {
            _mediator = mediator;
            _preHandleValidators = preHandleValidators ?? throw new ArgumentNullException(nameof(preHandleValidators));
            _postHandleValidators = postHandleValidators ?? throw new ArgumentNullException(nameof(postHandleValidators));
            _contextBehavior = contextBehavior.Instance;
            _executionBehavior = executionBehavior?.Instance ?? throw new ArgumentNullException(nameof(executionBehavior));
            _storeBehavior = storeBehavior?.Instance ?? throw new ArgumentNullException(nameof(storeBehavior));
        }
        
        public async Task<ICommandResponse<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                // LoadContext
                TAggregate aggregate = default;
                if (_contextBehavior != null)
                {
                    aggregate = await _contextBehavior.LoadAsync(command, cancellationToken);
                }
                
                var state = new CommandState<TCommand, TAggregate>(command, aggregate);
                
                // Validate pre state change
                var preCommandHandleFailures = _preHandleValidators
                    .Select(v => v.Validate(state))
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)
                    .ToList();
                
                if (!preCommandHandleFailures.Any())
                {
                    throw new PreCommandHandleValidationException(preCommandHandleFailures);
                }
                
                // Execute state change
                await _executionBehavior.ExecuteAsync(command, aggregate, cancellationToken);
                
                // Validate post state change
                var postHandleFailures = _postHandleValidators
                    .Select(v => v.Validate(state))
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)
                    .ToList();
                
                if (!postHandleFailures.Any())
                {
                    throw new PostCommandHandleValidationException(postHandleFailures);
                }
                
                var response = await _storeBehavior.StoreAsync(aggregate, cancellationToken);
                
                await _mediator.Publish(new CommandSuccess<TCommand, TAggregate>(command, aggregate), cancellationToken);
                return new CommandResponse<TCommand,TResponse>(response);
            }
            catch (Exception exception)
            {
                var exceptions = (exception is AggregateException aggregate
                    ? aggregate.InnerExceptions
                    : exception.Yield()).ToArray();
                
                await _mediator.Publish(new CommandError<TCommand>(command, exceptions), cancellationToken);
                return new CommandResponse<TCommand,TResponse>(exceptions);
            }
        }
    }
}