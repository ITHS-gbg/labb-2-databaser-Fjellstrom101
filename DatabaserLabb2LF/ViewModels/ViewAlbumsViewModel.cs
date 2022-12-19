using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaserLabb2LF.DataAccess.DbModels;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.Stores;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace DatabaserLabb2LF.ViewModels;

public class ViewAlbumsViewModel : ObservableObject
{
    private readonly Labb2LfContext _dbContext;
    private readonly NavigationStore _navigationStore;
    private Album? _selectedAlbum;

    public IEnumerable<Album> Albums
    {
        get => _dbContext.Albums.Include(a => a.Tracks).Include(a => a.Artist).ToList();
    }

    public Album? SelectedAlbum
    {
        get => _selectedAlbum;
        set
        {
            SetProperty(ref _selectedAlbum, value);
            OnPropertyChanged(nameof(AlbumIsSelected));
        }

    }

    public bool AlbumIsSelected => SelectedAlbum is not null;

    public IRelayCommand NewAlbumCommand { get; set; }
    public IRelayCommand EditAlbumCommand { get; set; }
    public IRelayCommand DeleteAlbumCommand { get; set; }

    public ViewAlbumsViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;

        InitCommands();
    }

    public void InitCommands()
    {
        NewAlbumCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditAlbumViewModel(_dbContext, _navigationStore);
        });

        EditAlbumCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditAlbumViewModel(_dbContext, _navigationStore, SelectedAlbum);
        });
        DeleteAlbumCommand = new RelayCommand(() =>
        {
            if (System.Windows.MessageBox.Show(
                    $"Är du säker på att du vill ta bort albumet {SelectedAlbum.Title}? Alla låtar som finns på albumet kommer också att tas bort",
                    "Delete Confirmation", System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            foreach (var track in SelectedAlbum.Tracks)
            {
                var playlistWithTrack = _dbContext.Playlists.Where(p => p.Tracks.Contains(track));
                
                foreach (var playlist in playlistWithTrack)
                {
                    playlist.Tracks.Remove(track);
                }

                _dbContext.Tracks.Remove(track);
            }


            _dbContext.Albums.Remove(SelectedAlbum);
            _dbContext.SaveChanges();
            _navigationStore.CurrentViewModel = new ViewAlbumsViewModel(_dbContext, _navigationStore);
        })
        {

        };
    }
}