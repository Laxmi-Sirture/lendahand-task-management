using FluentValidation;
using LendAHand.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Validators
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDTO>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required");

            RuleFor(x => x.Designation)
                .NotEmpty().WithMessage("Designation is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Must have uppercase letter")
                .Matches(@"[a-z]").WithMessage("Must have lowercase letter")
                .Matches(@"[0-9]").WithMessage("Must have a number");
        }
    }

    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDTO>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required");

            RuleFor(x => x.Designation)
                .NotEmpty().WithMessage("Designation is required");
        }
    }
}
