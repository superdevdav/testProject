using CryptoPortfolio;
using System.Windows;

namespace CryptoPortfolio
{
    public partial class MainWindow : Window
    {
        private readonly PortfolioViewModel _viewModel = new PortfolioViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadPortfolio();
        }
    }
}