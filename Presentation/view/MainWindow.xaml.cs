using System.Windows;
using System.Windows.Media.Animation;
using Presentation.model;
using Presentation.viewModel;

namespace Presentation.view
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            this.mainViewModel = (MainViewModel)DataContext;
        }
        
        public MainWindow(BackendController cont)
        {
            InitializeComponent();
            this.mainViewModel = new MainViewModel(cont);
            this.DataContext = mainViewModel;
        }
        /// <summary> a button click to- register a new user to the system </summary>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.Register();
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            sb.Begin(lblError);
        }
        
        /// <summary> a button click to- Log in an logged in user </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel u = mainViewModel.Login();
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            sb.Begin(lblError);
            if (u != null)
            {
                BoardWindow boardWindow=new BoardWindow(u);
                boardWindow.Show();
                this.Close();
            }
                
        }

        /// <summary> a button click to- delete all the data from the system </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.deleteData();
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            sb.Begin(lblError);
            
        }
    }
}