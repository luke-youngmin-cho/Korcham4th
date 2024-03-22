using System;

namespace Practice
{
    class CharactorController : Object
    {
    }


    struct ItemData
    {
        public int Id;
        public int Num;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            object obj = new CharactorController();
            obj = new ItemData { Id = 0, Num = 0 }; // boxing
            ItemData data = (ItemData)obj;
            object obj2 = obj;
            long a = 10;
            int b = (int)a;
            a = b; // 승격에 의한 암시적 형변환
            Console.WriteLine("Hello, World!");
        }
    }
}
