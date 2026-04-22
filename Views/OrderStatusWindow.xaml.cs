using System.Windows;
using AISPC.Models;

namespace AISPC.Views
{
    public partial class OrderStatusWindow : Window
    {
        private readonly Order _order;

        public OrderStatusWindow(Order order)
        {
            InitializeComponent();
            _order = order;
            LoadOrderData();
        }

        private void LoadOrderData()
        {
            OrderIdText.Text = _order.Id.ToString();
            ClientNameText.Text = _order.ClientName;
            CurrentStatusText.Text = _order.Status;
            
            // Устанавливаем текущий статус в ComboBox
            foreach (var item in StatusComboBox.Items)
            {
                if (item is System.Windows.Controls.ComboBoxItem comboItem && 
                    comboItem.Content.ToString() == _order.Status)
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                var newStatus = selectedItem.Content.ToString();
                _order.Status = newStatus;
                
                if (newStatus == "Завершен")
                {
                    _order.CompletionDate = System.DateTime.Now;
                }
                
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Выберите новый статус", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}