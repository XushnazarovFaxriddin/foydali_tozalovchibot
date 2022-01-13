using Telegram;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;

namespace Foydali_tozalovchibot
{
    public class DB
    {
        [JsonProperty("chatId")]
        public long chatId { get; set; }

        [JsonProperty("chatType")]
        public string chatType { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

    }
}
