using System.Windows;
using System.Windows.Media;

namespace AISPC.Views.Dialogs
{
    public partial class CustomMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public CustomMessageBox(string title, string message, MessageBoxImage icon = MessageBoxImage.Information, MessageBoxButton button = MessageBoxButton.OK)
        {
            InitializeComponent();
            
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;
            
            SetIcon(icon);
            SetButtons(button);
        }

        private void SetIcon(MessageBoxImage icon)
        {
            switch (icon)
            {
                case MessageBoxImage.Information:
                    IconTextBlock.Text = "ℹ️";
                    IconTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#58A6FF"));
                    break;
                case MessageBoxImage.Warning:
                    IconTextBlock.Text = "⚠️";
                    IconTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F85149"));
                    break;
                case MessageBoxImage.Error:
                    IconTextBlock.Text = "❌";
                    IconTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F85149"));
                    break;
                case MessageBoxImage.Question:
                    IconTextBlock.Text = "❓";
                    IconTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5A5A5"));
                    break;
                default:
                    IconTextBlock.Text = "ℹ️";
                    IconTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#58A6FF"));
                    break;
            }
        }

        private void SetButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    OkButton.Content = "OK";
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    OkButton.Content = "OK";
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Content = "Отмена";
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    OkButton.Content = "Да";
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Content = "Нет";
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    OkButton.Content = "Да";
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Content = "Нет";
                    CancelButton.Visibility = Visibility.Visible;
                    // Для простоты используем только Да/Нет
                    break;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (OkButton.Content.ToString() == "Да")
                Result = MessageBoxResult.Yes;
            else
                Result = MessageBoxResult.OK;
            
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelButton.Content.ToString() == "Нет")
                Result = MessageBoxResult.No;
            else
                Result = MessageBoxResult.Cancel;
            
            DialogResult = false;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            DialogResult = false;
            Close();
        }

        public static MessageBoxResult Show(string message, string title = "Сообщение", MessageBoxImage icon = MessageBoxImage.Information, MessageBoxButton button = MessageBoxButton.OK, Window owner = null)
        {
            var messageBox = new CustomMessageBox(title, message, icon, button);
            if (owner != null)
                messageBox.Owner = owner;
            
            messageBox.ShowDialog();
            return messageBox.Result;
        }
    }
}