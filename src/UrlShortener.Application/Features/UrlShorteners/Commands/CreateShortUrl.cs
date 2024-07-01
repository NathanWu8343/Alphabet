using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel.Messaging;
using SharedKernel.Messaging.Base;
using SharedKernel.Results;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Extensions;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Features.UrlShorteners.Commands
{
    /// <summary>
    /// Represents the create short url command.
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="RedirectPath"></param>
    public sealed record CreateShortUrlCommand(string Url, string RedirectPath) : ICommand<Result<string>>
    {
    }

    /// <summary>
    /// Represents the <see cref="CreateShortUrlCommand"/> validator.
    /// </summary>
    internal sealed class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
    {
        public CreateShortUrlCommandValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithError(ValidationErrors.CreateShortUrl.UrlRequired)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithError(ValidationErrors.CreateShortUrl.UrlInvalid);
        }
    }

    /// <summary>
    /// Represents the <see cref="CreateShortUrlCommand"/> handler.
    /// </summary>
    internal sealed class CreateShortUrlCommandHandler :
        BaseCommandHandler<CreateShortUrlCommand, Result<string>>
    {
        private readonly IShortenedUrlRepository _shortendUrlRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShortCodeGenerator _shortCodeGenerator;
        private readonly TimeProvider _timeProvider;

        public CreateShortUrlCommandHandler(ILogger<CreateShortUrlCommandHandler> logger, IShortenedUrlRepository shortendUrlRepository, IUnitOfWork unitOfWork, IShortCodeGenerator shortCodeGenerator, TimeProvider timeProvider)
            : base(logger)
        {
            _shortendUrlRepository = shortendUrlRepository;
            _unitOfWork = unitOfWork;
            _shortCodeGenerator = shortCodeGenerator;
            _timeProvider = timeProvider;
        }

        public override async Task<Result<string>> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
        {
            var code = _shortCodeGenerator.Generate(request.Url);
            var shortendUrl = ShortenedUrl.Create(
                request.Url,
                $"{request.RedirectPath}/{code}",
                code,
                _timeProvider.GetUtcNow().DateTime);
            _shortendUrlRepository.Add(shortendUrl);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return shortendUrl.ShortUrl;
        }
    }
}