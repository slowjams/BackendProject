using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using System.Linq;

namespace WebApplication.Core.Common.Behaviours
{
   public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
   {
      private readonly IEnumerable<IValidator<TRequest>> _validators;

      public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
      {
         _validators = validators;
      }

      /// <inheritdoc />
      public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
      {
         // TODO: throw a validation exception if there are any validation errors
         // NOTE: the validation exception should contain all failures
         if (_validators.Any())
         {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(result => result.Errors).Where(failure => failure != null).ToList();
            if (failures.Count != 0)
               throw new ValidationException(failures);
         }

         return await next();
      }
   }
}
