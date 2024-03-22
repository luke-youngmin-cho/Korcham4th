namespace MultiThreading
{
    internal class Program
    {
        const int MB = 1024 * 1024;

        static void Main(string[] args)
        {
            Barista barista1 = HireBarista("J");
            Barista barista2 = HireBarista("K");

            Thread t1 = new Thread(() =>
            {
                HireBarista("J")
                    .GoToWork()
                    .MakeRandomBeverage();

            }, 1 * MB);
            t1.Name = barista1.Name;
            t1.IsBackground = true;
            t1.Start();
            t1.Join();

            Thread.Sleep(3000);

            ThreadPool.SetMinThreads(1, 0);
            ThreadPool.SetMaxThreads(4, 4);

            Task task1 = new Task(() =>
            {
                HireBarista("J")
                    .GoToWork()
                    .MakeRandomBeverage();
            });
            task1.Start();
            task1.Wait();

            Task[] tasks = new Task[10];
            
            // 멀티쓰레드 환경에서는 태스크를 할당하고 실행한 순서대로 출력이 순서대로 진행된다는 보장이 없다..!
            for (int i = 0; i < tasks.Length; i++)
            {
                int index = i;
                tasks[i] = new Task(() =>
                {
                    HireBarista($"Barista{index}")
                        .GoToWork()
                        .MakeRandomBeverage();
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
        }

        static Barista HireBarista(string nickname)
        {
            return new Barista(nickname);
        }
    }


    public enum Beverage
    {
        None,
        Aspresso,
        Latte,
        Lemonade,
    }

    public class Barista
    {
        public Barista(string name)
        {
            Name = name;
            _random = new Random();
        }


        public string Name { get; private set; }

        private Random _random;

        public Barista GoToWork()
        {
            Console.WriteLine($"바리스타 {Name} 은 출근합니다 ...");
            return this;
        }

        public Beverage MakeRandomBeverage()
        {
            Beverage beverage = (Beverage)_random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
            Console.WriteLine($"바리스타 {Name} 은 음료 {beverage} 를 제조했습니다.");
            return beverage;
        }
    }
}
