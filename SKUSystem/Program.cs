// See https://aka.ms/new-console-template for more information
using SKUSystem;
using System;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
  
            Console.WriteLine("Hello World!");
            
            
            DatabaseManipulator db = new DatabaseManipulator();
            db.GetData();

        }
    }
}