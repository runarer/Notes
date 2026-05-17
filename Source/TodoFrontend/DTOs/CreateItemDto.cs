namespace TodoFrontend.DTOs;

public record CreateItemDto(string Title, string? Description = null, DateTimeOffset? Due = null, int? ListId = null, int? Order = null);