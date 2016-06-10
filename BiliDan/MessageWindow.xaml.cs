using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace BiliDan
{
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();
        }

        public void AddMessage(string userName, string message)
        {
            var item = new WrapPanel();
            var userNameTextBlock = new TextBlock();
            var messageTextBlock = new TextBlock();

            userNameTextBlock.Text = userName + "：";
            userNameTextBlock.TextWrapping = TextWrapping.Wrap;
            userNameTextBlock.FontWeight = FontWeights.Bold;
            userNameTextBlock.FontSize = 16;
            userNameTextBlock.Margin = new Thickness(5);
            userNameTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0xf7, 0xf6, 0x3c));
            userNameTextBlock.VerticalAlignment = VerticalAlignment.Center;

            messageTextBlock.Text = message;
            messageTextBlock.TextWrapping = TextWrapping.Wrap;
            messageTextBlock.FontSize = 16;
            messageTextBlock.Margin = new Thickness(5);
            messageTextBlock.VerticalAlignment = VerticalAlignment.Center;

            item.Children.Add(userNameTextBlock);
            item.Children.Add(messageTextBlock);

            MessageList.Children.Add(item);
        }

        private void SetWindowPostion()
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 5;
            this.Top = desktopWorkingArea.Bottom - this.Height - 5;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowPostion();
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            SetWindowPostion();
        }
    }
}
