namespace GitInsight.Core;

public record ForkDto(string name, Owner owner, string htmlUrl, DateTime created);

public record Owner(string username, string htmlUrl, string avatarUrl);
