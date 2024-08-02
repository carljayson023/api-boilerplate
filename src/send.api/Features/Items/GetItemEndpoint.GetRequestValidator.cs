using FluentValidation;

namespace send.api.Features.Items
{
    public class GetRequestValidator : AbstractValidator<GetRequest>
    {
        public GetRequestValidator()
        {
            //RuleFor(x => x.Date)
            //    .NotEmpty()
            //    .Must(date => date >= DateTime.Now.Date)
            //    .WithMessage("Date must be today or in the future.");
        }
    }
}
