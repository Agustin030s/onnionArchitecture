﻿using FluentValidation;
using MediatR;

namespace Application.Behaviours
{
    // Esta clase recibe una peticion y una respuesta e implementa la interfaz de pipeline
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
           if(_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationsResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationsResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count() != 0)
                    throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
