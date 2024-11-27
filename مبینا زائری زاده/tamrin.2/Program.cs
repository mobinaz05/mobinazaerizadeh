using System;
using System.IO;
using System.Text;

public interface IEncryptDecrypt
{
    string Encode(string message, string sender, string receiver);
    string Decode(string encodedMessage, string sender, string receiver);
}

public class EncryptDecrypt : IEncryptDecrypt
{
    private static int CharToNumber(char c)
    {
        if (c >= 'a' && c <= 'z') return c - 'a' + 1;
        if (c >= 'A' && c <= 'Z') return c - 'A' + 27;
        return -1;
    }

    private static char NumberToChar(int number)
    {
        if (number >= 1 && number <= 26) return (char)('a' + number - 1);
        if (number >= 27 && number <= 52) return (char)('A' + number - 27);
        return '?'; 
    }

    private static int CalculateKey(string sender, string receiver)
    {
        int senderSum = 0, receiverSum = 0;
        foreach (char c in sender)
            senderSum += CharToNumber(c);

        foreach (char c in receiver)
            receiverSum += CharToNumber(c);

        int key = (senderSum * receiverSum) / (senderSum + receiverSum);
        return key > 52 ? key % 52 : key;
    }

    public string Encode(string message, string sender, string receiver)
    {
        int key = CalculateKey(sender, receiver);
        StringBuilder encodedMessage = new StringBuilder();

        foreach (char c in message)
        {
            if (char.IsLetter(c)) 
            {
                int number = CharToNumber(c);
                number += key; 
                if (number > 52) number %= 52; 
                encodedMessage.Append(NumberToChar(number)); 
            }
            else
            {
                encodedMessage.Append(c); 
            }
        }

        return encodedMessage.ToString();
    }

    public string Decode(string encodedMessage, string sender, string receiver)
    {
        int key = CalculateKey(sender, receiver);
        StringBuilder decodedMessage = new StringBuilder();

        foreach (char c in encodedMessage)
        {
            if (char.IsLetter(c)) 
            {
                int number = CharToNumber(c);
                number -= key; 
                if (number < 1) number += 52; 
                decodedMessage.Append(NumberToChar(number)); 
            }
            else
            {
                decodedMessage.Append(c); 
            }
        }

        return decodedMessage.ToString();
    }
}

class Program
{
    static void Main(string[] args)
    {
        IEncryptDecrypt encryptDecrypt = new EncryptDecrypt();

        Console.Write("Enter sender's name: ");
        string sender = Console.ReadLine();

        Console.Write("Enter receiver's name: ");
        string receiver = Console.ReadLine();

        Console.WriteLine("Choose operation:");
        Console.WriteLine("1. Encode");
        Console.WriteLine("2. Decode");
        int choice = int.Parse(Console.ReadLine());

        if (choice == 1)
        {
            Console.Write("Enter the message to encode: ");
            string message = Console.ReadLine();

            string encodedMessage = encryptDecrypt.Encode(message, sender, receiver);
            Console.WriteLine("Encoded Message: " + encodedMessage);

            Console.Write("Enter file path to save encoded message (e.g., C:\\path\\to\\file.txt): ");
            string filePath = Console.ReadLine();
            File.WriteAllText(filePath, encodedMessage);
            Console.WriteLine("Encoded message saved to file.");
        }
        else if (choice == 2)
        {
            Console.Write("Enter the path of the encoded file to decode: ");
            string filePath = Console.ReadLine();

            string encodedMessage = File.ReadAllText(filePath);
            string decodedMessage = encryptDecrypt.Decode(encodedMessage, sender, receiver);
            Console.WriteLine("Decoded Message: " + decodedMessage);
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
    }
}
