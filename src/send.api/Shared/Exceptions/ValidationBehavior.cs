using FluentValidation;
using FastEndpoints;

namespace send.api.Shared.Exceptions
{
    public class ValidationBehavior<TRequest> : IPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
        {
            var req = context.Request;

            if (_validators.Any())
            {
                var validationContext = new FluentValidation.ValidationContext<TRequest>(req);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, ct)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Any())
                {
                    throw new ValidationException(failures); //this must be build in "ValidationException class
                }
            }
        }
    }
}
