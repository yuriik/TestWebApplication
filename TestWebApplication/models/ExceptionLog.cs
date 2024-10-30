namespace TestWebApplication.models
{
    public class ExceptionLog
    {
        public int Id { get; set; }
        public string EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string QueryParams { get; set; }
        public string RequestBody { get; set; }
        public string StackTrace { get; set; }
    }
}
