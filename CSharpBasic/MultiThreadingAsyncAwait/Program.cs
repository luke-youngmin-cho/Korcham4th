namespace MultiThreadingAsyncAwait
{
    internal class Program
    {
        public readonly static object Lock = new object(); // 감시하려는 자물쇠
        public static int Ticks;


        // 어떤 Task 를 취소시키기 위해서 신호를 보내는 객체.
        // Task 할당 시에, 이 Source를 통해 Token 을 발행해서 줄 수 있고, 
        // 이 Source의 Cancel() 요청이 발생했을 때,  Token 을 발행받은 모든 Task 를 취소시킬 수 있다.

        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task t1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Run dummy...");
            },cts.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

            cts.Cancel();

            if (t1.IsCanceled)
            {
                // todo -> Do exception handling
            }

            // 쓰레드풀의 작업 할당을 위한 대기열에 등록
            Task.Run(() =>
            {
                Console.WriteLine("Run dummy...");
            });

            // _ 는 반환 내용에 대해 무시하겠다는 명시
            _ = HireBarista("J")
                    .GoToWork()
                    .MakeRandomBeverage();

            // C# 버전 등의 이슈로 async 를 사용할 수 없는 함수에서는 직접 Task 참조를 통해 Wait 등을 수행해야한다.
            Task<Beverage> task = HireBarista("J")
                                    .GoToWork()
                                    .MakeRandomBeverage();
            task.Wait();


            Task[] tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                int index = i;
                tasks[i] = HireBarista($"Barista {i}")
                                    .GoToWork()
                                    .MakeRandomBeverage();
            }
            Task.WaitAll(tasks);

            Console.WriteLine($"Ticks : {Ticks++}");

            ++Ticks;
            Ticks = Ticks + 1;
            Ticks++;
            PPAF(ref Ticks);
        }

        public static int PPAF(ref int value)
        {
            int origin = value;
            ++value;
            return origin;
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

        private static Dictionary<Beverage, int> s_delayTimes = new Dictionary<Beverage, int>
        {
            { Beverage.Aspresso, 1000 },
            { Beverage.Latte, 3000 },
            { Beverage.Lemonade, 2000 },
        };
        private Random _random;

        public Barista GoToWork()
        {
            Console.WriteLine($"바리스타 {Name} 은 출근합니다 ...");
            return this;
        }

        public async Task<Beverage> MakeRandomBeverage()
        {
            Beverage beverage = (Beverage)_random.Next(1, Enum.GetValues(typeof(Beverage)).Length);
            Console.WriteLine($"바리스타 {Name} 은 음료 {beverage} 제조를 시작했습니다.");

            await Task.Delay(s_delayTimes[beverage]);

            // lock 키워드 : 현재 어플리케이션 내에서 둘이상의 쓰레드 접근을 막기위한 키워드.
            lock (Program.Lock)
            {
                for (int i = 0; i < 100000; i++)
                {
                    Program.Ticks++;
                }
            }

            Monitor.Enter(Program.Lock); // 감시 시작

            for (int i = 0; i < 100000; i++)
            {
                Interlocked.Increment(ref Program.Ticks);
            }

            // Critical Section (임계 영역) : 둘이상의 쓰레드가 접근하면 안되는 공유 자원에 접근하는 영역
            // Critical Section 시작
            for (int i = 0; i < 100000; i++)
            {
                Program.Ticks++;
            }
            // Critical Section 끝

            Monitor.Exit(Program.Lock); // 감시 끝

            Semaphore pool = new Semaphore(0, 3);
            pool.WaitOne(); // 한자리 날때까지 기다림
            // todo -> 크리티컬 섹션 작성
            pool.Release(); // 점유하고있는거 비움

            Mutex mutex = new Mutex();
            mutex.WaitOne();
            // todo ->  크리티컬 섹션 작성
            mutex.ReleaseMutex();

            Console.WriteLine($"바리스타 {Name} 은 음료 {beverage} 제조를 완료했습니다.");
            return beverage;
        }

        private Task Delay(int milliseconds)
        {
            return new Task(() =>
            {
                Thread.Sleep(milliseconds);
            });
        }
    }
}
