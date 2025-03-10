using System;
using System.IO;

// Интерфейс для генерации числа
public interface INumberGenerator
{
    int Generate(int min, int max);
}

// Реализация генератора чисел
public class RandomNumberGenerator : INumberGenerator
{
    private readonly Random _random = new Random();

    public int Generate(int min, int max)
    {
        return _random.Next(min, max + 1);
    }
}

// Интерфейс для настроек игры
public interface IGameSettings
{
    int MinNumber { get; }
    int MaxNumber { get; }
    int MaxAttempts { get; }
}

// Реализация настроек игры
public class GameSettings : IGameSettings
{
    public int MinNumber { get; }
    public int MaxNumber { get; }
    public int MaxAttempts { get; }

    public GameSettings(int minNumber, int maxNumber, int maxAttempts)
    {
        MinNumber = minNumber;
        MaxNumber = maxNumber;
        MaxAttempts = maxAttempts;
    }
}

// Интерфейс для получения ввода
public interface IInputProvider
{
    int GetUserGuess();
}

// Интерфейс для вывода сообщений
public interface IOutputProvider
{
    void ShowMessage(string message);
}

// Обновленный интерфейс IUserInterface
public interface IUserInterface : IInputProvider, IOutputProvider
{
}

// Реализация консольного интерфейса
public class ConsoleUserInterface : IUserInterface
{
    public int GetUserGuess()
    {
        Console.Write("Введите число: ");
        return int.Parse(Console.ReadLine());
    }

    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }
}

// Реализация файлового интерфейса
public class FileUserInterface : IUserInterface
{
    private readonly string _filePath;

    public FileUserInterface(string filePath)
    {
        _filePath = filePath;
    }

    public int GetUserGuess()
    {
        string input = File.ReadAllText(_filePath);
        return int.Parse(input);
    }

    public void ShowMessage(string message)
    {
        File.AppendAllText(_filePath, message + Environment.NewLine);
    }
}

// Логика игры
public class Game
{
    private readonly INumberGenerator _numberGenerator;
    private readonly IGameSettings _settings;
    private readonly IUserInterface _userInterface;

    public Game(INumberGenerator numberGenerator, IGameSettings settings, IUserInterface userInterface)
    {
        _numberGenerator = numberGenerator;
        _settings = settings;
        _userInterface = userInterface;
    }

    public void Play()
    {
        int targetNumber = _numberGenerator.Generate(_settings.MinNumber, _settings.MaxNumber);
        int attemptsLeft = _settings.MaxAttempts;

        _userInterface.ShowMessage($"Угадайте число от {_settings.MinNumber} до {_settings.MaxNumber}. У вас {attemptsLeft} попыток.");

        while (attemptsLeft > 0)
        {
            int guess = _userInterface.GetUserGuess();

            if (guess == targetNumber)
            {
                _userInterface.ShowMessage("Поздравляем! Вы угадали число!");
                return;
            }

            attemptsLeft--;
            _userInterface.ShowMessage(guess < targetNumber ? "Больше!" : "Меньше!");
            _userInterface.ShowMessage($"Осталось попыток: {attemptsLeft}");
        }

        _userInterface.ShowMessage($"Попытки закончились. Загаданное число: {targetNumber}");
    }
}

// Фабрика для создания игры
public static class GameFactory
{
    public static Game CreateGame(IUserInterface userInterface)
    {
        var settings = new GameSettings(1, 100, 5);
        var numberGenerator = new RandomNumberGenerator();
        return new Game(numberGenerator, settings, userInterface);
    }
}

// Основная программа
class Program
{
    static void Main(string[] args)
    {
        // Использование фабрики
        var userInterface = new ConsoleUserInterface();
        var game = GameFactory.CreateGame(userInterface);
        game.Play();
    }
}