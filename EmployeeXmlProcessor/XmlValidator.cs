using System;
using System.Xml;
using System.IO;
using System.Text;

namespace EmployeeXmlProcessor
{
    public class XmlValidator
    {
        public static bool ValidateXmlStructure(string xmlPath)
        {
            try
            {
                // ✅ ПРОСТО ПРОВЕРЯЕМ ЧТО ФАЙЛ ВООБЩЕ XML
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                // ✅ ЕСЛИ ЗАГРУЗИЛОСЬ БЕЗ ОШИБОК - ХОРОШО
                return true;
            }
            catch (Exception ex)
            {
                // ✅ НЕ ВЫБРАСЫВАЕМ ОШИБКУ, просто возвращаем false
                Console.WriteLine($"XML validation warning: {ex.Message}");
                return false;
            }
        }

        public static string GetXmlInfo(string xmlPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                return $"Файл: {Path.GetFileName(xmlPath)}\n" +
                       $"Корневой элемент: {doc.DocumentElement?.Name ?? "нет"}";
            }
            catch (Exception ex)
            {
                return $"Файл: {Path.GetFileName(xmlPath)}\nОшибка: {ex.Message}";
            }
        }
    }
}