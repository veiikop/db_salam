using System;
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
using System.Windows.Shapes;

namespace db_salam
{
    /// <summary>
    /// Логика взаимодействия для EditProductWindow.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        private Products product;
        private salamEntities db;

        // объявление события
        public event Action DataUpdated;

        public EditProductWindow(Products product, salamEntities db)
        {
            InitializeComponent();
            this.product = product;
            this.db = db;

            // заполнение полей данными продукта
            tbStorageID.Text = product.Storage_ID.ToString();
            tbType.Text = product.Type;
            tbTitle.Text = product.Title;
            tbPrice.Text = product.Price.ToString();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            // обновление данных продукта
            product.Storage_ID = Convert.ToInt32(tbStorageID.Text);
            product.Type = tbType.Text;
            product.Title = tbTitle.Text;
            product.Price = Convert.ToDecimal(tbPrice.Text);

            db.SaveChanges(); 

            // вызываем событие, чтобы уведомить о том, что данные обновлены
            DataUpdated?.Invoke();

            this.DialogResult = true; // закрытие окна с результатом
            this.Close();
        }
    }
}
