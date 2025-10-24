using Back.Domain.Entity;

namespace Back.Application;

public class RequestServices
{
    public RequestServices()
    {
        
    }

    public Guid CreateRequest(Guid userId, string name, string text)
    {
        // TODO: check if user exists
        var request = new Request(userId,  name, text);
        // 
        // TODO: add req to repo
        return request.Id;
    }

    public List<Request> GetUserRequests(Guid userId)
    {
        // TODO: get user requests

        return new List<Request>();
    }
}