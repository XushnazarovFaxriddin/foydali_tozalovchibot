﻿using Microsoft.AspNetCore.Mvc;
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
        //Bot ulangan domen
        string url = "hali bu yeri chala" + @"/Bot/Info";
        // DB list
        private static List<DB> dBs = default;
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
            //await botClient.SendTextMessageAsync(adminId[0], JsonConvert.SerializeObject(e.Message));
            try
            {
                //Console.WriteLine(JsonConvert.SerializeObject(e.Message));

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
                    dBs.ForEach(async (s) =>
                    {
                        try
                        {
                            await botClient.ForwardMessageAsync(s.chatId, chatId, msgId);
                            count++;
                        }
                        catch { }
                    });
                    rekYubor = false;
                    await botClient.SendTextMessageAsync(chatId, $"reklama {count} ta guruh va foydalanuvchilarga yuborildi yuborildi");
                    /* await botClient.ForwardMessageAsync(adminId[1], chatId, msgId);
                     rekYubor = false;
                     await botClient.SendTextMessageAsync(chatId, "reklama yuborildi");*/
                }

                // yuborilgan xabardagi matnni olish
                if (types.Contains(msgType))
                {
                    try
                    {
                        msg = e.Message.Caption;
                    }
                    catch
                    {
                        msg = "ERROR!";
                    }
                }

                if (chatType == ChatType.Supergroup || chatType == ChatType.Group)
                {
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
                            await botClient.SendTextMessageAsync(chatId, $"Botning vazifasini batafsil bilish uchun /start bosing yoki <a href='{url}'>Bot Haqida</a> ga bosing.", replyToMessageId: msgId, parseMode: ParseMode.Html);
                        }
                    }
                }
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
            try
            {
                //await System.IO.File.WriteAllTextAsync("json.json", JsonConvert.SerializeObject(e.Update));
                if (JsonConvert.SerializeObject(e.Update.MyChatMember) != "null")
                {
                    dynamic status = e.Update?.MyChatMember?.NewChatMember?.Status;
                    if (status == ChatMemberStatus.Administrator)
                    {
                        await botClient.SendTextMessageAsync(e.Update.MyChatMember.Chat.Id, "Bot guruhda administrator bo'ldi!");
                        await fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "up");
                    }
                    else if (status == ChatMemberStatus.Member)
                    {
                        await fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "add");
                    }
                    else if (status == ChatMemberStatus.Left)
                    {
                        await fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "del");
                    }
                    else if (status == ChatMemberStatus.Kicked)
                    {
                        await fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "del");
                    }
                    else if (status == ChatMemberStatus.Restricted)
                    {
                        await fun(e.Update.MyChatMember.Chat.Id, e.Update.MyChatMember.Chat.Type, status, "add");
                    }
                    //await botClient.SendTextMessageAsync(adminId[0], JsonConvert.SerializeObject(status));
                }
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(adminId[0], "Xabar_Yangilanganda delegatetidan error:\n" + ex.Message);
            }
        }

        // funk ga "del", "add", "up" yuborish mumkin
        private async void fun(ChatId id, ChatType type, ChatMemberStatus status, string funk)
        {
            try
            {
                if (funk == "add")
                {
                    dBs.Add(new DB()
                    {
                        chatId = id,
                        chatType = type,
                        status = status,
                    });
                    await System.IO.File.WriteAllTextAsync("db.json", JsonConvert.SerializeObject(dBs));
                    dBs = JsonConvert.DeserializeObject<List<DB>>(await System.IO.File.ReadAllTextAsync("db.json"));
                    return;
                }
                dBs.ForEach(async dbx =>
                {
                    if (dbx.chatId == id)
                    {
                        if (funk == "del")
                        {
                            dBs.Remove(dbx);
                            await System.IO.File.WriteAllTextAsync("db.json", JsonConvert.SerializeObject(dBs));
                            dBs = JsonConvert.DeserializeObject<List<DB>>(await System.IO.File.ReadAllTextAsync("db.json"));
                            return;
                        }
                        else if (funk == "up")
                        {
                            dBs.Remove(dbx);
                            dbx.chatId = id;
                            dbx.chatType = type;
                            dbx.status = status;
                            dBs.Add(dbx);
                            await System.IO.File.WriteAllTextAsync("db.json", JsonConvert.SerializeObject(dBs));
                            dBs = JsonConvert.DeserializeObject<List<DB>>(await System.IO.File.ReadAllTextAsync("db.json"));
                            return;
                        }
                    }
                });
                dBs.Add(new DB()
                {
                    chatId = id,
                    chatType = type,
                    status = status,
                });
                await System.IO.File.WriteAllTextAsync("db.json", JsonConvert.SerializeObject(dBs));
                dBs = JsonConvert.DeserializeObject<List<DB>>(await System.IO.File.ReadAllTextAsync("db.json"));
                return;
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(adminId[0], "fun funksiyasidan error:\n" + ex.Message);
            }
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
