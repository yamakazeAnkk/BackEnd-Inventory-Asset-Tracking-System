namespace SharedKernel.Messages;

public record UserCreatedMessage
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record UserUpdatedMessage
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}

public record UserDeletedMessage
{
    public Guid UserId { get; init; }
    public DateTime DeletedAt { get; init; }
}

public record UserLoginMessage
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public DateTime LoginAt { get; init; }
    public string IpAddress { get; init; } = string.Empty;
}

public record UserLogoutMessage
{
    public Guid UserId { get; init; }
    public DateTime LogoutAt { get; init; }
}
