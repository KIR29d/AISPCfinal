using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AISPC.Models;

namespace AISPC.Views.Dialogs
{
    public partial class EditClientDialog : Window
    {
        public Client Client { get; private set; }

        public EditClientDialog(Client client)
        {
            InitializeComponent();
            Client = client;
            LoadClientData();
        }

        private void LoadClientData()
        {
            NameTextBox.Text = Client.Name;
            PhoneTextBox.Text = Client.Phone ?? "";
            EmailTextBox.Text = Client.Email ?? "";
            AddressTextBox.Text = Client.Address ?? "";
            INNTextBox.Text = Client.INN ?? "";
            NotesTextBox.Text = Client.Notes ?? "";

            // Устанавливаем тип клиента
            if (!string.IsNullOrEmpty(Client.ClientType))
            {
                for (int i = 0; i < ClientTypeComboBox.Items.Count; i++)
                {
                    if (((ComboBoxItem)ClientTypeComboBox.Items[i]).Content.ToString() == Client.ClientType)
                    {
                        ClientTypeComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ShowCustomMessage("Ошибка", "Пожалуйста, введите название компании или ФИО клиента.", MessageBoxImage.Warning);
                return;
            }

            // Обновляем данные клиента
            Client.Name = NameTextBox.Text.Trim();
            Client.Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim();
            Client.Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text.Trim();
            Client.Address = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text.Trim();
            Client.INN = string.IsNullOrWhiteSpace(INNTextBox.Text) ? null : INNTextBox.Text.Trim();
            Client.Notes = string.IsNullOrWhiteSpace(NotesTextBox.Text) ? null : NotesTextBox.Text.Trim();
            Client.ClientType = ClientTypeComboBox.SelectedItem != null ? 
                ((ComboBoxItem)ClientTypeComboBox.SelectedItem).Content.ToString() : "Физическое лицо";

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Foreground == Brushes.Gray)
            {
                textBox.Text = "";
                textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F6FC"));
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Логика для placeholder текста если нужно
        }

        private void ShowCustomMessage(string title, string message, MessageBoxImage icon)
        {
            var messageWindow = new CustomMessageBox(title, message, icon);
            messageWindow.Owner = this;
            messageWindow.ShowDialog();
        }
    }
}