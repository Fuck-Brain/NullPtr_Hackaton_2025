namespace Back.Domain.Entity
{
    public class ResultRequest
    {
        public Guid Id { get; set; }
        public Request Request { get; set; } = null;
        public User UserRequest { get; set; } = null;
        public IEnumerable<User> ResultRequestUsers { get; set; } = Enumerable.Empty<User>();

        public ResultRequest () { }
        public ResultRequest(Request request, User userRequest, IEnumerable<User> resultRequestUsers)
        {
            Id = Guid.NewGuid();
            Request = request;
            UserRequest = userRequest;
            ResultRequestUsers = resultRequestUsers;
        }
    }
}
