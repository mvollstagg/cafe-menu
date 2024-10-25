using IAndOthers.Core.Data.Enumeration;

namespace IAndOthers.Core.Data.Result
{
    public class IOResultMetadata
    {
        public IOResultStatusEnum Status { get; set; }
        public List<KeyValuePair<string, List<string>>>? Messages { get; set; }

        public IOResultMetadata()
        {
            Status = IOResultStatusEnum.Success;
            Messages = new List<KeyValuePair<string, List<string>>>();
        }

        public IOResultMetadata(IOResultStatusEnum status, List<KeyValuePair<string, List<string>>>? messages = null)
        {
            Status = status;
            Messages = messages ?? new List<KeyValuePair<string, List<string>>>();
        }

        public IOResultMetadata(IOResultStatusEnum status, string message)
        {
            Status = status;
            Messages = new List<KeyValuePair<string, List<string>>>
            {
                new KeyValuePair<string, List<string>>("Message", new List<string> { message })
            };
        }

        public void AddMessage(string key, string message)
        {
            var kvp = Messages?.Find(kvp => kvp.Key == key);
            if (kvp.HasValue)
            {
                kvp.Value.Value.Add(message);
            }
            else
            {
                Messages?.Add(new KeyValuePair<string, List<string>>(key, new List<string> { message }));
            }
        }
    }
}