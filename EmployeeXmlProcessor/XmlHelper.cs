using System;
using System.Xml;
using System.Linq;

namespace EmployeeXmlProcessor
{
    public class XmlHelper
    {
        public static void AddSalarySumToEmployees(string xmlPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                // Находим всех сотрудников
                XmlNodeList employees = doc.SelectNodes("//Employee");

                if (employees == null || employees.Count == 0)
                {
                    throw new Exception("Не найдены элементы Employee в файле");
                }

                foreach (XmlElement employee in employees)
                {
                    double totalSalary = 0;

                    // Суммируем все salary для этого сотрудника
                    XmlNodeList amounts = employee.SelectNodes(".//Amount[@salary]");

                    foreach (XmlElement amount in amounts)
                    {
                        if (double.TryParse(amount.GetAttribute("salary"), out double salary))
                        {
                            totalSalary += salary;
                        }
                    }

                    // Добавляем атрибут с суммой
                    employee.SetAttribute("total_salary", totalSalary.ToString("F2"));
                }

                doc.Save(xmlPath);
                Console.WriteLine($"Добавлены суммы salary для {employees.Count} сотрудников");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка добавления сумм salary: {ex.Message}");
            }
        }

        public static void AddAmountSumToSource(string xmlPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                // Находим все item элементы
                XmlNodeList items = doc.SelectNodes("//item");

                if (items == null || items.Count == 0)
                {
                    throw new Exception("Не найдены элементы item в исходном файле");
                }

                foreach (XmlElement item in items)
                {
                    // Количество amount элементов
                    int amountCount = item.SelectNodes("amount").Count;
                    item.SetAttribute("amount_count", amountCount.ToString());

                    // Сумма salary для этого item
                    double totalSalary = 0;
                    XmlNodeList amounts = item.SelectNodes("amount[@salary]");

                    foreach (XmlElement amount in amounts)
                    {
                        if (double.TryParse(amount.GetAttribute("salary"), out double salary))
                        {
                            totalSalary += salary;
                        }
                    }

                    item.SetAttribute("total_salary", totalSalary.ToString("F2"));
                }

                doc.Save(xmlPath);
                Console.WriteLine($"Обновлено {items.Count} элементов item в исходном файле");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка обновления исходного файла: {ex.Message}");
            }
        }
    }
}