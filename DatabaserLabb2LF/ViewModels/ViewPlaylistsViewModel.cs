using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.DataAccess.DbModels;
using DatabaserLabb2LF.Stores;
using Microsoft.EntityFrameworkCore;

namespace DatabaserLabb2LF.ViewModels;

public class ViewPlaylistsViewModel : ObservableObject
{
    private Labb2LfContext _dbContext;
    private NavigationStore _navigationStore;

    private Playlist? _selectectPlaylist;

    public IEnumerable<Playlist> Playlists
    {
        get => _dbContext.Playlists.Include(p => p.Tracks).ToList();
    }

    public Playlist? SelectedPlaylist
    {
        get => _selectectPlaylist;
        set
        {
            SetProperty(ref _selectectPlaylist, value);
            OnPropertyChanged(nameof(PlaylistIsSelected));
        }

    }

    public bool PlaylistIsSelected => SelectedPlaylist is not null;

    public IRelayCommand NewPlaylistCommand { get; set; }
    public IRelayCommand EditPlaylistCommand { get; set; }
    public IRelayCommand DeletePlaylistCommand { get; set; }

    public ViewPlaylistsViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;

        InitCommands();
    }

    private void InitCommands()
    {
        NewPlaylistCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditPlaylistViewModel(_dbContext, _navigationStore);
        });

        EditPlaylistCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditPlaylistViewModel(_dbContext, _navigationStore, SelectedPlaylist);
        });

        DeletePlaylistCommand = new RelayCommand(() =>
        {
            if (System.Windows.MessageBox.Show(
                    $"Är du säker på att du vill ta bort {SelectedPlaylist.Name}?",
                    "Delete Confirmation", System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            SelectedPlaylist.Tracks.Clear();

            _dbContext.Remove(SelectedPlaylist);
            _dbContext.SaveChanges();
            _navigationStore.CurrentViewModel = new ViewPlaylistsViewModel(_dbContext, _navigationStore);
        });
    }


}