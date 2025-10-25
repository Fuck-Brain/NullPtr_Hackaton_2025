namespace Back.Domain.Entity
{
    public class ResultRequest
    {
        public Guid Id { get; private set; }

        // ---- Связь "один к одному" с Request ----
        public Guid RequestId { get;  set; }
        public Request Request { get;  set; }

        // ---- Связь с пользователем, создавшим запрос ----
        public Guid UserRequestId { get;  set; }
        public User UserRequest { get;  set; }

        // ---- Много пользователей в результате ----
        public ICollection<User> ResultRequestUsers { get;  set; } = new List<User>();

        private ResultRequest() { } // Для EF

        public ResultRequest(Request request, User userRequest, IEnumerable<User> resultRequestUsers)
        {
            Id = Guid.NewGuid();
            Request = request ?? throw new ArgumentNullException(nameof(request));
            UserRequest = userRequest ?? throw new ArgumentNullException(nameof(userRequest));
            RequestId = request.Id;
            UserRequestId = userRequest.Id;
            ResultRequestUsers = resultRequestUsers?.ToList() ?? new List<User>();
        }

        public void AddResultUser(User user)
        {
            if (!ResultRequestUsers.Contains(user))
                ResultRequestUsers.Add(user);
        }
    }
}
