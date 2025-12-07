using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Infrastructure.Persistence;
using PropertyManagement.Api.Shared.Exceptions;

namespace PropertyManagement.Api.Features.Properties.Application.Commands;

public sealed record ArchivePropertyCommand(Guid Id) : IRequest;

internal sealed class ArchivePropertyCommandValidator : AbstractValidator<ArchivePropertyCommand>
{
    public ArchivePropertyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

internal sealed class ArchivePropertyCommandHandler : IRequestHandler<ArchivePropertyCommand>
{
    private readonly PropertyManagementDbContext _context;

    public ArchivePropertyCommandHandler(PropertyManagementDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ArchivePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Property with ID {request.Id} not found");

        property.Archive();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
