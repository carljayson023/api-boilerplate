
using FastEndpoints;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace send.api.Features.WeatherForeCasts
{
    public class CreateRequestValidator : Validator<CreateRequest>
    {
        public CreateRequestValidator()
        {
            RuleFor(x => x.TemperatureC)
                .NotNull()
                .NotEmpty();
        }
    }
}
