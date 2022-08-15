using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using RozetkaHelperBot.DB;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozetkaHelperBot.Dialog
{
    class Dialog
    {
        static string token = new Config.Config().GetToken();

        
        static ITelegramBotClient bot = new TelegramBotClient(token);
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            /* <<<<<<<<<<<< ------------------ KEYBOARDS ------------------ >>>>>>>>>>>>>*/
            ReplyKeyboardMarkup startShopping = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Завершити", "Здійснити пошук продукту" },
            })
            {
                ResizeKeyboard = true
            };

            ReplyKeyboardMarkup firstShopping = new ReplyKeyboardMarkup(new[]{
                new KeyboardButton[]{"Пошук", "Завершити"},
            })
            {
                ResizeKeyboard = true
            };
            /* <<<<<<<<<<<< ------------------ KEYBOARDS ------------------ >>>>>>>>>>>>>*/

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                switch (message.Text.ToLower())
                {
                    case "/start":                                             //case START
                        await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!", replyMarkup: startShopping);
                        try
                        {
                            using (var context = new ClientContext())
                            {
                                context.Add(new Client
                                {
                                    Id = message.From.Id.ToString(),
                                    MessageId = 0
                                });
                                context.SaveChanges();
                            }
                        }
                        catch
                        {
                            using (var context = new ClientContext())
                            {
                                var list = context.Clients;
                                foreach(var item in list)
                                {
                                    if(item.Id == message.From.Id.ToString())
                                    {
                                        item.MessageId = 0;
                                        context.Update(item);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            Console.WriteLine("its already exist");
                        }
                        break;
                    case "/close":                                              //case STOP
                    case "завершити":                                           
                        using (var context = new ClientContext())
                        {
                            try
                            {
                                var list = context.Clients;
                                Client toDelete = new Client();
                                foreach (var item in list)
                                {
                                    if (item.Id == message.From.Id.ToString())
                                        toDelete = item;
                                }

                                context.Remove(toDelete);
                                context.SaveChanges();
                            }
                            catch
                            {
                                Console.WriteLine("Already deleted . . . ");
                            }
                            

                            Console.WriteLine("Deleted client . . .");
                        }
                        break;
                    case "здійснити пошук продукту":                            //case Start Shopping
                        using (var context = new ClientContext())
                        {
                            var list = context.Clients;
                            foreach (var item in list)
                            {
                                if (item.Id == message.From.Id.ToString() && item.MessageId == 0)
                                {
                                    item.MessageId += 1;
                                    context.Update(item);
                                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть назву товару для вашого пошуку. \n Формат введення: \n Назва: . . . ");
                                }
                            }
                            context.SaveChanges();
                        }
                        break;
                    case string sub when sub.ToLower().Contains("назва:"):
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Nice job", replyMarkup: firstShopping);
                        Parsing.Parsing parsing = new RozetkaHelperBot.Parsing.Parsing();
                        parsing.Url = "https://rozetka.com.ua/ua/search/?text=Asus+Zenbook+14&producer=asus&page=1";
                        var temp = parsing.ParsingNodesTP();
                        var titles = temp.title;
                        var prices = temp.price;
                        List<Product.Product> products = new List<Product.Product>();
                        for(int i = 0; i < 3; i++)
                        {
                            products.Add(new Product.Product() { Title = titles[i].InnerText, Price = Convert.ToInt32(prices[i].InnerText) });
                        }
                        
                        foreach (var item in products)
                            Console.WriteLine(item.ToString());
                        //PARSING PRODUCT
                        break;
                    default:
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Вибач, я лише бот і не розумію твоїх намірів :(");
                        await botClient.SendStickerAsync(message.Chat.Id, "CAACAgIAAxkBAAEFdqFi67d0AyDS_wcRkjAq-J7LX2qSGQACMRcAAkgY2EuTzc3FKasWVykE");
                        break;
                }
            }
            
        }
    

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        public Dialog()
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
            bot.CloseAsync();
        }
    }
}
