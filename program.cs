using System;

abstract class NotificationChannel
{
    protected string ChannelName;

    public int DeliveredCount { get; protected set; }

    public abstract bool IsOnline { get; }

    public NotificationChannel(string name)
    {
        ChannelName = name;
        DeliveredCount = 0;
    }

    public abstract bool Deliver(string recipient, string message);

    public virtual string FormatPayload(string rawMessage)
    {
        return $"[{ChannelName}] {rawMessage}";
    }

    public void PrintAudit()
    {
        Console.WriteLine($"Канал: {ChannelName}");
        Console.WriteLine($"Успешно доставлено: {DeliveredCount}");
        Console.WriteLine();
    }
}

class EmailChannel : NotificationChannel
{
    public override bool IsOnline
    {
        get { return true; }
    }

    public EmailChannel(string name) : base(name)
    {
    }

    public override string FormatPayload(string rawMessage)
    {
        return base.FormatPayload(rawMessage) +
               "\n---\nС уважением, Почтовый Сервис";
    }

    public override bool Deliver(string recipient, string message)
    {
        if (recipient.Contains("@") == false)
        {
            Console.WriteLine("Ошибка: неправильный адрес электронной почты.");
            return false;
        }

        DeliveredCount++;

        Console.WriteLine("Отправка Email:");
        Console.WriteLine($"Получатель: {recipient}");
        Console.WriteLine(FormatPayload(message));
        Console.WriteLine("Письмо успешно отправлено.");
        Console.WriteLine();

        return true;
    }
}

class TelegramChannel : NotificationChannel
{
    private string BotToken;

    public override bool IsOnline
    {
        get { return true; }
    }

    public TelegramChannel(string name, string token) : base(name)
    {
        BotToken = token;
    }

    public override bool Deliver(string recipient, string message)
    {
        if (recipient[0] == '@' == false)
        {
            Console.WriteLine("Ошибка: неправильный Telegram username.");
            return false;
        }

        if (message.Length > 100)
        {
            Console.WriteLine("Ошибка: сообщение превышает 100 символов.");
            return false;
        }

        DeliveredCount++;

        Console.WriteLine("Отправка Telegram:");
        Console.WriteLine($"Получатель: {recipient}");
        Console.WriteLine(FormatPayload(message));
        Console.WriteLine("Сообщение успешно отправлено.");
        Console.WriteLine();

        return true;
    }
}

class SirenAlarmChannel : NotificationChannel
{
    private int RelayLevel = 100;

    public override bool IsOnline
    {
        get { return RelayLevel > 50; }
    }

    public SirenAlarmChannel() : base("Звуковая Сирена")
    {
    }

    public new string FormatPayload(string rawMessage)
    {
        return "[ТРЕВОГА МЧС]: " + rawMessage.ToUpper();
    }

    public override bool Deliver(string recipient, string message)
    {
        if (IsOnline == false)
        {
            Console.WriteLine("Ошибка: сирена недоступна.");
            return false;
        }

        DeliveredCount++;

        Console.WriteLine("!!! ЗВУКОВАЯ ТРЕВОГА !!!");
        Console.WriteLine(FormatPayload(message));
        Console.WriteLine("Сирена активирована.");
        Console.WriteLine();

        return true;
    }
}

class Program
{
    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==========================================");
        Console.WriteLine("       МУЛЬТИКАНАЛЬНЫЙ ШЛЮЗ УВЕДОМЛЕНИЙ");
        Console.WriteLine("==========================================");
        Console.ResetColor();

        NotificationChannel[] channels = new NotificationChannel[3];

        channels[0] = new EmailChannel("Email");
        channels[1] = new TelegramChannel("Telegram", "123456:ABCDEF");
        channels[2] = new SirenAlarmChannel();

        Console.WriteLine("\n1. ПРОВЕРКА ДОСТУПНОСТИ КАНАЛОВ");
        Console.WriteLine("------------------------------------------");

        for (int i = 0; i < channels.Length; i++)
        {
            Console.WriteLine($"{channels[i].IsOnline} - {channels[i].GetType().Name}");
        }

        Console.WriteLine("\n2. ОТПРАВКА УВЕДОМЛЕНИЙ");
        Console.WriteLine("------------------------------------------");

        string message = "Внимание: регламентные технические работы в серверной!";

        string[] recipients =
        {
            "admin@example.com",
            "@student",
            "Все пользователи"
        };

        for (int i = 0; i < channels.Length; i++)
        {
            channels[i].Deliver(recipients[i], message);
        }

        Console.WriteLine("\n3. ДЕМОНСТРАЦИЯ OVERRIDE И NEW");
        Console.WriteLine("------------------------------------------");

        NotificationChannel siren = channels[2];

        Console.WriteLine("Вызов через базовый тип:");
        Console.WriteLine(siren.FormatPayload("Пожарная тревога!"));

        Console.WriteLine();

        SirenAlarmChannel directSiren = (SirenAlarmChannel)siren;

        Console.WriteLine("Вызов через тип SirenAlarmChannel:");
        Console.WriteLine(directSiren.FormatPayload("Пожарная тревога!"));

        Console.WriteLine("\n4. ИТОГОВЫЙ АУДИТ");
        Console.WriteLine("------------------------------------------");

        for (int i = 0; i < channels.Length; i++)
        {
            channels[i].PrintAudit();
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Работа программы завершена.");
        Console.ResetColor();
    }
}
