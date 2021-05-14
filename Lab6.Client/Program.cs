﻿using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public struct Date
{
    public int year;
    public int month;
    public int day;


    public Date(int year, int month, int day)
    {
        this.year = year;
        this.month = month;
        this.day = day;
    }


    public void DisplayInfo()
    {
        Console.WriteLine($"year: {year}  month: {month} day:{day}");
    }
}
public class Person
{
    Random random = new Random();

    public string Name { get;  }
    public int Age { get;  }
    public Date Date { get;  }

    public Person()
    {
        Age = random.Next(1, 100);
        Name = "Номер человека в списке: " + Age;
    }
    public Person(string name, int age, Date date)
    {
        Name = name;
        Age = age;
        Date = date;
    }
    public override string ToString()
    {
        return Name;
    }
}
#region передача данных
public class UdpFileClient
{

    // Детали файла
    [Serializable]
    public class FileDetails
    {
        public string FILETYPE = "";
        public long FILESIZE = 0;
    }

    private static FileDetails fileDet;

    // Поля, связанные с UdpClient
    private static int localPort = 5002;
    private static UdpClient receivingUdpClient = new UdpClient(localPort);
    private static IPEndPoint RemoteIpEndPoint = null;

    private static FileStream fs;
    private static Byte[] receiveBytes = new Byte[0];

    [STAThread]
    static void Main(string[] args)
    {
        // Получаем информацию о файле
        GetFileDetails();

        // Получаем файл
        ReceiveFile();

    }
    private static void GetFileDetails()
    {
        try
        {
            Console.WriteLine("-----------*******Ожидание информации о файле*******-----------");

            // Получаем информацию о файле
            receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
            Console.WriteLine("----Информация о файле получена!");

            XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
            MemoryStream stream1 = new MemoryStream();

            // Считываем информацию о файле
            stream1.Write(receiveBytes, 0, receiveBytes.Length);
            stream1.Position = 0;

            // Вызываем метод Deserialize
            fileDet = (FileDetails)fileSerializer.Deserialize(stream1);
            Console.WriteLine("Получен файл типа ." + fileDet.FILETYPE +
                " имеющий размер " + fileDet.FILESIZE.ToString() + " байт");
        }
        catch (Exception eR)
        {
            Console.WriteLine(eR.ToString());
        }
    }
    public static void ReceiveFile()
    {
        try
        {
            Console.WriteLine("-----------*******Ожидайте получение файла*******-----------");

            // Получаем файл
            receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

            // Преобразуем и отображаем данные
            Console.WriteLine("----Файл получен...Сохраняем...");

            // Создаем временный файл с полученным расширением
            fs = new FileStream("File." + fileDet.FILETYPE, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(receiveBytes, 0, receiveBytes.Length);


            Console.WriteLine("----Файл сохранен...");

            Console.WriteLine("-------Открытие файла------");

            

                  

            // Открываем файл связанной с ним программой
            Process.Start(fs.Name);
        }
        catch (Exception eR)
        {
            Console.WriteLine(eR.ToString());
        }
        finally
        {
            
            fs.Close();
            receivingUdpClient.Close();
            Console.Read();
        }
    }
    
}
#endregion