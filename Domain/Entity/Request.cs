namespace Back.Domain.Entity
{
    public class Request
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public string NameRequest { get; set; }
        public string TextRequest { get; set; }
        public string Label {  get; set; } = string.Empty;
        public bool IsSended { get; set; } = false;

        public Request(Guid IdUser, string  nameRequest, string textRequest)
        {
            Id = new Guid();
            this.IdUser = IdUser;
            NameRequest = nameRequest;
            TextRequest = textRequest;
        }

        void SetLabel(string label)
        {
            this.Label = label;
            IsSended = true;
        }
    }
}
