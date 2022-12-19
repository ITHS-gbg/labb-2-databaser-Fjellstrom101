using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.Stores;
using DatabaserLabb2LF.ViewModels;

namespace DatabaserLabb2LF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly NavigationStore _navigationStores;
        private readonly Labb2LfContext _dbContext;
        public App()
        {
            _navigationStores = new NavigationStore();
            _dbContext = new Labb2LfContext();
            _navigationStores.CurrentViewModel = new ViewPlaylistsViewModel(_dbContext, _navigationStores);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStores, _dbContext)
            };

            MainWindow.Show();
        }
    }
}
