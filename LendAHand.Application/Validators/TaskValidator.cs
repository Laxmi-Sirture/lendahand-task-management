using FluentValidation;
using LendAHand.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Validators
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskDTO>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot exceed 255 characters");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required")
                .Must(p => p == "Low" || p == "Medium" || p == "High")
                .WithMessage("Priority must be Low, Medium or High");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(s => s == "Pending" || s == "InProgress" || s == "Completed")
                .WithMessage("Status must be Pending, InProgress or Completed");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Due date cannot be earlier than start date");

            RuleFor(x => x.AssignedEmployeeId)
                .NotEmpty().WithMessage("Assigned employee is required");
        }
    }

    public class UpdateTaskValidator : AbstractValidator<UpdateTaskDTO>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot exceed 255 characters");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required")
                .Must(p => p == "Low" || p == "Medium" || p == "High")
                .WithMessage("Priority must be Low, Medium or High");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(s => s == "Pending" || s == "InProgress" || s == "Completed")
                .WithMessage("Status must be Pending, InProgress or Completed");

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Due date cannot be earlier than start date");
        }
    }
}
