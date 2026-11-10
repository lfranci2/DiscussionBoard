using Microsoft.AspNetCore.Authorization;

namespace DiscussionBoard.Authorization
{
    public sealed record IsOwnerOrAdminRequirement : IAuthorizationRequirement;
}
