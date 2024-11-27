using System;
using System.IO;

public interface ICipher
{
    string Encode(string sender, string receiver, string message);
    string Decode(string sender, string receiver, string encodedMessage);
}

public class Cipher : ICipher
{
    private int CharToNumber(char c) =>
        char.IsLower(c) ? c - 'a' + 1 :
        char.IsUpper(c) ? c - 'A' + 27 : 0;

    private char NumberToChar(int num) =>
        num <= 26 ? (char)('a' + num - 1) : (char)('A' + num - 27);


    private int CalculateKey(string sender, string receiver)
    {
        int key = 0;
        foreach (char c in sender + receiver)
        {
            key += CharToNumber(c);
            if (key > 52) key %= 52;
        }
        return key;
    }


    public string Encode(string sender, string receiver, string message)
    {
        int key = CalculateKey(sender, receiver);
        string encodedMessage = "";

        foreach (char c in message)
        {
            if (char.IsLetter(c))
            {
                int charNumber = CharToNumber(c);
                int encodedNumber = (charNumber + key) % 52;
                if (encodedNumber == 0) encodedNumber = 52;
                encodedMessage += NumberToChar(encodedNumber);
            }
            else
            {
                encodedMessage += c; 


            }
        }

        return encodedMessage;
    }

    public string Decode(string sender, string receiver, string encodedMessage)
    {
        int key = CalculateKey(sender, receiver);
        string decodedMessage = "";

        foreach (char c in encodedMessage)
        {
            if (char.IsLetter(c))
            {
                int charNumber = CharToNumber(c);
                int decodedNumber = (charNumber - key + 52) % 52;
                if (decodedNumber == 0) decodedNumber = 52;
                decodedMessage += NumberToChar(decodedNumber);
            }
            else
            {
                decodedMessage += c; 
            }
        }

        return decodedMessage;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Cipher cipher = new Cipher();

        Console.WriteLine("Select an operation:");
        Console.WriteLine("1. Encode a message");
        Console.WriteLine("2. Decode a message");
        int choice = int.Parse(Console.ReadLine());

        switch (choice)
        {
            case 1: 
                Console.Write("Enter sender's name: ");
                string sender = Console.ReadLine();

                Console.Write("Enter receiver's name: ");
                string receiver = Console.ReadLine();

                Console.Write("Enter the message to encode: ");
                string message = Console.ReadLine();

                string encodedMessage = cipher.Encode(sender, receiver, message);
                Console.WriteLine($"Encoded Message: {encodedMessage}");

                Console.Write("Enter the full path to save the encoded message (e.g., C:\\path\\to\\file.txt): ");
                string savePath = Console.ReadLine();
                File.WriteAllText(savePath, encodedMessage);
                Console.WriteLine($"Encoded message: {encodedMessage}");
                break;

            case 2: 
                Console.Write("Enter sender's name: ");
                sender = Console.ReadLine();

                Console.Write("Enter receiver's name: ");
                receiver = Console.ReadLine();

                Console.Write("Enter the full path of the file to decode (e.g., C:\\path\\to\\file.txt): ");
                string filePath = Console.ReadLine();

                if (File.Exists(filePath))
                {
                    string encodedFromFile = File.ReadAllText(filePath);
                    string decodedMessage = cipher.Decode(sender, receiver, encodedFromFile);
                    Console.WriteLine($"Decoded Message: {decodedMessage}");
                }
                else
                {
                    Console.WriteLine("File not found! Please check the path.");
                }
                break;

            default:
                Console.WriteLine("Invalid choice. Please restart the program.");
                break;
        }
    }
}
