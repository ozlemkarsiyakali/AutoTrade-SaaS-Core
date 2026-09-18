using AutoTrade.Core.DTOs;
using FluentValidation;

namespace AutoTrade.Core.Validators;

public class CreateVehicleBrandDtoValidator : AbstractValidator<CreateVehicleBrandDto>
{
    public CreateVehicleBrandDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} alanı boş geçilemez.")
            .NotNull().WithMessage("{PropertyName} alanı zorunludur.")
            .Length(2, 50).WithMessage("{PropertyName} alanı {MinLength}-{MaxLength} karakter arasında olmalıdır.");
    }
}