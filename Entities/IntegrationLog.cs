namespace DataIntegrationHub.Entities
{
    public class IntegrationLog
    {
        public int Id { get; set; }
        public string SourceApi { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ResponsePayload { get; set; } = string.Empty;
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}
