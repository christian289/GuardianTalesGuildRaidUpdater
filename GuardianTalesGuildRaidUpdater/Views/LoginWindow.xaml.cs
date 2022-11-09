using GuardianTalesGuildRaidUpdater.Models.Messages;

namespace GuardianTalesGuildRaidUpdater.Views
{
    public partial class LoginWindow : Window, IRecipient<DialogResultMessage>
    {
        public LoginWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<DialogResultMessage>(this);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Receive(DialogResultMessage message)
        {
            DialogResult = message.DialogResult;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WeakReferenceMessenger.Default.Unregister<DialogResultMessage>(this);
        }
    }
}
