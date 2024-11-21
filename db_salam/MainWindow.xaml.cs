using System;
using QRCoder;
using QRCoder.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OfficeOpenXml;
using System.IO;

namespace db_salam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        salamEntities db;
        public MainWindow()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            db = new salamEntities();
            var productsList = db.Products.ToList();
            var clientsList = db.Clients.ToList();
            if (productsList.Count == 0)
            {
                MessageBox.Show("Список продуктов пуст. Проверьте базу данных.");
            }
            else
            {
                cbProducts.ItemsSource = productsList;
            }
            if (clientsList.Count == 0)
            {
                MessageBox.Show("Список клиентов пуст.");
            }
            else
            {
                cbClients.ItemsSource = clientsList;
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCombinedData();
            RefreshDataGrid();
        }

      

        // перезагрузка продуктов в DataGrid
        private void RefreshDataGrid()
        {
            dgProducts.ItemsSource = db.Products.ToList();
        }

        // свойство для хранения списка продуктов
        public List<Products> Prod { get; set; }


        //фильтрация продуктов на основе combobox
        private void cbP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            dgProducts.ItemsSource = db.Products.Where(x => x.Product_ID == cbProducts.SelectedIndex + 1).ToList(); 

        }
        //фильтрация продуктов на основе textbox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbN.Text))
            {
                dgProducts.ItemsSource = db.Products.Where(x => x.Type.Contains(tbN.Text)).ToList();
            }
            else
            {
                RefreshDataGrid(); //обновление, чтобы показать все продукты, если ввод очищен
            }
        }

        private void combobind()
        {
            salamEntities db = new salamEntities();
            var item = db.Products.ToList();
            Prod = item;
            DataContext = Prod;
        }

        private void Button_Report(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false; //отключение UI во время печати
                PrintDialog pD = new PrintDialog();
                if (pD.ShowDialog() == true)
                {
                    pD.PrintVisual(dgProducts, "докс");
                }
            }
            finally
            {
                this.IsEnabled=true; // включение UI обратно
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close(); // Закрывает текущее окно
        }

        //метод редактирования по двойному щелчку
        private void dgProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgProducts.SelectedItem is var selectedProduct && selectedProduct != null)
            {
                int productId = ((dynamic)selectedProduct).Product_ID;
                var productToEdit = db.Products.Find(productId);

                EditProductWindow editWindow = new EditProductWindow(productToEdit, db);
                editWindow.DataUpdated += RefreshDataGrid; // подписка на событие
                editWindow.ShowDialog();
            }
        }

        //формирование второй таблицы
        private void LoadCombinedData()
        {
            var combinedData = from o in db.Orders
                               join c in db.Clients on o.Client_ID equals c.Client_ID
                               join s in db.Staffs on o.Staff_ID equals s.Staff_ID
                               join p in db.Post on s.Post_ID equals p.Post_ID
                               join pr in db.Products on o.Order_ID equals pr.Product_ID
                               join st in db.Storage on pr.Storage_ID equals st.Storage_ID
                               select new
                               {
                                   o.Order_ID,
                                   ClientName = c.Name_Client + " " + c.Surname_Client,
                                   StaffName = s.Name_Staff + " " + s.Surname_Staff,
                                   PostName = p.PostName,
                                   ProductName = pr.Title,
                                   StorageName = st.Storage_Name,
                                   o.Address,
                                   o.Payment,
                                   o.Status
                               };

            dgCombinedData.ItemsSource = combinedData.ToList();
        }

        private void cbC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedClientId = (cbClients.SelectedItem as Clients)?.Client_ID;

            if (selectedClientId != null)
            {
                var filteredData = from o in db.Orders
                                   join c in db.Clients on o.Client_ID equals c.Client_ID
                                   join s in db.Staffs on o.Staff_ID equals s.Staff_ID
                                   join p in db.Post on s.Post_ID equals p.Post_ID
                                   join pr in db.Products on o.Order_ID equals pr.Product_ID
                                   join st in db.Storage on pr.Storage_ID equals st.Storage_ID
                                   where o.Client_ID == selectedClientId
                                   select new
                                   {
                                       o.Order_ID,
                                       ClientName = c.Name_Client + " " + c.Surname_Client,
                                       StaffName = s.Name_Staff + " " + s.Surname_Staff,
                                       PostName = p.PostName,
                                       ProductName = pr.Title,
                                       StorageName = st.Storage_Name,
                                       o.Address,
                                       o.Payment,
                                       o.Status
                                   };
                dgCombinedData.ItemsSource = filteredData.ToList();
            }
            else
            {
                LoadCombinedData(); // если не выбран клиент, загружаем все данные
            }
        }


        private void Button_Add(object sender, RoutedEventArgs e)
        {
            Products pr = new Products();

            pr.Product_ID = Convert.ToInt32(tbProductID.Text);
            pr.Storage_ID = Convert.ToInt32(tbStorageID.Text);
            pr.Type = tbType.Text;  
            pr.Title = tbTitle.Text;
            pr.Price = Convert.ToDecimal(tbPrice.Text);

            db.Products.Add(pr);
            db.SaveChanges();
            RefreshDataGrid();
        }

        private void Button_Update(object sender, RoutedEventArgs e)
        {
            int sDId = Convert.ToInt32(tbProductID.Text);
            var selectUptId = db.Products.Where(w=>w.Product_ID == sDId).FirstOrDefault();

            if (selectUptId != null)
            {
                selectUptId.Product_ID = Convert.ToInt32(tbProductID.Text);
                selectUptId.Storage_ID = Convert.ToInt32(tbStorageID.Text);
                selectUptId.Type = tbType.Text;
                selectUptId.Title = tbTitle.Text;
                selectUptId.Price = Convert.ToDecimal(tbPrice.Text);
                db.SaveChanges();
                RefreshDataGrid();
            }
            else
            {
                MessageBox.Show("Продукт не найден!");
            }
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            int sDId = Convert.ToInt32(tbProductID.Text);
            var selectDId = db.Products.Where(w => w.Product_ID == sDId).FirstOrDefault();
            if (selectDId != null)
            {
                db.Products.Remove(selectDId);
                db.SaveChanges();
                RefreshDataGrid();
            }
            else
            {
                MessageBox.Show("Продукт не найден!");
            }
        }

        private void Button_OpenQRWindow(object sender, RoutedEventArgs e)
        {
            QR qrWindow = new QR();
            qrWindow.ShowDialog(); // Показываем окно как модальное
        }

        private void ExportToCsv(string filePath)
        {
            var products = db.Products.ToList();
            StringBuilder csvContent = new StringBuilder();

            // Заголовки
            csvContent.AppendLine("Product_ID,Storage_ID,Type,Title,Price");

            // Данные
            foreach (var product in products)
            {
                csvContent.AppendLine($"{product.Product_ID},{product.Storage_ID},{product.Type},{product.Title},{product.Price}");
            }

            // Сохранение в файл с кодировкой UTF-8
            System.IO.File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
        }

        private void ExportToExcel(string filePath)
        {
            var products = db.Products.ToList();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");
                worksheet.Cells[1, 1].Value = "Product_ID";
                worksheet.Cells[1, 2].Value = "Storage_ID";
                worksheet.Cells[1, 3].Value = "Type";
                worksheet.Cells[1, 4].Value = "Title";
                worksheet.Cells[1, 5].Value = "Price";

                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = products[i].Product_ID;
                    worksheet.Cells[i + 2, 2].Value = products[i].Storage_ID;
                    worksheet.Cells[i + 2, 3].Value = products[i].Type;
                    worksheet.Cells[i + 2, 4].Value = products[i].Title;
                    worksheet.Cells[i + 2, 5].Value = products[i].Price;
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }

        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "products.csv",
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportToCsv(saveFileDialog.FileName);
                MessageBox.Show("Данные успешно экспортированы в CSV.");
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "products.xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportToExcel(saveFileDialog.FileName);
                MessageBox.Show("Данные успешно экспортированы в Excel.");
            }
        }

        private void ImportFromCsv(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++) // пропускаем заголовок
            {
                var values = lines[i].Split(',');
                var product = new Products
                {
                    Product_ID = int.Parse(values[0]),
                    Storage_ID = int.Parse(values[1]),
                    Type = values[2],
                    Title = values[3],
                    Price = decimal.Parse(values[4])
                };
                db.Products.Add(product);
            }
            db.SaveChanges();
            RefreshDataGrid();
            MessageBox.Show("Данные успешно импортированы из CSV.");
        }

        private void ImportFromExcel(string filePath)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // первая таблица
                int rows = worksheet.Dimension.Rows;

                for (int i = 2; i <= rows; i++) // пропускаем заголовок
                {
                    var product = new Products();

                    // Проверяем значение и приводим к типу
                    if (worksheet.Cells[i, 1].Value != null && int.TryParse(worksheet.Cells[i, 1].Text, out int productId))
                    {
                        product.Product_ID = productId;
                    }

                    if (worksheet.Cells[i, 2].Value != null && int.TryParse(worksheet.Cells[i, 2].Text, out int storageId))
                    {
                        product.Storage_ID = storageId;
                    }

                    product.Type = worksheet.Cells[i, 3].Text ?? string.Empty; // строка
                    product.Title = worksheet.Cells[i, 4].Text ?? string.Empty; // строка

                    if (worksheet.Cells[i, 5].Value != null && decimal.TryParse(worksheet.Cells[i, 5].Text, out decimal price))
                    {
                        product.Price = price;
                    }
                    else
                    {
                        product.Price = 0; // значение по умолчанию
                    }

                    db.Products.Add(product);
                }
                db.SaveChanges();
                RefreshDataGrid();
                MessageBox.Show("Данные успешно импортированы из Excel.");
            }
        }

        private void ImportFromCsv_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImportFromCsv(openFileDialog.FileName);
            }
        }

        private void ImportFromExcel_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImportFromExcel(openFileDialog.FileName);
            }
        }

        private void OpenAddImageWindow(object sender, RoutedEventArgs e)
        {
            AddImageWindow addImageWindow = new AddImageWindow();
            addImageWindow.ShowDialog(); // Открываем окно как модальное (пользователь не может взаимодействовать с основным окном пока не закроет его)
        }
    }
}
