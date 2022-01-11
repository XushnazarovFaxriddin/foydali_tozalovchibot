using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Foydali_tozalovchibot.Controllers
{
    public class BotController : Controller
    {
        bool rekYubor = false;
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
            botClient.OnUpdate += Xabar_Yangilanganda;
            botClient.OnCallbackQuery += InlineKeyboardButtonCallback;
            botClient.StartReceiving();
            return "Bot Ishlamoqda!";
        }
        private async void Xabar_Kelganda(object sender, MessageEventArgs e)
        {
            try
            {
                //Console.WriteLine(JsonConvert.SerializeObject(e.Message));
               
                long botId = 5063739606;
                long chatId = e.Message.Chat.Id;
                int msgId = e.Message.MessageId;
                string msg = e.Message.Text;
                var chatType = e.Message.Chat.Type;// Supergroup 
                MessageType msgType = e.Message.Type;
                ///
                var x = e.Message.Type;
                bool d = x == MessageType.ChatMembersAdded;
                var r = ChatType.Group;
                /// 
                ChatMemberStatus? role;
                ChatMemberStatus? userRole;
                long msgUserId = e.Message.From.Id;
                try { userRole=botClient.GetChatMemberAsync(chatId,msgUserId)?.Result?.Status; }
                catch { userRole=null; }

                try { role = botClient?.GetChatMemberAsync(chatId, botId)?.Result?.Status; }
                catch { role = null; }

                // admin reklama yuborishi
                if (rekYubor && adminId.Contains<long>(chatId))
                {
                    await botClient.ForwardMessageAsync(adminId[1], chatId, msgId);
                    rekYubor = false;
                    await botClient.SendTextMessageAsync(chatId, "reklama yuborildi");
                }

                // yuborilgan xabardagi matnni olish
                if (types.Contains(msgType))
                {
                    msg = e.Message.Caption;
                    await botClient.SendTextMessageAsync(adminId[0], msg);
                    await botClient.SendTextMessageAsync(adminId[0], chatType.ToString());
                    await botClient.SendTextMessageAsync(adminId[0], msgType.ToString());
                }

                if (chatType == ChatType.Supergroup || chatType == ChatType.Group)
                {
                    if (role == ChatMemberStatus.Administrator )
                    {
                        if (msgType == MessageType.ChatMemberLeft || msgType == MessageType.ChatMembersAdded)
                        {
                            try
                            {
                                await botClient.DeleteMessageAsync(chatId, msgId);
                                //await botClient.SendTextMessageAsync(chatId, chatType.ToString());
                            }
                            catch
                            {
                                await botClient.SendTextMessageAsync(chatId,
                                    "Bot guruhdagi kirdi-chiqdilarni o'chirishi uchun botga Adminstrator huqularidan xabarlarni o'chirish huquqini yoqing! ",
                                    replyToMessageId: msgId);
                                //await botClient.SendTextMessageAsync(chatId, chatType.ToString());
                            }
                        }
                        else if (Yordamchi.Reklama(msg) && !(userRole == ChatMemberStatus.Administrator || userRole == ChatMemberStatus.Creator))
                        {
                            try
                            {
                                await botClient.DeleteMessageAsync(chatId, msgId);
                                await botClient.SendTextMessageAsync(chatId,
                                    $"Hurmatli <a href='tg://user?id={msgUserId}'>{e.Message.From.FirstName}</a> bu guruhda reklama uzatish mumkin emas!",
                                    parseMode: ParseMode.Html);
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
                        if (msgType == MessageType.ChatMemberLeft || msgType == MessageType.ChatMembersAdded || Yordamchi.Reklama(msg))
                            await botClient.SendTextMessageAsync(chatId,
                                "Bot guruhdagi kirdi chiqdilarni o'chirishi uchun botni guruhga admin qiling",
                                replyToMessageId: msgId);
                    }
                }
                // yuborilgan xabar matnligiga tekshirish
                if (msgType == MessageType.Text && chatType == ChatType.Private)
                {
                    // adminligigga tekshirish
                    if (adminId.Contains<long>(chatId))
                    {
                        await botClient.SendTextMessageAsync(chatId, "siz admizsiz");
                        if (msg == "/reklama")
                        {
                            await botClient.SendTextMessageAsync(chatId, "Reklama yuboring!");
                            rekYubor = true;
                        }
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
                            await botClient.SendTextMessageAsync(chatId, $"noto'g'ri so'z: {msg}", replyToMessageId: msgId);
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
            catch (Exception ex)
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

        private async void Xabar_Yangilanganda(object sender, UpdateEventArgs e)
        {
            await System.IO.File.WriteAllTextAsync("json.json", JsonConvert.SerializeObject(e.Update));
        }

        public IActionResult Info()
        {
            return View();
        }


        [Route("/NotFound")]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
