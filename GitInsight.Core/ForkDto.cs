namespace GitInsight.Core;

public record ForkDto(string name, Owner owner, string html_url, DateTime created_at);

public record Owner(string login, string html_url, string avatar_url);
