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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BiliDan.Live;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.IO;
using System.Media;

namespace BiliDan
{
    public partial class MainWindow : Window
    {
        private DanMuListener danMuListener;
        private ContextMenu danMuItemContextMenu;
        private CommandBinding danMuCopyCmdBinding;
        private SoundPlayer danMuMessageTipSoundPlayer;
        private MessageWindow danMuMessageWindow;

        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/BiliDan;component/App.ico")).Stream;

            notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;

            danMuCopyCmdBinding = new CommandBinding(ApplicationCommands.Copy, DanMuCopyCmdExecuted, DanMuCopyCmdCanExecute);

            danMuItemContextMenu = new ContextMenu();
            MenuItem danMuItemContextMenuItem = new MenuItem();

            danMuItemContextMenuItem.Command = ApplicationCommands.Copy;
            danMuItemContextMenu.Items.Add(danMuItemContextMenuItem);

            danMuMessageTipSoundPlayer = new SoundPlayer(BiliDan.Properties.Resources.pop);

            danMuMessageWindow = new MessageWindow();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            int roomId;

            if (!int.TryParse(roomIdTextBox.Text, out roomId))
            {
                MessageBox.Show("直播间编号范围必须为 " + DanMuListener.MinRoomId + "~" + DanMuListener.MaxRoomId);
                return;
            }

            if (roomId < DanMuListener.MinRoomId || roomId > DanMuListener.MaxRoomId)
            {
                MessageBox.Show("直播间编号范围必须为 " + DanMuListener.MinRoomId + "~" + DanMuListener.MaxRoomId);
                return;
            }

            danMuListener = new DanMuListener(roomId);
            danMuListener.Start();

            danMuListener.Message += danMuListener_Message;
            danMuListener.OnlineCountUpdate += danMuListener_OnlineCountUpdate;
            danMuListener.RoomBlockInto += danMuListener_RoomBlockInto;
            danMuListener.RoomBlockMessage += danMuListener_RoomBlockMessage;
            danMuListener.RoomSlientOn += danMuListener_RoomSlientOn;
            danMuListener.RoomSlientOff += danMuListener_RoomSlientOff;

            danMuListener.Error += danMuListener_Error;

            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            roomIdTextBox.IsEnabled = false;
        }

        void danMuListener_RoomSlientOff(object sender, EventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = "管理取消全局禁言";
            item.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            item.CommandBindings.Add(danMuCopyCmdBinding);
            item.ContextMenu = danMuItemContextMenu;

            danmuListBox.Items.Add(item);

            if ((bool)autoScrollDanMuListCheckBox.IsChecked)
            {
                ScrollViewer scrollViewer = RecursiveVisualChildFinder<ScrollViewer>(danmuListBox) as ScrollViewer;

                scrollViewer.ScrollToBottom();
            }
        }

        void danMuListener_RoomSlientOn(object sender, DanMuListenerRoomSilentOnEventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = "管理设置全局禁言 " + e.Countdown + " 秒";
            item.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            item.CommandBindings.Add(danMuCopyCmdBinding);
            item.ContextMenu = danMuItemContextMenu;

            danmuListBox.Items.Add(item);

            if ((bool)autoScrollDanMuListCheckBox.IsChecked)
            {
                ScrollViewer scrollViewer = RecursiveVisualChildFinder<ScrollViewer>(danmuListBox) as ScrollViewer;

                scrollViewer.ScrollToBottom();
            }
        }

        void danMuListener_RoomBlockMessage(object sender, DanMuListenerRoomBlockEventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = "管理员设置用户 \"" + e.UserName + "(" + e.UserId + ")" + "\" 禁言";
            item.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            item.CommandBindings.Add(danMuCopyCmdBinding);
            item.ContextMenu = danMuItemContextMenu;

            danmuListBox.Items.Add(item);

            if ((bool)autoScrollDanMuListCheckBox.IsChecked)
            {
                ScrollViewer scrollViewer = RecursiveVisualChildFinder<ScrollViewer>(danmuListBox) as ScrollViewer;

                scrollViewer.ScrollToBottom();
            }
        }

        void danMuListener_RoomBlockInto(object sender, DanMuListenerRoomBlockEventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = "管理员踢出用户 \"" + e.UserName + "(" + e.UserId + ")\"";
            item.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            item.CommandBindings.Add(danMuCopyCmdBinding);
            item.ContextMenu = danMuItemContextMenu;

            danmuListBox.Items.Add(item);

            if ((bool)autoScrollDanMuListCheckBox.IsChecked)
            {
                ScrollViewer scrollViewer = RecursiveVisualChildFinder<ScrollViewer>(danmuListBox) as ScrollViewer;

                scrollViewer.ScrollToBottom();
            }
        }

        private void danMuListener_Error(object sender, DanMuListenerErrorEventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = e.Exception;

            item.CommandBindings.Add(danMuCopyCmdBinding);
            item.ContextMenu = danMuItemContextMenu;

            danmuListBox.Items.Add(item);

            danMuListener.Start();
        }

        private void danMuListener_OnlineCountUpdate(object sender, DanMuListenerOnlineCountUpdateEventArgs e)
        {
            onlineCountLabel.Content = "在线人数: " + e.Count;
        }

        private void danMuListener_Message(object sender, DanMuListenerMessageEventArgs e)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = "[" + e.SendDateTime.ToString("T") + "] " + e.UserName + "(" + e.UserId + "): " + e.Message;

            item.CommandBindings.Add(danMuCopyCmdBinding);
            item.ContextMenu = danMuItemContextMenu;

            danmuListBox.Items.Add(item);

            if ((bool)autoScrollDanMuListCheckBox.IsChecked)
            {
                ScrollViewer scrollViewer = RecursiveVisualChildFinder<ScrollViewer>(danmuListBox) as ScrollViewer;

                scrollViewer.ScrollToBottom();
            }

            if ((bool)displayDanMuTipCheckBox.IsChecked)
            {
                danMuMessageWindow.AddMessage(e.UserName, e.Message);

                danMuMessageWindow.Show();

                var danMuMessageWindowHideTimer = new DispatcherTimer();
                danMuMessageWindowHideTimer.Interval = new TimeSpan(0, 0, 4);
                danMuMessageWindowHideTimer.Tick += (_sender, _e) =>
                {
                    danMuMessageWindowHideTimer.Stop();
                    danMuMessageWindow.MessageList.Children.RemoveAt(0);

                    if (danMuMessageWindow.MessageList.Children.Count == 0)
                    {
                        danMuMessageWindow.Hide();
                    }
                };

                danMuMessageWindowHideTimer.Start();
            }

            if ((bool)playDanMuTipCheckBox.IsChecked)
            {
                danMuMessageTipSoundPlayer.Play();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            danMuListener.Stop();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            roomIdTextBox.IsEnabled = true;
            onlineCountLabel.Content = "在线人数: 0";

            danmuListBox.Items.Clear();
        }

        private void roomIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextRoomId(e.Text);
        }

        private static bool IsTextRoomId(string text)
        {
            Regex regex = new Regex("[0-9]+");
            return regex.IsMatch(text);
        }

        private static DependencyObject RecursiveVisualChildFinder<T>(DependencyObject rootObject)
        {
            var child = VisualTreeHelper.GetChild(rootObject, 0);
            if (child == null) return null;

            return child.GetType() == typeof(T) ? child : RecursiveVisualChildFinder<T>(child);
        }

        private void DanMuCopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            ListBoxItem danMuItem = target as ListBoxItem;
            Clipboard.SetText((string)danMuItem.Content);
        }

        private void DanMuCopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            danMuMessageWindow.Close();
        }
    }
}
