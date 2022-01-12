using Telegram;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
namespace Foydali_tozalovchibot
{
    public class DB
    {
        public ChatId chatId { get; set; }
        public ChatType chatType { get; set; }
        public ChatMemberStatus status { get; set; }

    }
}
