namespace Back.Domain.Entity
{
    public class ResultRequest
    {
        public Guid Id { get; set; }
        public Request Request { get; set; }
        public User UserRequest { get; set; }
        public IEnumerable<User> ResultRequestUsers { get; set; }
        public ResultRequest(Guid id, Request request, User userRequest, IEnumerable<User> resultRequestUsers)
        {
            Id = id;
            Request = request;
            UserRequest = userRequest;
            ResultRequestUsers = resultRequestUsers;
        }
    }
}
