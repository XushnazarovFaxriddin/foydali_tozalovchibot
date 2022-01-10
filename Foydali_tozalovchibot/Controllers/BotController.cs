using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Foydali_tozalovchibot.Controllers
{
    public class BotController : Controller
    {
        // msg types
        private static MessageType[] types = new MessageType[] {
            MessageType.Photo,
            MessageType.Audio,
            MessageType.Document,
            MessageType.Unknown,
            MessageType.Video,
            MessageType.Voice,
            MessageType.Location,
            MessageType.Contact,
            MessageType.Sticker,
            MessageType.Venue};
        // bot usernamesi @ siz
        private static string botUsername = "foydali_tozalovchibot";
        // adminlarning Id lari
        private static long[] adminId = new long[] { 1092338349, 583526419 };
        // botClient obyektiga API:Token yordamida botni ulash
        private static TelegramBotClient botClient = new("5063739606:AAFWQr7XN4mVpSOW_HHP_q1rwAbvOtR5Mdw");
        public string Index()
        {
            botClient.OnMessage += Xabar_Kelganda;
            botClient.OnCallbackQuery += InlineKeyboardButtonCallback;
            botClient.StartReceiving();
            return "Bot Ishlamoqda";
        }
        private async void Xabar_Kelganda(object sender, MessageEventArgs e)
        {
            try { 
            //var x = e.Message;
            Console.WriteLine(e.Message.Chat.LastName + "  =>  " + e.Message.Text);
            long botId = 5063739606;
            long chatId = e.Message.Chat.Id;
            int msgId = e.Message.MessageId;
            string msg = e.Message.Text;
            var chatType = e.Message.Chat.Type;// Supergroup 
            string msgType = e.Message.Type.ToString();
            ///
            var x = e.Message.Type;
            bool d = x == MessageType.ChatMembersAdded;
            var r = ChatType.Group;
            /// 
            ChatMemberStatus? role;
            long? msgUserId = e.Message?.From?.Id;

            try { role = botClient?.GetChatMemberAsync(chatId, botId)?.Result?.Status; }
            catch { role = null; }

            // yuborilgan xabardagi matnni olish
            if (types.Contains(e.Message.Type))
            {
                msg = e.Message.Caption;
                await botClient.SendTextMessageAsync(adminId[0], msg);
            }
            
            if (chatType.ToString() == "Supergroup" || chatType.ToString() == "Group")
            {
                if (role.ToString() == "Administrator")
                {
                    if (msgType == "ChatMemberLeft" || msgType == "ChatMembersAdded")
                    {
                        try
                        {
                            await botClient.DeleteMessageAsync(chatId, msgId);
                            //await botClient.SendTextMessageAsync(chatId, chatType.ToString());
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(chatId,
                                "Botni adminstrator huquqlaridan xabarlarni o'chirish huquqini yoqing!",
                                replyToMessageId: msgId);
                            //await botClient.SendTextMessageAsync(chatId, chatType.ToString());
                        }
                    }
                    else if (Yordamchi.Reklama(msg))
                    {
                        try
                        {
                            await botClient.DeleteMessageAsync(chatId, msgId);
                            await botClient.SendTextMessageAsync(chatId,
                                $"Hurmatli <a href='tg://user?id={msgUserId}'>{e.Message.From.FirstName}</a> bu guruhda reklama uzatish mumkin emas!",
                                parseMode:ParseMode.Html);
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(chatId,
                                "Bot guruhdagi reklamalarni o'chirishi uchun botga Adminstrator huqularidan xabarlarni o'chirish huquqini yoqing! "
                                + "Hurmatli Admin",
                                replyToMessageId: msgId);
                        }
                    }
                }
                else
                {
                    if (msgType == "ChatMemberLeft" || msgType == "ChatMembersAdded" || Yordamchi.Reklama(msg))
                        await botClient.SendTextMessageAsync(chatId,
                            "Bot guruhdagi kirdi chiqdilarni o'chirishi uchun botni guruhga admin qiling",
                            replyToMessageId:msgId);
                }
            }
            // yuborilgan xabar matnligiga tekshirish
            if (e.Message.Type == MessageType.Text)
            {
                // adminligigga tekshirish
                if (adminId.Contains<long>(chatId))
                {
                    await botClient.SendTextMessageAsync(chatId, "siz adminsiz");
                }
                else
                {
                    if (msg == "/start")
                    {
                        InlineKeyboardMarkup markup = new InlineKeyboardMarkup(
                            new InlineKeyboardButton[][]
                            {
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton
                                        .WithCallbackData(text: "Bot haqida", callbackData: "info")
                                },
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithUrl(text: "➕ Gruppaga Qoʻshish➕",
                                        url:$"t.me/{botUsername}?startgroup=new")
                                },
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithUrl(text:"saytga o'tamiz",
                                        url:$"https://google.com/salom")
                                }
                            }
                        );
                        await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Bu bot gruppangizdagi reklama va kirdi-chiqdilarini tozalaydi.",
                           replyMarkup: markup
                       );
                    }
                    else
                    {
                       // await botClient.SendTextMessageAsync(chatId, $"noto'g'ri so'z: {msg}", replyToMessageId: msgId);
                    }
                }
            }
                //            else
                //            {
                //                await botClient.SendTextMessageAsync(chatId,
                //                    @$"Bot faqat matnli xabarlarga javob qaytaradi:
                //{role.ToString()}
                //", replyToMessageId: msgId);
                //
            }
            catch(Exception ex)
            {
                await botClient.SendTextMessageAsync(adminId[0], ex.Message);
            }
        }
        private async void InlineKeyboardButtonCallback(object sender, CallbackQueryEventArgs e)
        {
            if (e.CallbackQuery.Data != "join Group")
                await botClient
                   .SendTextMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Data);
        }
    }
}
