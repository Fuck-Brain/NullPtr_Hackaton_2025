namespace Back.API.DTO.Request;

public class RequestCreateDTO
{
    public Guid userId { get; set; }
    public string name { get; set; }
    public string text { get; set; }
}