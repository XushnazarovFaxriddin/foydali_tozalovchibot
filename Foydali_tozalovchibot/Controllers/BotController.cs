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
using Foydali_tozalovchibot.ViewModels;
using System.Net;

namespace Foydali_tozalovchibot.Controllers
{
    public class BotController : Controller
    {
        //Bot ulangan domen
        string url = "https://google.com" + @"/Bot/Info";
        // rek yuborish un kk
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
        private static string botAPI = "5063739606:AAFWQr7XN4mVpSOW_HHP_q1rwAbvOtR5Mdw";
        private static TelegramBotClient botClient = new(botAPI);
        public string Index()
        {
            botClient.OnMessage += Xabar_Kelganda;
            botClient.OnUpdate += Xabar_Yangilanganda;
            //botClient.OnCallbackQuery += InlineKeyboardButtonCallback;
            botClient.StartReceiving();
            return "Bot Ishlamoqda!";
        }
        private async void Xabar_Kelganda(object sender, MessageEventArgs e)
        {
            //await botClient.SendTextMessageAsync(adminId[0], JsonConvert.SerializeObject(e.Message));
            try
            {
                //Console.WriteLine(JsonConvert.SerializeObject(e.Message));
                MessageEntityType? msgEnType = null;
                long botId = 5063739606;
                long chatId = e.Message.Chat.Id;
                int msgId = e.Message.MessageId;
                string msg = e.Message.Text;
                var chatType = e.Message.Chat.Type;// Supergroup Group vahokazo
                MessageType msgType = e.Message.Type;
                ///
                var x = e.Message.Type;
                bool d = x == MessageType.ChatMembersAdded;
                var r = ChatType.Group;
                /// 
                ChatMemberStatus? role;
                ChatMemberStatus? userRole;
                long msgUserId = e.Message.From.Id;
                try { userRole = botClient.GetChatMemberAsync(chatId, msgUserId)?.Result?.Status; }
                catch { userRole = null; }

                try { role = botClient?.GetChatMemberAsync(chatId, botId)?.Result?.Status; }
                catch { role = null; }

                // admin reklama yuborishi
                if (rekYubor && adminId.Contains<long>(chatId))
                {
                    int count = 0;
                    Program.dBs.ForEach((s) =>
                    {
                        try
                        {
                            botClient.ForwardMessageAsync(s.chatId, chatId, msgId);
                            count++;
                        }
                        catch { }
                    });
                    rekYubor = false;
                    await botClient.SendTextMessageAsync(chatId, $"reklama {count} ta guruh va foydalanuvchilarga yuborildi yuborildi.");
                    /* await botClient.ForwardMessageAsync(adminId[1], chatId, msgId);
                     rekYubor = false;
                     await botClient.SendTextMessageAsync(chatId, "reklama yuborildi");*/
                }

                if (chatType == ChatType.Supergroup || chatType == ChatType.Group)
                {

                    // yuborilgan xabardagi matnni olish
                    if (msgType != MessageType.Text)
                    {
                        if(JsonConvert.SerializeObject(e.Message?.CaptionEntities[0]?.Type) != "null")
                        try
                        {
                            msgEnType = e.Message?.CaptionEntities[0]?.Type;
                            msg = e.Message.Caption;
                        }
                        catch
                        {
                            msg = "ERROR!";
                            msgEnType = null;
                        }
                    }
                    else
                    {
                        if(JsonConvert.SerializeObject(e.Message.Entities[0].Type) != "null")
                        try
                        {
                            msgEnType = e.Message?.Entities[0]?.Type;
                        }
                        catch { msgEnType = null; }
                    }

                    if (role == ChatMemberStatus.Administrator)
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
                        else if ((msgEnType == MessageEntityType.Url
                            || msgEnType == MessageEntityType.TextLink
                            || msgEnType == MessageEntityType.TextMention
                            || msgEnType == MessageEntityType.Mention) &&
                            !(userRole == ChatMemberStatus.Administrator
                            || userRole == ChatMemberStatus.Creator))
                        {
                            //await botClient.SendTextMessageAsync(chatId, JsonConvert.SerializeObject(e.Message));
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
                        if (msg != "/reklama")
                            await botClient.SendTextMessageAsync(chatId, "Assalomu aleykum siz ushbu botning adminisiz!\n" +
                                "Reklama yuborish uchun /reklama ga bosing.\n" +
                                $"Bot statistikasini ko'rish uchun <a href='{url}'>bot haqida</a> ga bosing.\n" +
                                $"Botni guruhga qoshish uchun <a href='t.me/{botUsername}?startgroup=new'>bu yerga</a> bosing.\n" +
                                $"\n\nDasturchi <a href='tg://user?id={msgUserId}'>Faxriddin Xushnazarov</a>", parseMode: ParseMode.Html);
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
                                {/*
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton
                                            .WithCallbackData(text: "Bot haqida", callbackData: "info")
                                    },*/
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithUrl(text: "➕ Gruppaga Qoʻshish➕",
                                            url:$"t.me/{botUsername}?startgroup=new")
                                    },
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithUrl(text:"Bot haqida",
                                            url:$"{url}")
                                    }
                                }
                            );
                            await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "Bu bot gruppangizdagi reklama va kirdi-chiqdilarini tozalaydi.",
                               replyMarkup: markup
                           );
                            fun(chatId, chatType, ChatMemberStatus.Member, "add");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, $"Botning vazifasini batafsil bilish uchun /start bosing yoki <a href='{url}'>Bot Haqida</a> ga bosing.", replyToMessageId: msgId, parseMode: ParseMode.Html);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != "Forbidden: bot was kicked from the supergroup chat")
                    await botClient.SendTextMessageAsync(adminId[0], "Xabar_Kelganda delegatetidan error:\n" + ex.Message);
            }
        }/*
        private async void InlineKeyboardButtonCallback(object sender, CallbackQueryEventArgs e)
        {
            if (e.CallbackQuery.Data != "join Group")
                await botClient
                   .SendTextMessageAsync(e.CallbackQuery.From.Id, e.CallbackQuery.Data);
        }*/

        private async void Xabar_Yangilanganda(object sender, UpdateEventArgs e)
        {
            try
            {
                ChatMemberStatus? status;
                //await System.IO.File.WriteAllTextAsync("json.json", JsonConvert.SerializeObject(e.Update));
                if (JsonConvert.SerializeObject(e.Update.MyChatMember) != "null")
                {
                    status = e.Update.MyChatMember.NewChatMember.Status;
                    if (status == ChatMemberStatus.Administrator)
                    {
                        //await botClient.SendTextMessageAsync(e.Update.MyChatMember.Chat.Id, "Bot guruhda administrator bo'ldi!");
                        //up
                        fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "up");
                    }
                    else if (status == ChatMemberStatus.Member)
                    {
                        fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "add");
                    }
                    else if (status == ChatMemberStatus.Left)
                    {
                        //del
                        fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "del");
                    }
                    else if (status == ChatMemberStatus.Kicked)
                    {
                        //del
                        fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "del");
                    }
                    else if (status == ChatMemberStatus.Restricted)
                    {
                        //add
                        fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "add");
                    }
                    else
                    {
                        //await botClient.SendTextMessageAsync(adminId[0], JsonConvert.SerializeObject(status));
                    }
                }
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(adminId[0], "Xabar_Yangilanganda delegatetidan error:\n" + ex.Message);
            }
        }

        // funk ga "del", "add", "up" yuborish mumkin
        private async void fun(ChatId id, ChatType type, ChatMemberStatus? status, string funk)
        {
            try
            {
                int i = 0;
                bool x = false;
                if (funk == "add")
                {
                    int n = Program.dBs.Count;
                    bool bormi = false;
                    for (int j = 0; j < n; j++)
                    {
                        if (Program.dBs[j].chatId == Convert.ToInt64(id))
                        {
                            Program.dBs[j].chatType = type.ToString().ToLower();
                            Program.dBs[j].status = status.ToString().ToLower();
                            bormi = true;
                            break;
                        }
                    }
                    if (!bormi)
                        Program.dBs.Add(new DB()
                        {
                            chatId = Convert.ToInt64(id),
                            chatType = type.ToString().ToLower(),
                            status = status.ToString().ToLower()
                        });
                    await System.IO.File.WriteAllTextAsync("db.json", JsonConvert.SerializeObject(Program.dBs));
                    return;
                }
                if (funk == "del")
                {
                    int n = Program.dBs.Count;
                    for (int j = 0; j < n; j++)
                    {
                        if (Program.dBs[j].chatId == Convert.ToInt64(id))
                        {
                            Program.dBs.RemoveAt(j);
                            await System.IO.File.WriteAllTextAsync("db.json", JsonConvert.SerializeObject(Program.dBs));
                            return;
                        }
                    }
                    return;
                }
                if (funk == "up")
                {
                    int n = Program.dBs.Count;
                    bool bormi = false;
                    for (int j = 0; j < n; j++)
                    {
                        if (Program.dBs[j].chatId == Convert.ToInt64(id))
                        {
                            Program.dBs[j].chatType = type.ToString().ToLower();
                            Program.dBs[j].status = status.ToString().ToLower();
                            bormi = true;
                            break;
                        }
                    }
                    if (!bormi)
                        Program.dBs.Add(new DB()
                        {
                            chatId = Convert.ToInt64(id),
                            chatType = type.ToString().ToLower(),
                            status = status.ToString().ToLower()
                        });
                    await System.IO.File.WriteAllTextAsync("db.json", JsonConvert.SerializeObject(Program.dBs));
                    return;
                }
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(adminId[0], "fun funksiyasidan error:\n" + ex.Message);
            }
        }
        public IActionResult Info()
        {
            long all = 0;
            string urlAllMembers = "https://api.telegram.org/bot" + botAPI + "/getChatMembersCount?chat_id=";
            WebClient wc = new WebClient();
            GroupMemberCount groupMemberCount = new();
            for (int i = 0; i < Program.dBs.Count; i++)
            {
                try
                {
                    string json = wc.DownloadString(urlAllMembers + Program.dBs[i].chatId);
                    groupMemberCount = JsonConvert.DeserializeObject<GroupMemberCount>(json);
                    if (groupMemberCount != null && groupMemberCount.Ok)
                    {
                        all += groupMemberCount.Result;
                    }
                }
                catch
                {
                    continue;
                }
            }
            BotInfoViewModel viewModel = new()
            {
                dBs = Program.dBs,
                allMember = all
            };
            return View(viewModel);
        }

        public string dbJson()
        {
            return JsonConvert.SerializeObject(Program.dBs);
        }

        [Route("/NotFound")]
        public string NotFound()
        {
            return "sahifa topilmadi. 404";
        }
    }
}
