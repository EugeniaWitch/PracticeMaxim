using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxiService
{
    //Создаем класс, который будет хранить всю инорфмацию о городе, водителях и заказе
    public class CityMap
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool Driver { get; set; }
        public bool Order { get; set; }
        public int identifier { get; set; }

        //Пеерегруженный конструктор с водителями
        public CityMap(int x, int y, bool Driver)
        {
            if (x < 0) throw new ArgumentException("Значение x должно быть положительным", nameof(x));
            if (y < 0) throw new ArgumentException("Значение y должно быть положительным", nameof(y));
            this.x = x;
            this.y = y;
            this.Driver = Driver;
        }

        //Перегруженный конструктор без водителей (по умолчанию False)
        public CityMap(int x, int y)
        {
            if (x < 0) throw new ArgumentException("Значение x должно быть положительным", nameof(x));
            if (y < 0) throw new ArgumentException("Значение y должно быть положительным", nameof(y));
            this.x = x;
            this.y = y;
        }
    }

    //Класс, производящий операции создания карты и поиска ближайших водителей
    public class GridMap
    {
        CityMap[,] cityMaps;
        Random random;
        public CityMap orderReciever;
        private int n;
        private int m;
        private int drivers;

        //Обычный конструктор
        public GridMap(int n, int m, int drivers)
        {
            if (n <= 0) throw new ArgumentException("Размер n должен быть положительным числом", nameof(n));
            if (m <= 0) throw new ArgumentException("Размер m должен быть положительным числом", nameof(m));
            if (drivers < 0) throw new ArgumentException("Количество водителей не может быть отрицательным", nameof(drivers));
            this.n = n;
            this.m = m;
            this.drivers = drivers;
            this.random = new Random();
        }

        //Перегруженный метод для инциализации пустой карты и генерации водителей и заказа
        public CityMap[,] GenerateMap()
        {
            if (drivers + 1 > n * m) throw new Exception("Слишком маленькая карта, пожалуйста, укажите большие значения");
            if (drivers < 5) throw new Exception("Должно быть указано минимум 5 водителей");

            cityMaps = new CityMap[n, m];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    cityMaps[i, j] = new CityMap(i, j);
                }
            }
            DriversGenerator(cityMaps, drivers); //генерируем водителей относительно карты
            OrderGeneration(cityMaps); //генерируем заказ относительно карты
            return cityMaps;
        }

        // Перегруженный метод для ручного размещения водителей и заказа
        public CityMap[,] GenerateMap(List<(int x, int y)> driverPositions, (int x, int y) orderPosition)
        {
            if (n * m < drivers + 1) throw new Exception("Слишком маленькая карта, пожалуйста, укажите большие значения");
            if (drivers < 5) throw new Exception("Должно быть указано минимум 5 водителей");

            cityMaps = new CityMap[n, m];

            // Инициализируем пустую карту
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    cityMaps[i, j] = new CityMap(i, j);
                }
            }

            // Размещаем водителей по заданным координатам
            for (int i = 0; i < driverPositions.Count; i++)
            {
                var pos = driverPositions[i];
                if (pos.x < 0 || pos.x >= n || pos.y < 0 || pos.y >= m)
                    throw new ArgumentException($"Координаты водителя ({pos.x}, {pos.y}) выходят за границы карты");

                if (cityMaps[pos.x, pos.y].Driver)
                    throw new ArgumentException($"Позиция ({pos.x}, {pos.y}) уже занята другим водителем");

                cityMaps[pos.x, pos.y].Driver = true;
                cityMaps[pos.x, pos.y].identifier = i + 1;
            }

            // Размещаем заказ
            if (orderPosition.x < 0 || orderPosition.x >= n || orderPosition.y < 0 || orderPosition.y >= m)
                throw new ArgumentException($"Координаты заказа ({orderPosition.x}, {orderPosition.y}) выходят за границы карта");

            if (cityMaps[orderPosition.x, orderPosition.y].Driver)
                throw new ArgumentException($"Позиция заказа ({orderPosition.x}, {orderPosition.y}) уже занята водителем");

            cityMaps[orderPosition.x, orderPosition.y].Order = true;
            orderReciever = cityMaps[orderPosition.x, orderPosition.y];

            return cityMaps;
        }

        //Отображение города
        public void ShowMap()
        {
            Console.WriteLine("\n=== КАРТА ГОРОДА ===");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var spot = cityMaps[i, j];

                    if (spot.Driver)
                    {
                        Console.Write($"[{spot.identifier}]"); //Водитель (отображается в виде своего идентификатора)
                    }
                    else if (spot.Order)
                    {
                        Console.Write("[*]"); //Заказ
                    }
                    else
                    {
                        Console.Write("[0]"); //Пустая точка (ячнйка)
                    }
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        //Генерация координат водителей
        public void DriversGenerator(CityMap[,] cityMaps, int dCount)
        {
            int drivers = 0;
            int rn_x;
            int rn_y;
            while (drivers < dCount)
            {
                rn_x = random.Next(0, n);
                rn_y = random.Next(0, m);
                if (!cityMaps[rn_x, rn_y].Driver) //Проверка, чтобы не занимать точку с уже имеющимся водителем
                {
                    cityMaps[rn_x, rn_y].Driver = true;
                    cityMaps[rn_x, rn_y].identifier = drivers + 1;
                    drivers++;
                }
            }
        }

        //Генрация координат заказа
        public void OrderGeneration(CityMap[,] cityMaps)
        {
            int order = 0;
            int rn_x;
            int rn_y;
            while (order != 1)
            {
                rn_x = random.Next(0, n);
                rn_y = random.Next(0, m);
                if (!cityMaps[rn_x, rn_y].Driver) //Проверка, чтобы не занимать точку с уже имющимя водителем
                {
                    cityMaps[rn_x, rn_y].Order = true;
                    order++;
                    orderReciever = cityMaps[rn_x, rn_y];
                }
            }
        }

        // Поиск через перебор точек карты
        public List<CityMap> EnumerationSearch()
        {
            List<CityMap> allDrivers = new List<CityMap>();
            List<CityMap> total;
            foreach (var spot in cityMaps)
            {
                if (spot.Driver)
                {
                    allDrivers.Add(spot);
                }
            }

            //После обнаружения всех водителей на карте ищет самых ближайших к заказу
            total = FindNeighbors(allDrivers, orderReciever);
            return total;
        }

        // Увеличиваем радиус-квадрат поиска от заказа, пока не насчитаем 5 водителей (радиальный поиск)
        public List<CityMap> RadialSearch()
        {
            int curDrivers = 0;
            List<CityMap> allDrivers = new List<CityMap>();
            int maxRadius = Math.Max(n, m);

            for (int i = 1; i < maxRadius; i++)
            {
                // Регулировка по высоте
                for (int j = -i; j <= i; j++)
                {
                    // Проверка выхода за границы
                    if (orderReciever.x + j >= n || orderReciever.x + j < 0)
                    {
                        continue;
                    }
                    // Регулировка по ширине
                    for (int w = -i; w <= i; w++)
                    {
                        // Проверка выхода за границы
                        if (orderReciever.y + w >= m || orderReciever.y + w < 0)
                        {
                            continue;
                        }
                        // Учитываем только новые ячейки по периметру, проверенные не обходим
                        if (-i < j && i > j)
                        {
                            if (orderReciever.y + i < m && cityMaps[orderReciever.x + j, orderReciever.y + i].Driver)
                            {
                                curDrivers++;
                                allDrivers.Add(cityMaps[orderReciever.x + j, orderReciever.y + i]);
                            }

                            if (orderReciever.y - i >= 0 && cityMaps[orderReciever.x + j, orderReciever.y - i].Driver)
                            {
                                curDrivers++;
                                allDrivers.Add(cityMaps[orderReciever.x + j, orderReciever.y - i]);
                            }
                            break;
                        }
                        if (cityMaps[orderReciever.x + j, orderReciever.y + w].Driver)
                        {
                            curDrivers++;
                            allDrivers.Add(cityMaps[orderReciever.x + j, orderReciever.y + w]);
                        }
                    }
                }

                // Если найдено 5 и более водителей
                if (curDrivers >= 5)
                {
                    List<CityMap> total = FindNeighbors(allDrivers, orderReciever);

                    // Если расстояние до заказа больше радиуса итерации, возможно наличие более близкого водителя на большем радиусе i
                    if (CalcDistance(total.Last(), orderReciever) > i && i != maxRadius - 1)
                    {
                        continue;
                    }
                    return total;
                }
            }
            return allDrivers.Take(5).ToList();
        }

        // Поиск через приоритетную очередь
        public List<CityMap> PriorityQueueSearch()
        {
            var nearest = new CityMap[5];
            var distances = new int[5];

            // Инициализируем максимальными значениями
            for (int i = 0; i < 5; i++)
            {
                distances[i] = int.MaxValue;
            }

            // Один проход по всем водителям
            foreach (var spot in cityMaps)
            {
                if (!spot.Driver) continue;

                int distance = CalcDistance(spot, orderReciever);

                // Вставляем в отсортированный массив если расстояние меньше
                for (int i = 0; i < 5; i++)
                {
                    if (distance < distances[i])
                    {
                        // Сдвигаем элементы
                        for (int j = 4; j > i; j--)
                        {
                            nearest[j] = nearest[j - 1];
                            distances[j] = distances[j - 1];
                        }

                        nearest[i] = spot;
                        distances[i] = distance;
                        break;
                    }
                }
            }

            return nearest.Where(c => c != null).ToList();
        }

        public int CalcDistance(CityMap spot1, CityMap spot2)
        {
            // Вычисляем Манхэттенское расстояние
            int distance = Math.Abs(spot2.x - spot1.x) + Math.Abs(spot2.y - spot1.y);
            return distance;
        }

        // Находит 5 ближайших водителей из всего списка
        public List<CityMap> FindNeighbors(List<CityMap> allDrivers, CityMap order)
        {
            List<CityMap> neighborsDrivers = new List<CityMap>();
            List<CityMap> driversCopy = new List<CityMap>(allDrivers); // Работаем с копией

            for (int i = 0; i < 5 && driversCopy.Count > 0; i++)
            {
                CityMap curMin = driversCopy[0];

                foreach (var spot in driversCopy)
                {
                    if (CalcDistance(spot, order) < CalcDistance(curMin, order))
                    {
                        curMin = spot;
                    }
                }

                neighborsDrivers.Add(curMin);
                driversCopy.Remove(curMin);
            }

            return neighborsDrivers;
        }
    }

    //Стандартное тестирование
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class SearchAlgorithmsBenchmarks
    {
        private GridMap smallMap;
        private GridMap mediumMap;
        private GridMap largeMap;
        private GridMap veryLargeMap;

        [GlobalSetup]
        public void Setup()
        {
            // Маленькая карта (10x10)
            smallMap = new GridMap(10, 10, 5);
            smallMap.GenerateMap();

            // Средняя карта (100x100)
            mediumMap = new GridMap(100, 100, 10);
            mediumMap.GenerateMap();

            // Большая карта (500x500)
            largeMap = new GridMap(500, 500, 50);
            largeMap.GenerateMap();

            // Очень большая карта (1000x1000)
            veryLargeMap = new GridMap(1000, 1000, 100);
            veryLargeMap.GenerateMap();
        }
        //Проходимся по каждой карте, чтобы выснить, какой метод лучше работает
        // Маленькая карта
        [Benchmark]
        public void SmallMap_EnumerationSearch() => smallMap.EnumerationSearch();

        [Benchmark]
        public void SmallMap_RadialSearch() => smallMap.RadialSearch();

        [Benchmark]
        public void SmallMap_PriorityQueueSearch() => smallMap.PriorityQueueSearch();

        // Средняя карта
        [Benchmark]
        public void MediumMap_EnumerationSearch() => mediumMap.EnumerationSearch();

        [Benchmark]
        public void MediumMap_RadialSearch() => mediumMap.RadialSearch();

        [Benchmark]
        public void MediumMap_PriorityQueueSearch() => mediumMap.PriorityQueueSearch();

        // Большая карта
        [Benchmark]
        public void LargeMap_EnumerationSearch() => largeMap.EnumerationSearch();

        [Benchmark]
        public void LargeMap_RadialSearch() => largeMap.RadialSearch();

        [Benchmark]
        public void LargeMap_PriorityQueueSearch() => largeMap.PriorityQueueSearch();

        // Очень большая карта
        [Benchmark]
        public void VeryLargeMap_EnumerationSearch() => veryLargeMap.EnumerationSearch();

        [Benchmark]
        public void VeryLargeMap_RadialSearch() => veryLargeMap.RadialSearch();

        [Benchmark]
        public void VeryLargeMap_PriorityQueueSearch() => veryLargeMap.PriorityQueueSearch();
    }
    //Особое тестирование
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AlgorithmScenariosBenchmarks
    {
        private GridMap fewDriversOnVeryLargeMap;     //Мало водителей на большой карте
        private GridMap fewDriversOnSmallMap;         //Мало водителей на маленькой карте
        private GridMap manyDriversOnVeryLargeMap;    //Много водителей на большой карте
        private GridMap manyDriversOnSmallMap;         //Много водителей на маленько  карте

        [GlobalSetup]
        public void Setup()
        {
            //Мало водителей на большой карте
            fewDriversOnVeryLargeMap = new GridMap(1000, 1000, 10);
            fewDriversOnVeryLargeMap.GenerateMap();

            //Мало водителей на маленькой карте
            fewDriversOnSmallMap = new GridMap(30, 30, 5);
            fewDriversOnSmallMap.GenerateMap();

            //Много водителей на большой карте
            manyDriversOnVeryLargeMap = new GridMap(1000, 1000, 500000);
            manyDriversOnVeryLargeMap.GenerateMap();

            //Много водителей на маленькой карте
            manyDriversOnSmallMap = new GridMap(30, 30, 50);
            manyDriversOnSmallMap.GenerateMap();
        }

        //Мало водителей на большой карте
        [Benchmark]
        public void fewDriversOnVeryLargeMap_EnumerationSearch() => fewDriversOnVeryLargeMap.EnumerationSearch();

        [Benchmark]
        public void fewDriversOnVeryLargeMap_RadialSearch() => fewDriversOnVeryLargeMap.RadialSearch();

        [Benchmark]
        public void fewDriversOnVeryLargeMap_PriorityQueueSearch() => fewDriversOnVeryLargeMap.PriorityQueueSearch();

        //Мало водителей на маленькой карте
        [Benchmark]
        public void fewDriversOnSmallMap_EnumerationSearch() => fewDriversOnSmallMap.EnumerationSearch();

        [Benchmark]
        public void fewDriversOnSmallMap_RadialSearch() => fewDriversOnSmallMap.RadialSearch();

        [Benchmark]
        public void fewDriversOnSmallMap_PriorityQueueSearch() => fewDriversOnSmallMap.PriorityQueueSearch();

        //Много водителей на большой карте
        [Benchmark]
        public void manyDriversOnVeryLargeMap_EnumerationSearch() => manyDriversOnVeryLargeMap.EnumerationSearch();

        [Benchmark]
        public void manyDriversOnVeryLargeMap_RadialSearch() => manyDriversOnVeryLargeMap.RadialSearch();

        [Benchmark]
        public void manyDriversOnVeryLargeMap_PriorityQueueSearch() => manyDriversOnVeryLargeMap.PriorityQueueSearch();

        //Много водителей на маленькой карте
        [Benchmark]
        public void manyDriversOnSmallMap_EnumerationSearch() => manyDriversOnSmallMap.EnumerationSearch();

        [Benchmark]
        public void manyDriversOnSmallMap_RadialSearch() => manyDriversOnSmallMap.RadialSearch();

        [Benchmark]
        public void manyDriversOnSmallMap_PriorityQueueSearch() => manyDriversOnSmallMap.PriorityQueueSearch();
    }

        //Основная работа программы
        internal class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== СИСТЕМА ПОИСКА БЛИЖАЙШИХ ВОДИТЕЛЕЙ ===");

            while (true)
            {
                Console.WriteLine("\nВыберите режим:");
                Console.WriteLine("1 - Бенчмарки (тестирование скорости)");
                Console.WriteLine("2 - Обычная работа программы");
                Console.WriteLine("3 - Выход");
                Console.Write("Введите номер режима: ");

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    RunBenchmarkMode();
                }
                else if (choice == "2")
                {
                    RunNormalMode();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Выход из программы...");
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                }
            }
        }

        //Метод, запускающий тестирование
        static void RunBenchmarkMode()
        {
            Console.WriteLine("\nВыберите режим тестирования:");
            Console.WriteLine("1 - Стандартное тестирование по размерам карт");
            Console.WriteLine("2 - Тестирование с малым и большим количеством водителей");
            Console.Write("Введите номер режима: ");
            var choice =Console.ReadLine();
            if (choice == "1")
            {
                Console.WriteLine("\nЗапуск бенчмарков...");
                var summary1 = BenchmarkRunner.Run<SearchAlgorithmsBenchmarks>();
            }
           else if (choice == "2")
            {
                Console.WriteLine("\nЗапуск бенчмарков...");
                var summary2 = BenchmarkRunner.Run<AlgorithmScenariosBenchmarks>();
            }
            else
            {
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
            }

        }

        //Метод, запускающий базовое использование поиска
        static void RunNormalMode()
        {
            try
            {
                // Ввод параметров города
                Console.WriteLine("\n--- НАСТРОЙКА ГОРОДА ---");

                Console.Write("Введите ширину города (N): ");
                int n = int.Parse(Console.ReadLine() ?? "0");

                Console.Write("Введите высоту города (M): ");
                int m = int.Parse(Console.ReadLine() ?? "0");

                Console.Write("Введите количество водителей (минимум 5): ");
                int drivers = int.Parse(Console.ReadLine() ?? "0");

                // Выбор способа размещения
                Console.WriteLine("\n--- СПОСОБ РАЗМЕЩЕНИЯ ---");
                Console.WriteLine("1 - Автоматическое размещение (случайное)");
                Console.WriteLine("2 - Ручное размещение координат");
                Console.Write("Выберите способ: ");

                var placementChoice = Console.ReadLine();
                GridMap map;

                if (placementChoice == "1")
                {
                    // Автоматическое размещение (генерация)
                    map = new GridMap(n, m, drivers);
                    map.GenerateMap();
                }
                else if (placementChoice == "2")
                {
                    // Ручное размещение
                    var driverPositions = new List<(int x, int y)>();

                    Console.WriteLine("\n--- РАЗМЕЩЕНИЕ ВОДИТЕЛЕЙ ---");
                    for (int i = 0; i < drivers; i++)
                    {
                        Console.WriteLine($"Водитель {i + 1}:");
                        Console.Write("  X координата: ");
                        int x = int.Parse(Console.ReadLine() ?? "0");
                        Console.Write("  Y координата: ");
                        int y = int.Parse(Console.ReadLine() ?? "0");

                        driverPositions.Add((x, y));
                    }

                    Console.WriteLine("\n--- РАЗМЕЩЕНИЕ ЗАКАЗА ---");
                    Console.Write("X координата заказа: ");
                    int orderX = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Y координата заказа: ");
                    int orderY = int.Parse(Console.ReadLine() ?? ")");

                    map = new GridMap(n, m, drivers);
                    map.GenerateMap(driverPositions, (orderX, orderY));
                }
                else
                {
                    Console.WriteLine("Неверный выбор способа размещения!");
                    return;
                }

                // Показываем карту
                map.ShowMap();

                // Запускаем поиск всеми алгоритмами
                Console.WriteLine("\n--- РЕЗУЛЬТАТЫ ПОИСКА ---");

                var algorithms = new[]
                {
                    new { Name = "Радиальный поиск", Method = new Func<List<CityMap>>(map.RadialSearch) },
                    new { Name = "Переборный поиск", Method = new Func<List<CityMap>>(map.EnumerationSearch) },
                    new { Name = "Поиск с приоритетной очередью", Method = new Func<List<CityMap>>(map.PriorityQueueSearch) }
                };

                foreach (var algorithm in algorithms)
                {
                    Console.WriteLine($"\n{algorithm.Name}:");
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var result = algorithm.Method();
                    stopwatch.Stop();

                    Console.WriteLine($"Время выполнения: {stopwatch.Elapsed.TotalMilliseconds:F4} мс");
                    Console.WriteLine("Ближайшие водители:");

                    foreach (var driver in result)
                    {
                        int distance = map.CalcDistance(driver, map.orderReciever);
                        Console.WriteLine($"  Водитель ID: {driver.identifier} | Позиция: ({driver.x}, {driver.y}) | Расстояние: {distance}");
                    }
                }

                // Сравнение результатов
                Console.WriteLine("\n--- СРАВНЕНИЕ АЛГОРИТМОВ ---");

                var firstAlgorithmDrivers = algorithms[0].Method().Select(d => d.identifier).ToHashSet();
                bool allSame = true;

                foreach (var algorithm in algorithms.Skip(1))
                {
                    var currentDrivers = algorithm.Method().Select(d => d.identifier).ToHashSet();
                    if (!currentDrivers.SetEquals(firstAlgorithmDrivers))
                    {
                        allSame = false;
                        break;
                    }
                }

                Console.WriteLine($"Все алгоритмы нашли одинаковых водителей: {(allSame ? "ДА" : "НЕТ")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}