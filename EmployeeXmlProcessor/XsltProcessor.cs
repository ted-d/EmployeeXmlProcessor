using System;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

namespace EmployeeXmlProcessor
{
    public class XsltProcessor
    {
        public static void TransformXml(string inputXmlPath, string xsltPath, string outputXmlPath)
        {
            try
            {
                // Проверяем существование файлов
                if (!File.Exists(inputXmlPath))
                    throw new FileNotFoundException($"Входной XML файл не найден: {inputXmlPath}");

                if (!File.Exists(xsltPath))
                    throw new FileNotFoundException($"XSLT файл не найден: {xsltPath}");

                // Создаём XSLT трансформатор
                XslCompiledTransform xslt = new XslCompiledTransform();

                // Настройки для красивого форматирования
                XsltSettings settings = new XsltSettings(true, true);

                // Загружаем XSLT
                xslt.Load(xsltPath, settings, new XmlUrlResolver());

                // Создаём выходную папку если нужно
                string outputDir = Path.GetDirectoryName(outputXmlPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Выполняем преобразование
                xslt.Transform(inputXmlPath, outputXmlPath);

                Console.WriteLine($"XSLT преобразование завершено: {outputXmlPath}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка XSLT преобразования: {ex.Message}", ex);
            }
        }
    }
}
