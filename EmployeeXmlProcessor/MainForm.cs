using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace EmployeeXmlProcessor
{
    public partial class MainForm : Form
    {
        private Button btnSelectFile;
        private Button btnProcess;
        private DataGridView dataGridView;
        private Label lblSelectedFile;
        private OpenFileDialog openFileDialog;

        // ПУТИ К ФАЙЛАМ:
        private string xsltFilePath = "C:\\EmployeeXmlProcessor\\EmployeeXmlProcessor\\transform.xslt";        // Лежит рядом с exe
        private string employeesFilePath = "Employees.xml";    // Создаётся программой
        private string currentDataFile = "";                   // Выбирается пользователем

        public MainForm()
        {
            InitializeComponents();
            CheckRequiredFiles();
        }

        private void InitializeComponents()
        {
            // Настройка формы
            this.Size = new Size(800, 600);
            this.Text = "Обработка данных сотрудников";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Кнопка выбора файла
            btnSelectFile = new Button();
            btnSelectFile.Text = "Выбрать XML файл (Data1.xml или Data2.xml)";
            btnSelectFile.Location = new Point(20, 20);
            btnSelectFile.Size = new Size(250, 30);
            btnSelectFile.Click += BtnSelectFile_Click;

            // Метка для выбранного файла
            lblSelectedFile = new Label();
            lblSelectedFile.Location = new Point(280, 25);
            lblSelectedFile.Size = new Size(400, 20);
            lblSelectedFile.Text = "Файл не выбран";
            lblSelectedFile.ForeColor = Color.Gray;

            // Кнопка обработки
            btnProcess = new Button();
            btnProcess.Text = "Запустить обработку";
            btnProcess.Location = new Point(20, 60);
            btnProcess.Size = new Size(150, 30);
            btnProcess.Click += BtnProcess_Click;  // ← ВАЖНО: подключить обработчик!
            btnProcess.Enabled = false;

            Button btnViewXml = new Button();
            btnViewXml.Text = "Просмотр XML";
            btnViewXml.Location = new Point(180, 60); // Рядом с кнопкой обработки
            btnViewXml.Size = new Size(100, 30);
            btnViewXml.Click += (s, e) => ViewXmlContent();
            // Таблица для данных
            dataGridView = new DataGridView();
            dataGridView.Location = new Point(20, 110);
            dataGridView.Size = new Size(740, 400);
            dataGridView.AutoGenerateColumns = true;
            dataGridView.ReadOnly = true;

            // Диалог выбора файла
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите Data1.xml или Data2.xml";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;

            this.Controls.Add(btnSelectFile);
            this.Controls.Add(lblSelectedFile);
            this.Controls.Add(btnProcess);
            this.Controls.Add(btnViewXml); // Добавляем кнопку просмотра
            this.Controls.Add(dataGridView);

            this.ResumeLayout(false);

        }

        // Проверяем необходимые файлы
        private void CheckRequiredFiles()
        {
            if (!File.Exists(xsltFilePath))
            {
                MessageBox.Show($"XSLT файл не найден: {xsltFilePath}\n\nПоместите transform.xslt в папку с программой.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentDataFile = openFileDialog.FileName;
                lblSelectedFile.Text = Path.GetFileName(currentDataFile);
                lblSelectedFile.ForeColor = Color.Black;
                btnProcess.Enabled = true;

                // Показываем информацию о выбранном файле
                ShowFileInfo();
            }
        }

        private void ShowFileInfo()
        {
            try
            {
                // Простая проверка что это XML
                if (Path.GetExtension(currentDataFile).ToLower() != ".xml")
                {
                    MessageBox.Show("Выбранный файл не является XML!", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"Выбран файл: {Path.GetFileName(currentDataFile)}\n\nПуть: {currentDataFile}",
                    "Файл выбран", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения файла: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentDataFile))
            {
                MessageBox.Show("Сначала выберите файл!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Проверяем XML структуру
                if (!XmlValidator.ValidateXmlStructure(currentDataFile))
                {
                    MessageBox.Show("Файл не соответствует ожидаемой структуре XML!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Показываем информацию о файле
                string fileInfo = XmlValidator.GetXmlInfo(currentDataFile);
                MessageBox.Show(fileInfo, "Информация о файле",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: Здесь будет XSLT преобразование
                // Показываем тестовые данные
                ProcessXmlFile(currentDataFile);

                MessageBox.Show("Обработка завершена успешно!", "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DebugFileContents()
        {
            try
            {
                string debugInfo = "=== ДЕБАГ ИНФОРМАЦИЯ ===\n\n";

                // 1. Исходный файл
                if (File.Exists(currentDataFile))
                {
                    debugInfo += $"ИСХОДНЫЙ ФАЙЛ ({currentDataFile}):\n";
                    debugInfo += File.ReadAllText(currentDataFile).Substring(0, 500) + "...\n\n";
                }

                // 2. XSLT файл
                if (File.Exists(xsltFilePath))
                {
                    debugInfo += $"XSLT ФАЙЛ ({xsltFilePath}):\n";
                    debugInfo += "Существует\n\n";
                }

                // 3. Результирующий файл
                if (File.Exists(employeesFilePath))
                {
                    debugInfo += $"РЕЗУЛЬТАТ ({employeesFilePath}):\n";
                    string resultContent = File.ReadAllText(employeesFilePath);
                    debugInfo += resultContent.Length > 500 ? resultContent.Substring(0, 500) + "..." : resultContent;
                }
                else
                {
                    debugInfo += "Результирующий файл НЕ СУЩЕСТВУЕТ\n";
                }

                MessageBox.Show(debugInfo, "Дебаг информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отладки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ProcessXmlFile(string dataFilePath)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnProcess.Enabled = false;

                // 1. XSLT преобразование
                XsltProcessor.TransformXml(dataFilePath, xsltFilePath, employeesFilePath);

                // 2. Добавляем суммы
                XmlHelper.AddSalarySumToEmployees(employeesFilePath);
                XmlHelper.AddAmountSumToSource(dataFilePath);

                // 3. Загружаем данные БЕЗ fallback на заглушку
                LoadRealData();

                MessageBox.Show("Обработка завершена!", "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // ✅ ВРЕМЕННО ПОКАЗЫВАЕМ ТОЛЬКО ОШИБКУ, НЕ ЗАГЛУШКУ
                MessageBox.Show($"Ошибка обработки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // ЗАКОММЕНТИРУЕМ НА ВРЕМЯ ОТЛАДКИ
                // ShowTestData();
            }
            finally
            {
                Cursor = Cursors.Default;
                btnProcess.Enabled = true;
            }
        }
        private void ViewXmlContent()
        {
            if (string.IsNullOrEmpty(currentDataFile)) return;

            try
            {
                string content = File.ReadAllText(currentDataFile, Encoding.UTF8);

                // Покажем первые 1000 символов
                string preview = content.Length > 1000 ? content.Substring(0, 1000) + "..." : content;

                MessageBox.Show($"Содержимое XML:\n\n{preview}", "Просмотр XML",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения файла: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadRealData()
        {
            try
            {
                if (!File.Exists(employeesFilePath))
                {
                    ShowTestData();
                    return;
                }

                // Создаём DataTable с нужными колонками
                DataTable table = new DataTable("EmployeePayments");
                table.Columns.Add("EmployeeID", typeof(string));
                table.Columns.Add("Month", typeof(string));
                table.Columns.Add("Salary", typeof(decimal));
                table.Columns.Add("TotalSalary", typeof(string));

                // Читаем XML и заполняем таблицу
                XmlDocument doc = new XmlDocument();
                doc.Load(employeesFilePath);

                foreach (XmlElement employee in doc.SelectNodes("//Employee"))
                {
                    string employeeId = employee.GetAttribute("id");
                    string totalSalary = employee.GetAttribute("total_salary");

                    foreach (XmlElement month in employee.SelectNodes("Month"))
                    {
                        string monthNumber = month.GetAttribute("number");

                        foreach (XmlElement amount in month.SelectNodes("Amount"))
                        {
                            string salary = amount.GetAttribute("salary");

                            // Добавляем строку для каждой выплаты
                            table.Rows.Add(employeeId, monthNumber, decimal.Parse(salary), totalSalary);
                        }
                    }
                }

                // ОТОБРАЖАЕМ ДАННЫЕ В ТАБЛИЦЕ
                dataGridView.DataSource = table;
                EnhanceGridView();

                MessageBox.Show($"Загружено: {table.Rows.Count} выплат для {doc.SelectNodes("//Employee").Count} сотрудников",
                    "Данные загружены", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowTestData();
            }
        }
        private void EnhanceGridView()
        {
            // Настраиваем внешний вид таблицы
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView.RowHeadersVisible = false;

            // Сортируем по сотрудникам и месяцам
            if (dataGridView.Columns.Contains("EmployeeID") && dataGridView.Columns.Contains("Month"))
            {
                dataGridView.Sort(dataGridView.Columns["EmployeeID"], System.ComponentModel.ListSortDirection.Ascending);
            }
        }
        private void ShowTestData()
        {
            // Заглушка с тестовыми данными
            var table = new System.Data.DataTable();
            table.Columns.Add("EmployeeID", typeof(string));
            table.Columns.Add("Month", typeof(string));
            table.Columns.Add("Salary", typeof(decimal));
            table.Columns.Add("SourceFile", typeof(string));

            table.Rows.Add("1", "01", 1000, Path.GetFileName(currentDataFile));
            table.Rows.Add("1", "01", 1500, Path.GetFileName(currentDataFile));
            table.Rows.Add("1", "02", 1200, Path.GetFileName(currentDataFile));
            table.Rows.Add("2", "01", 800, Path.GetFileName(currentDataFile));

            dataGridView.DataSource = table;
        }
    }
}