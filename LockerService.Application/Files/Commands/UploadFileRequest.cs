using FluentValidation;
using LockerService.Application.Files.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LockerService.Application.Files.Commands;

public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest> {
    public UploadFileRequestValidator()
    {
        RuleFor(model => model.File)
            .NotNull();
    }
}
public class UploadFileRequest : IRequest<UploadFileResponse>
{
    public IFormFile File { get; set; } = default!;
}