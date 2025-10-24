using Back.Domain.Entity;

namespace Back.Application.Dtos;

public record ClassificationRequest(Request Req, List<User> Users);