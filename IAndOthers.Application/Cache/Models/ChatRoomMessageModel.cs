namespace IAndOthers.Application.Cache.Models
{
    public class ChatRoomMessageModel
    {
        public long ApplicationUserId { get; set; }
        public string Message { get; set; }
        public DateTime SentDateUtc { get; set; }
    }
}
