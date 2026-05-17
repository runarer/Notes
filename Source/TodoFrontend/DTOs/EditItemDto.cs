namespace TodoFrontend.DTOs;

public record EditItemDto(int Id, string Title, string? Description = null, DateTimeOffset? Due = null, bool? Completed = null, int? ListId = null, int? Order = null);