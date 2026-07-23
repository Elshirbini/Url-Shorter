using UrlShorter.Common.Exceptions;
using UrlShorter.Modules.Links.Application.Interfaces;
using UrlShorter.Modules.Links.Infrastructure.Models;

namespace UrlShorter.Modules.Links.Application.UseCases
{
    public class RedirectLinkUseCase
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IClickRepository _clickRepository;

        public RedirectLinkUseCase(ILinkRepository linkRepository, IClickRepository clickRepository)
        {
            _linkRepository = linkRepository;
            _clickRepository = clickRepository;
        }

        public async Task<string> RedirectAsync(string code, HttpContext context, CancellationToken cancellationToken = default)
        {
            var link = await _linkRepository.GetFirstOrDefaultLinkAsync(l => l.Code == code, cancellationToken)
                ?? throw new NotFoundException("Link not found");

            var userAgent = context.Request.Headers.UserAgent.ToString();
            var referer = context.Request.Headers.Referer.ToString();
            var ip = context.Connection.RemoteIpAddress?.ToString();

            var device = DetectDevice(userAgent);

            var click = new Click
            {
                LinkId = link.LinkId,
                DeviceType = device,
                Referer = string.IsNullOrWhiteSpace(referer) ? "direct" : referer,
                Ip = string.IsNullOrWhiteSpace(ip) ? "unknown" : ip
            };

            await _clickRepository.AddClickAsync(click, cancellationToken);

            link.Clicks += 1;

            await _linkRepository.SaveChangesAsync(cancellationToken);

            return link.RedirectUrl;
        }

        private string DetectDevice(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "unknown";

            userAgent = userAgent.ToLower();

            if (userAgent.Contains("mobile"))
                return "mobile";

            if (userAgent.Contains("tablet"))
                return "tablet";

            return "desktop";
        }
    }


}