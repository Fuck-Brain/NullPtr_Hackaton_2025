namespace Back.Domain.Entity
{
    public class Request
    {
        public Guid Id { get;  set; }
        public Guid UserId { get;  set; }
        public User User { get;  set; }

        public string NameRequest { get;  set; } = string.Empty;
        public string TextRequest { get;  set; } = string.Empty;
        public string Label { get;  set; } = string.Empty;
        public bool IsSended { get;  set; }

        private Request() { } // для EF

        public Request(Guid userId, string nameRequest, string textRequest)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            NameRequest = nameRequest;
            TextRequest = textRequest;
            IsSended = false;
        }

        public void SetLabel(string label)
        {
            Label = label;
            IsSended = true;
        }
    }
}
