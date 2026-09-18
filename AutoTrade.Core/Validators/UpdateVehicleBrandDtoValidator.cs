using AutoTrade.Core.DTOs;
using FluentValidation;

namespace AutoTrade.Core.Validators;

public class UpdateVehicleBrandDtoValidator : AbstractValidator<UpdateVehicleBrandDto>
{
    public UpdateVehicleBrandDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id field is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters.");
    }
}