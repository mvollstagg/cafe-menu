namespace IAndOthers.Application.Cache.Models
{
    public class ChatRoomMessagesModel
    {
        public long ChatRoomId { get; set; }
        public List<ChatRoomMessageModel> Messages { get; set; } = new List<ChatRoomMessageModel>();
    }
}
