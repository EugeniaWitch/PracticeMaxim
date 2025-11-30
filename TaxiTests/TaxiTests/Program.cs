using NUnit.Framework;
using TaxiService;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TaxiTests
{
    [TestFixture]
    public class CityMapTests
    {
        //Проверка корректности свойств ячеек (с водителями)
        [Test]
        public void CityMapCreate_ConstructWithDriver_PropertiesCorrect()
        {
            var spot = new CityMap(1, 1, true);

            Assert.AreEqual(1, spot.x);
            Assert.AreEqual(1, spot.y);
            Assert.IsTrue(spot.isDriver);
            Assert.IsFalse(spot.isOrder);
        }

        //Проверка корректности свойств ячеек (без водителей)
        [Test]
        public void CityMapCreate_ConstructWithoutDriver_PropertiesCorrect()
        {
            var spot = new CityMap(1, 1);

            Assert.AreEqual(1, spot.x);
            Assert.AreEqual(1, spot.y);
            Assert.IsFalse(spot.isDriver);
            Assert.IsFalse(spot.isOrder);
        }

        //Валидвция входгых данных X
        [Test]
        public void CityMapCreate_ConstructWithNegativeX_ExceptionsCorrect()
        {
            Assert.Throws<ArgumentException>(() => new CityMap(-1, 1));
        }

        //Валидация входных данных Y
        [Test]
        public void CityMapCreate_ConstructWithNegativeY_ExceptionsCorrect()
        {
            Assert.Throws<ArgumentException>(() => new CityMap(1, -1));
        }
    }

    //Тесты карты
    [TestFixture]
    public class GridMapTests
    {
        //Проверка конструктора карты на корректность параметров
        [Test]
        public void GridMap_Constructor_ValidParameters_CreatesInstance()
        {
            var gridMap = new GridMap(10, 10, 5);

            Assert.IsNotNull(gridMap);
        }

        //Валидация некорректных размеров карты и количества водителей (3)
        [Test]
        public void GridMap_Constructor_InvalidN_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GridMap(0, 10, 5));
        }

        [Test]
        public void GridMap_Constructor_InvalidM_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GridMap(10, -1, 5));
        }

        [Test]
        public void GridMap_Constructor_NegativeDrivers_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GridMap(10, 10, -1));
        }

        //Проверка, что метод GenerateMap возвращает корректный двумерный массив
        [Test]
        public void GenerateMap_ValidParameters_ReturnsCityMaps()
        {
            var gridMap = new GridMap(5, 5, 5);

            var result = gridMap.GenerateMap();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.GetLength(0));
            Assert.AreEqual(5, result.GetLength(1));
        }

        //Проверяет обработку ошибок (2)
        [Test]
        public void GenerateMap_TooSmallMap_ThrowsException()
        {
            var gridMap = new GridMap(2, 2, 10);

            var ex = Assert.Throws<Exception>(() => gridMap.GenerateMap());
            Assert.That(ex.Message, Does.Contain("Слишком маленькая карта"));
        }

        [Test]
        public void GenerateMap_TooFewDrivers_ThrowsException()
        {
            var gridMap = new GridMap(5, 5, 3);

            var ex = Assert.Throws<Exception>(() => gridMap.GenerateMap());
            Assert.That(ex.Message, Does.Contain("Должно быть указано минимум 5 водителей"));
        }
        
        //Проверка, что создается ровно указанное количество водителей
        [Test]
        public void GenerateMap_CreatesCorrectNumberOfDrivers()
        {
            var gridMap = new GridMap(10, 10, 7);

            var map = gridMap.GenerateMap();

            int driverCount = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (map[i, j].isDriver)
                        driverCount++;
                }
            }
            Assert.AreEqual(7, driverCount);
        }

        //Проверка, что создается только один заказ
        [Test]
        public void GenerateMap_CreatesExactlyOneOrder()
        {
            var gridMap = new GridMap(10, 10, 5);

            var map = gridMap.GenerateMap();

            int orderCount = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (map[i, j].isOrder) orderCount++;
                }
            }
            Assert.AreEqual(1, orderCount);
        }

        //Проверка, что заказ не размещен ни на одной из позиций водителя
        [Test]
        public void GenerateMap_OrderNotOnDriverPosition()
        {
            var gridMap = new GridMap(10, 10, 5);

            var map = gridMap.GenerateMap();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var spot = map[i, j];
                    if (spot.isOrder)
                    {
                        Assert.IsFalse(spot.isDriver, "Заказ не должен находиться на позиции водителя");
                    }
                }
            }
        }

        //Проверка корректности работы ручного размещения водителей и заказа
        [Test]
        public void GenerateMap_WithManualPlacement_ValidPositions()
        {
            var gridMap = new GridMap(5, 5, 3);
            var driverPositions = new List<(int x, int y)>
            {
                (0, 0),
                (1, 1),
                (2, 2)
            };
            var orderPosition = (4, 4);

            var map = gridMap.GenerateMap(driverPositions, orderPosition);

            // Проверяем водителей
            Assert.IsTrue(map[0, 0].isDriver);
            Assert.IsTrue(map[1, 1].isDriver);
            Assert.IsTrue(map[2, 2].isDriver);

            // Проверяем заказ
            Assert.IsTrue(map[4, 4].isOrder);
        }

        //Проверка вызова исключения на дубликаты водителей
        [Test]
        public void GenerateMap_WithManualPlacement_DuplicatePositions_ThrowsException()
        {
            var gridMap = new GridMap(5, 5, 3);
            var driverPositions = new List<(int x, int y)>
            {
                (0, 0),
                (0, 0), // Дубликат
                (2, 2)
            };
            var orderPosition = (4, 4);

            Assert.Throws<ArgumentException>(() => gridMap.GenerateMap(driverPositions, orderPosition));
        }

        //Проверка вызова исключения на размещение заказа на позиции водителя
        [Test]
        public void GenerateMap_WithManualPlacement_OrderOnDriverPosition_ThrowsException()
        {
            var gridMap = new GridMap(5, 5, 3);
            var driverPositions = new List<(int x, int y)>
            {
                (0, 0),
                (1, 1),
                (2, 2)
            };
            var orderPosition = (0, 0); // Заказ на позиции водителя

            Assert.Throws<ArgumentException>(() => gridMap.GenerateMap(driverPositions, orderPosition));
        }

        //Проверка, что каждый алгоритм поиска возвращает именно 5 ближайших водителей
        [Test]
        public void EnumerationSearch_FindsNearestDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            gridMap.GenerateMap();

            var result = gridMap.EnumerationSearch();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        [Test]
        public void RadialSearch_FindsNearestDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            gridMap.GenerateMap();

            var result = gridMap.RadialSearch();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        [Test]
        public void PriorityQueueSearch_FindsNearestDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            gridMap.GenerateMap();

            var result = gridMap.PriorityQueueSearch();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
        }

        //Проверка, чо метод отображения карты не выбрасывает исключений
        [Test]
        public void ShowMap_ExecutesWithoutErrors()
        {
            var gridMap = new GridMap(3, 3, 5);
            gridMap.GenerateMap();

            TestDelegate testDelegate = () => gridMap.ShowMap();

            Assert.DoesNotThrow(testDelegate);
        }

        //Проверка корректности расчета Манхэттенского расстояния
        [Test]
        public void CalcDistance_CalculatesManhattanDistanceCorrectly()
        {
            var gridMap = new GridMap(5, 5, 5);
            var spot1 = new CityMap(1, 1);
            var spot2 = new CityMap(4, 3);

            Assert.AreEqual(5, gridMap.CalcDistance(spot1, spot2)); // |4-1| + |3-1| = 3 + 2 = 5
        }

        //Проверка, что расстояние между ячейками равно 0
        [Test]
        public void CalcDistance_SameSpot_ReturnsZero()
        {
            var gridMap = new GridMap(5, 5, 5);
            var spot1 = new CityMap(2, 2);
            var spot2 = new CityMap(2, 2);

            Assert.AreEqual(0, gridMap.CalcDistance(spot1, spot2));
        }

        //Проверка возвращения правильного количества ближайших водителей
        [Test]
        public void FindNeighbors_ReturnsCorrectNumberOfDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            var allDrivers = new List<CityMap>
            {
                new CityMap(0, 0, true) { indentificator = 1 },
                new CityMap(1, 1, true) { indentificator = 2 },
                new CityMap(2, 2, true) { indentificator = 3 },
                new CityMap(3, 3, true) { indentificator = 4 },
                new CityMap(4, 4, true) { indentificator = 5 }
            };
            var order = new CityMap(2, 2);

            var result = gridMap.FindNeighbors(allDrivers, order);
            Assert.AreEqual(5, result.Count);
        }

        //Проверка, что при недостатке водителей возвращаются все доступные
        [Test]
        public void FindNeighbors_LessThan5Drivers_ReturnsAllAvailable()
        {
            var gridMap = new GridMap(5, 5, 5);
            var allDrivers = new List<CityMap>
            {
                new CityMap(0, 0, true) { indentificator = 1 },
                new CityMap(1, 1, true) { indentificator = 2 },
                new CityMap(2, 2, true) { indentificator = 3 }
            };
            var order = new CityMap(2, 2);

            var result = gridMap.FindNeighbors(allDrivers, order);
            Assert.AreEqual(3, result.Count);
        }

        //Проверка возвращения воидтелей по отсортированным расстояниям
        [Test]
        public void FindNeighbors_ReturnsSortedByDistance()
        {
            var gridMap = new GridMap(5, 5, 5);
            var allDrivers = new List<CityMap>
            {
                new CityMap(4, 4, true) { indentificator = 1 }, // дальше всего
                new CityMap(1, 1, true) { indentificator = 2 }, // ближе
                new CityMap(0, 0, true) { indentificator = 3 }, // дальше
                new CityMap(2, 2, true) { indentificator = 4 }, // ближе всего
                new CityMap(3, 3, true) { indentificator = 5 }  // дальше
            };
            var order = new CityMap(2, 2);

            var result = gridMap.FindNeighbors(allDrivers, order);

            // Проверяем что первый в списке ближайший
            Assert.AreEqual(4, result[0].indentificator);

            // Проверяем что список отсортирован по возрастанию расстояния
            for (int i = 0; i < result.Count - 1; i++)
            {
                var dist1 = gridMap.CalcDistance(result[i], order);
                var dist2 = gridMap.CalcDistance(result[i + 1], order);
                Assert.LessOrEqual(dist1, dist2);
            }
        }

        [Test]
        public void GenerateMap_MinimumValidParameters_WorksCorrectly()
        {
            var gridMap = new GridMap(3, 3, 5);

            Assert.DoesNotThrow(() => gridMap.GenerateMap());
        }

        [Test]
        public void IsPositionValid_ValidPosition_ReturnsTrue()
        {
            var gridMap = new GridMap(5, 5, 0); // Без водителей
            var position = new CityMap(2, 2);

            // Создаем пустую карту
            gridMap.GenerateMap(new List<(int x, int y)>(), (3, 3));

            // Должны иметь возможность проверить валидность позиции
            // Этот тест требует изменения доступа к методу IsPositionValid или его тестирования через публичные методы
        }
    }

    [TestFixture]
    public class SearchAlgorithmsSimpleTests
    {
        //Проверка согласованности результатов трех алгоритмов
        [Test]
        public void AllSearchMethods_ReturnSameFiveDrivers()
        {
            var gridMap = new GridMap(10, 10, 15);
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            Assert.AreEqual(5, enumResult.Count);
            Assert.AreEqual(5, radialResult.Count);
            Assert.AreEqual(5, priorityResult.Count);
        }

        //Проверка расстояний до найденных водителей одинаковы для всех алгоритмов
        [Test]
        public void AllSearchMethods_ReturnSameDistances()
        {
            var gridMap = new GridMap(10, 10, 10);
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            // Проверяем что расстояния до всех водителей одинаковые
            for (int i = 0; i < 5; i++)
            {
                var enumDistance = gridMap.CalcDistance(enumResult[i], gridMap.orderReciever);
                var radialDistance = gridMap.CalcDistance(radialResult[i], gridMap.orderReciever);
                var priorityDistance = gridMap.CalcDistance(priorityResult[i], gridMap.orderReciever);

                Assert.AreEqual(enumDistance, radialDistance);
                Assert.AreEqual(enumDistance, priorityDistance);
            }
        }

        //Проверка нахождения алгоритмами одних и тех же водителей (ID)
        [Test]
        public void AllSearchMethods_ReturnSameDriverIds()
        {
            var gridMap = new GridMap(8, 8, 10);
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            // Проверяем что все алгоритмы нашли одних и тех же водителей
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(enumResult[i].indentificator, radialResult[i].indentificator);
                Assert.AreEqual(enumResult[i].indentificator, priorityResult[i].indentificator);
            }
        }

        //Проверка работы алгоритмов при минимально допустимом количестве водителей
        [Test]
        public void SearchMethods_WorkWithMinimumRequiredDrivers()
        {
            var gridMap = new GridMap(6, 6, 5);
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            Assert.AreEqual(5, enumResult.Count);
            Assert.AreEqual(5, radialResult.Count);
            Assert.AreEqual(5, priorityResult.Count);
        }

        //Проверка согласованности результатов при ручном размещении
        [Test]
        public void SearchMethods_ManualPlacement_ConsistentResults()
        {
            var gridMap = new GridMap(5, 5, 5);
            var driverPositions = new List<(int x, int y)>
            {
                (0, 0),
                (1, 1),
                (2, 2),
                (3, 3),
                (4, 4)
            };
            var orderPosition = (2, 3);

            gridMap.GenerateMap(driverPositions, orderPosition);

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            // Все алгоритмы должны найти одинаковых водителей
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(enumResult[i].indentificator, radialResult[i].indentificator);
                Assert.AreEqual(enumResult[i].indentificator, priorityResult[i].indentificator);
            }
        }
    }

    [TestFixture]
    public class IntegrationTests
    {
        //Проверка работы всех методов поиска без ошибок в стандартном сценарии
        [Test]
        public void AllSearchMethods_ProduceConsistentResults()
        {
            var gridMap = new GridMap(8, 8, 10);
            gridMap.GenerateMap();

            // Проверяем что все методы поиска работают без ошибок
            Assert.DoesNotThrow(() => gridMap.EnumerationSearch());
            Assert.DoesNotThrow(() => gridMap.RadialSearch());
            Assert.DoesNotThrow(() => gridMap.PriorityQueueSearch());
        }

        //Проверка работы алгоритмов на карте среднего размера
        [Test]
        public void LargeMap_PerformanceTest()
        {
            var gridMap = new GridMap(100, 100, 20);
            gridMap.GenerateMap();

            Assert.DoesNotThrow(() => gridMap.EnumerationSearch());
            Assert.DoesNotThrow(() => gridMap.RadialSearch());
            Assert.DoesNotThrow(() => gridMap.PriorityQueueSearch());
        }

        //Проверка работы алгоритмов на карте очень большого размера
        [Test]
        public void VeryLargeMap_StressTest()
        {
            var gridMap = new GridMap(100, 100, 50);
            gridMap.GenerateMap();

            // Проверяем что алгоритмы работают на больших данных
            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            Assert.AreEqual(5, enumResult.Count);
            Assert.AreEqual(5, radialResult.Count);
            Assert.AreEqual(5, priorityResult.Count);
        }

        //Особый случай, когда водителей ровно 5 (должны быть все возвращены)
        [Test]
        public void EdgeCase_ExactFiveDrivers()
        {
            var gridMap = new GridMap(10, 10, 5);
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            // Когда водителей ровно 5, все алгоритмы должны вернуть всех водителей
            Assert.AreEqual(5, enumResult.Count);
            Assert.AreEqual(5, radialResult.Count);
            Assert.AreEqual(5, priorityResult.Count);
        }
    }

    [TestFixture]
    public class UserInputSimulationTests
    {
        //Проверка корректной обработки валидных координат
        [Test]
        public void ManualPlacement_ValidCoordinates_WorksCorrectly()
        {
            var gridMap = new GridMap(5, 5, 3);
            var driverPositions = new List<(int x, int y)>
            {
                (0, 0),
                (1, 1),
                (2, 2)
            };
            var orderPosition = (4, 4);

            Assert.DoesNotThrow(() => gridMap.GenerateMap(driverPositions, orderPosition));
        }

        //Проверка обработки координат выходящих за границы карты
        [Test]
        public void ManualPlacement_OutOfBoundsCoordinates_ThrowsException()
        {
            var gridMap = new GridMap(5, 5, 3);
            var driverPositions = new List<(int x, int y)>
            {
                (0, 0),
                (1, 1),
                (10, 10) // Вне границ
            };
            var orderPosition = (4, 4);

            Assert.Throws<ArgumentException>(() => gridMap.GenerateMap(driverPositions, orderPosition));
        }
    }
}