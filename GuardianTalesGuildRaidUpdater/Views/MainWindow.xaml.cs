using GuardianTalesGuildRaidUpdater.Models.Messages;

namespace GuardianTalesGuildRaidUpdater.Views
{
    public partial class MainWindow : Window, IRecipient<DialogResultMessage>
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Receive(DialogResultMessage message)
        {
            if (message.WindowType == typeof(MainWindow))
            {
                DialogResult = message.DialogResult;
            }
        }
    }
}
