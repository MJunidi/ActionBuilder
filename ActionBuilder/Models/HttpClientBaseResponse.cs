namespace ActionBuilder.Models
{
    public class HttpClientBaseResponse<T> where T : new()
    {
        public bool IsSuccess { get; set; }

        public List<string> Errors { get; set; }

        public String Data { get; set; }
    }

    public class HttpClientBaseRequest
    {
        public string Url { get; set; }

        public HttpMethod Method { get; set; } = HttpMethod.Get;

        public RequestDataSource Source { get; set; } = RequestDataSource.Empty;

        public object Payload { get; set; } = null;

        public List<KeyValuePair<string, string>> Headers { get; set; } = new List<KeyValuePair<string, string>>();
    }
    public enum RequestDataSource
    {
        Empty = 0,
        FromBody,
        FromQueryString,
        FromUrl,
        FromForm
    }
}
