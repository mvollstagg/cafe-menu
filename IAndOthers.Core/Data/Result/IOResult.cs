using IAndOthers.Core.Data.Enumeration;

namespace IAndOthers.Core.Data.Result
{
    public class IOResult<T>
    {
        public IOResultMetadata Meta { get; set; }
        public T? Data { get; set; }

        // Default constructor
        public IOResult()
        {
            Meta = new IOResultMetadata();
        }

        // Constructor for status only
        public IOResult(IOResultStatusEnum status)
        {
            Meta = new IOResultMetadata(status);
        }

        // Constructor for status and data
        public IOResult(IOResultStatusEnum status, T data)
        {
            Meta = new IOResultMetadata(status);
            Data = data;
        }

        // Constructor for status, data, and messages
        public IOResult(IOResultStatusEnum status, T data, List<KeyValuePair<string, List<string>>>? messages)
        {
            Meta = new IOResultMetadata(status, messages);
            Data = data;
        }

        // Constructor for status, data, and a single message
        public IOResult(IOResultStatusEnum status, T data, string message)
        {
            Meta = new IOResultMetadata(status, message);
            Data = data;
        }

        // Constructor for status and messages
        public IOResult(IOResultStatusEnum status, List<KeyValuePair<string, List<string>>>? messages)
        {
            Meta = new IOResultMetadata(status, messages);
        }

        // Constructor for status and a single message
        public IOResult(IOResultStatusEnum status, string message)
        {
            Meta = new IOResultMetadata(status, message);
        }
    }
}
