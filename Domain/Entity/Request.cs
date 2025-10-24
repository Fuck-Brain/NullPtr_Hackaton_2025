namespace Back.Domain.Entity
{
    public class Request
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }

        public string NameRequest { get; private set; } = string.Empty;
        public string TextRequest { get; private set; } = string.Empty;
        public string Label { get; private set; } = string.Empty;
        public bool IsSended { get; private set; }

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
