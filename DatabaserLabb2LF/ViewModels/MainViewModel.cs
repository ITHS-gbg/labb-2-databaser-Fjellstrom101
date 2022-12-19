using DatabaserLabb2LF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using DatabaserLabb2LF.DataAccess;

namespace DatabaserLabb2LF.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly NavigationStore _navigationStore;
    private readonly Labb2LfContext _dbContext;

    private string _headerText;

    public ObservableObject CurrentViewModel => _navigationStore.CurrentViewModel;

    public string HeaderText
    {
        get => _headerText;
        set
        {
            SetProperty(ref _headerText, value);
        }
    }

    public IRelayCommand NavigateViewPlaylistsCommand { get; set; }
    public IRelayCommand NavigateViewAlbumsCommand { get; set; }
    public IRelayCommand NavigateViewTracksCommand { get; set; }
    public IRelayCommand NavigateViewArtistsCommand { get; set; }

    public MainViewModel(NavigationStore navigationStore, Labb2LfContext dbContext)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        InitCommands();

        _navigationStore.CurrentViewModel = new ViewPlaylistsViewModel(_dbContext, _navigationStore);
        HeaderText = "Spellistor";
    }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    private void InitCommands()
    {
        NavigateViewPlaylistsCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewPlaylistsViewModel(_dbContext, _navigationStore);
            HeaderText = "Spellistor";
        });

        NavigateViewAlbumsCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewAlbumsViewModel(_dbContext, _navigationStore);
            HeaderText = "Album";
        });

        NavigateViewTracksCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewTracksViewModel(_dbContext, _navigationStore);
            HeaderText = "Låtar";
        });

        NavigateViewArtistsCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewArtistsViewModel(_dbContext, _navigationStore);
            HeaderText = "Artister";
        });
    }
}