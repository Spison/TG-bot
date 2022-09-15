using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using ClassDataBase;

namespace TelegramBotExperiments
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("5692366788:AAFPDzYrevY3kUyZpQ7pCZSQICjXK_KfU-g");
        //5692366788:AAFPDzYrevY3kUyZpQ7pCZSQICjXK_KfU-g
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            string NameRecipe = "NameRecipe";
            string Materials = "Materials";
            string Recipe = "Recipe";
            string[] all;
            
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    return;
                }
                //В эту конструкцию можно поместить что угодно
                if (message.Text == "/random")
                {
                    GetDBRecipe(ref NameRecipe, ref Materials, ref Recipe);
                    //GetRecipe(ref NameRecipe,ref Materials, ref Recipe);//Из txt файла
                    await botClient.SendTextMessageAsync(message.Chat, NameRecipe);
                    await botClient.SendTextMessageAsync(message.Chat, Materials);
                    //await botClient.SendTextMessageAsync(message.Chat, Recipe);
                }
                if (message.Text== "/all")
                {
                    all= GetAll();
                    foreach (var item in all)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, item);//Вывод пока все all есть
                    }                    
                }
                //await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
        public static string[] GetAll()
        {
            string[] f = System.IO.File.ReadAllLines("D:\\Study\\TelegramBot\\TelegramBot\\Recipes.txt");
            string[] result = new string[f.Length];
            for(int i=0;i<f.Length;i++)
            {
                string[] el = f[i].Split(new char[] { '$' });
                result[i] = el[0];
            }
            return result;
        }
        public static void GetDBRecipe(ref string NameRecipe, ref string Materials, ref string Recipe)
        {
            NameRecipe = "";
            DataBase dataBase = new DataBase();
            string queryString = $"select*from Основная";
            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            //Здесь нужно добавить присвоение
            //string[] lines = null;
            int i = 0;
            while (reader.Read())
            {
                //object id = reader.GetValue(0);
                object name = reader.GetValue(1);
                NameRecipe = (string)name;
                object materials = reader.GetString(2);
                Materials = (string)materials;
                //object photo = reader.GetString(3); // Надо придумать, что делать с картинками

            }
            reader.Close();
            //Здесь можно рандомизировать, какую строчку из lines выводить
            //Идея для оптимизации - узнать количество строк чуть ранее и копировать только ее

        }
        public static void GetAllDBRecipe(ref string NameRecipe, ref string Materials, ref string Recipe)//Доделать в будущем
        {
            NameRecipe = "";
            DataBase dataBase = new DataBase();
            string queryString = $"select*from Основная";
            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            //Здесь нужно добавить присвоение
            string[] lines=null;
            //можно выполнить запрос на количество записей, чтобы сделать цикл for //complete
            while (reader.Read())
            {
                //object id = reader.GetValue(0);
                object name = reader.GetValue(1);
                NameRecipe= (string)name;
                object materials = reader.GetString(2);
                Materials = (string)materials;
                object photo = reader.GetString(3);
                //Как с фото делать????
            }

            reader.Close();
        }
        public static void GetRecipe(ref string NameRecipe,ref string Materials,ref string Recipe)
        {
            //Предположим, что здесь есть чтение с файла "рецепты"
            NameRecipe = "Название: \n";
            Recipe = "Рецепт: \n";
            Materials = "Нам потребуется: \n";
            //Первый вариант - считывать с базы данных с компьютера
            //Номер $ Название $ Материалы? $ Рецепт&
            //StreamReader Recipes = new StreamReader("D:\\Study\\TelegramBot\\TelegramBot\\Recipes.txt");
            string []f = System.IO.File.ReadAllLines("D:\\Study\\TelegramBot\\TelegramBot\\Recipes.txt");
            int count = System.IO.File.ReadAllLines("D:\\Study\\TelegramBot\\TelegramBot\\Recipes.txt").Length;
            Random rnd = new Random();
            string[] el = f[rnd.Next(0, count)].Split(new char[] { '$' });
            NameRecipe += el[0]; // Изъятие имени - здесь вообще, надо еще и картинку. Но тк я работаю не с базой данных, а чисто с txt, то картинки нет
            Materials += el[1]; // Изъятие ингридиентов
            Recipe += el[2]; // Сам рецепт

            //
            //
            //В будущем подключить базу данных SQL для хранения рецептов 
            //
            //
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}