using System;
using System.Collections.Generic;

namespace Task6._13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CarService carService = new CarService();
            carService.Work();
        }
    }

    class CarService
    {
        private List<Box> _boxes = new List<Box>();
        private List<string> _cachBills = new List<string>();
        private int _money = 100;

        public CarService()
        {
            AddStorage();
        }

        public void Work()
        {
            bool isWork = true;

            while (isWork)
            {
                Console.WriteLine("1.Посмотреть склад. \n2.Обслужить клиента. \n3.Показать все чеки за сегодня. \n4.Закончить рабочий день. \nВыберите вариант:");
                string userInput = Console.ReadLine();
                Console.Clear();

                switch (userInput)
                {
                    case "1":
                        ShowStorage();
                        break;
                    case "2":
                        ServeClient();
                        break;
                    case "3":
                        ShowAllCachBills();
                        break;
                    case "4":
                        isWork = false;
                        break;
                }

                WriteMessage();
            }

            Console.WriteLine($"За сегодня вы заработали {_money}");
        }

        private void ShowAllCachBills()
        {
            if (_cachBills.Count > 0)
            {
                foreach (var cachBill in _cachBills)
                {
                    Console.WriteLine(cachBill);
                }
            }
            else
            {
                Console.WriteLine("Чеков за сегодня нет!");
            }
        }

        private void ServeClient()
        {
            Random random = new Random();
            int numberOfBrokenComponent = random.Next(Enum.GetNames(typeof(NamesOfComponents)).Length);
            NamesOfComponents nameOfBrokenComponent = (NamesOfComponents)numberOfBrokenComponent;
            bool isWork = true;
            bool isPayFine = false;
            int priceForWork = 30;
            int priceForRepair = 0;
            int fine = 50;
            Console.WriteLine("К вам приехал клиент!");

            while (isWork)
            {
                Console.WriteLine("1.Узнать поломанную деталь и стоймость работы. \n2.Заменить деталь. \nВыберите вариант:");
                string userInput = Console.ReadLine();
                Console.Clear();

                switch (userInput)
                {
                    case "1":
                        priceForRepair = KnowPriceForBrokenComponent(nameOfBrokenComponent, numberOfBrokenComponent, priceForWork);
                        break;
                    case "2":
                        isWork = MakeRepair(nameOfBrokenComponent, priceForRepair, ref isPayFine, fine);
                        break;
                }

                WriteMessage();
            }

            WriteCashBill(isPayFine, priceForRepair, nameOfBrokenComponent, numberOfBrokenComponent, fine);
        }

        private int KnowPriceForBrokenComponent(NamesOfComponents nameOfBrokenComponent, int numberOfBrokenComponent, int priceForWork)
        {
            int priceForRepair = priceForWork + _boxes[numberOfBrokenComponent].KnowPriceForComponent();
            Console.WriteLine($"У клиента сломано: {nameOfBrokenComponent}.");
            Console.WriteLine($"Стоймость работы: {priceForRepair}.");
            return priceForRepair;
        }

        private bool MakeRepair(NamesOfComponents nameOfBrokenComponent, int priceForRepair, ref bool isPayFine, int fine)
        {
            bool isWork = true;

            if (priceForRepair != 0)
            {
                while (isWork)
                {
                    if (int.TryParse(ChooseBox(priceForRepair), out int numberOfBox))
                    {
                        numberOfBox -= 1;

                        if (numberOfBox < _boxes.Count && numberOfBox >= 0)
                        {
                            isWork = ReplaceComponent(nameOfBrokenComponent, numberOfBox, ref isPayFine, fine);

                            if (isPayFine == false)
                            {
                                _money += priceForRepair;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Данные некорректные");
                        WriteMessage();
                    }
                }
            }
            else
            {
                Console.WriteLine("Ты еще не знаешь сломанную деталь!");
            }

            return isWork;
        }

        private void WriteCashBill(bool isPayFine, int priceForRepair, NamesOfComponents nameOfBrokenComponent, int numberOfBox, int fine)
        {
            string text = "Автомастерская «У Билла» \n"
                       + $"Дата: {DateTime.Now:dd MMMM yyyy}. \n"
                       + $"Время: {DateTime.Now:HH:mm:ss}. \n";

            if (isPayFine == false)
            {
                text += $"За ремонт вы заработали {priceForRepair} монет.\n"
                      + $"В ящике с {nameOfBrokenComponent} осталось {_boxes[numberOfBox].CountOfComponents} деталей. \n";
            }
            else
            {
                text += $"Вы заплатили компенсацию клиенту за предоставленные неудобства в размере {fine} монет. \n";
            }

            text += "Удачного дня! \n";
            Console.WriteLine(text);
            _cachBills.Add(text);
        }

        private bool ReplaceComponent(NamesOfComponents nameOfBrokenComponent, int numberOfBox, ref bool isPayFine, int fine)
        {
            Component component = _boxes[numberOfBox].GetComponent();
            bool isWork = false;
            string text = "Деталь успешно заменена!";

            if (component.Name == nameOfBrokenComponent && _boxes[numberOfBox].CountOfComponents >= 1)
            {
                _boxes[numberOfBox].PickUpComponent();
            }
            else if (component.Name != nameOfBrokenComponent && _boxes[numberOfBox].CountOfComponents >= 1)
            {
                text = "Вы заменили ненужную деталь!";
                _boxes[numberOfBox].PickUpComponent();
                PayFine(fine);
                isPayFine = true;
            }
            else
            {
                text = $"{nameOfBrokenComponent} на складе закончились.";
                PayFine(fine);
                isPayFine = true;
            }

            Console.WriteLine(text);
            return isWork;
        }

        private string ChooseBox(int priceForRepair)
        {
            ShowStorage();
            Console.WriteLine($" \nУсловия: за замену неправильной детали - штраф. За невыполненую работу тоже - штраф. За починку вы получите: {priceForRepair}");
            Console.WriteLine("Введите номер ящика с нужными деталями, чтобы заменить деталь:");
            string userInput = Console.ReadLine();
            Console.Clear();
            return userInput;
        }

        private void PayFine(int fine)
        {
            _money -= fine;
            Console.WriteLine($"Вы заплатили клиенту {fine} монет за компенсацию.");
        }

        private void WriteMessage()
        {
            Console.WriteLine("Для продолжения нажмите любую клавишу:");
            Console.ReadKey();
            Console.Clear();
        }

        private void AddStorage()
        {
            Random random = new Random();
            int minCountOfComponents = 1;
            int maxCountOfComponents = 10;
            int minPrice = 20;
            int maxPrice = 35;

            for (int i = 0; i < Enum.GetNames(typeof(NamesOfComponents)).Length; i++)
            {
                int countOfComponents = random.Next(minCountOfComponents, maxCountOfComponents);
                int price = random.Next(minPrice, maxPrice);
                _boxes.Add(new Box(countOfComponents, new Component((NamesOfComponents)i, price)));
            }
        }

        public void ShowStorage()
        {
            Console.WriteLine("Ваш склад:");

            for (int i = 0; i < _boxes.Count; i++)
            {
                Console.Write(i + 1 + ".");
                _boxes[i].ShowComponents();
            }
        }
    }

    class Box
    {
        private Component _component;

        public int CountOfComponents { get; private set; }

        public Box(int countOfComponents, Component component)
        {
            CountOfComponents = countOfComponents;
            _component = component;
        }

        public int KnowPriceForComponent()
        {
            int price = _component.Price;
            return price;
        }

        public void ShowComponents()
        {
            Console.Write($"Ящик с ");
            _component.ShowInfo();
            Console.WriteLine($"Количество деталей: {CountOfComponents} штук. \n");
        }

        public void PickUpComponent()
        {
            CountOfComponents--;
        }

        public Component GetComponent()
        {
            return _component;
        }
    }

    class Component
    {
        public int Price { get; private set; }
        public NamesOfComponents Name { get; private set; }

        public Component(NamesOfComponents name, int price)
        {
            Name = name;
            Price = price;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"{Name}. Стоймость детали: {Price} монет.");
        }
    }

    enum NamesOfComponents
    {
        Помпа,
        Глушитель,
        Бензонасос,
        ТормозныеДиски,
        РадиаторПечки,
        ПодсветкаНомера,
        ПоршневыеКольца,
        ДатчикТемпературы,
        Термостат,
        СвечиЗажигания,
        КатушкаЗажигания,
        КрышкаТопливногоБака,
        Шина,
    }
}
