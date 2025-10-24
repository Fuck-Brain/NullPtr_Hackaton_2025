using Back.Domain.Entity;

namespace Back.Application.Dtos;

public record ClassificationResponse(IEnumerable<User> ClassifiedUsers);