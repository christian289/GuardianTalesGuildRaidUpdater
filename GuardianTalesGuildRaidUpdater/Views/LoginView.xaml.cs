namespace GuardianTalesGuildRaidUpdater.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            parent.DialogResult = false;
        }
    }
}
