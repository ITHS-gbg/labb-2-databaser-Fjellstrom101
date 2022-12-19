using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaserLabb2LF.DataAccess.DbModels;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace DatabaserLabb2LF.ViewModels;

public class ViewArtistsViewModel : ObservableObject
{
    private Labb2LfContext _dbContext;
    private NavigationStore _navigationStore;
    private Artist? _selectectArtist;

    public IEnumerable<Artist> ArtistList
    {
        get => _dbContext.Artists.Include(a => a.Albums).OrderBy(a => a.Name).OrderBy(a => a.ArtistId).ToList();
    }

    public Artist? SelectedArtist
    {
        get => _selectectArtist;
        set
        {
            SetProperty(ref _selectectArtist, value);
            OnPropertyChanged(nameof(ArtistIsSelected));
        }

    }

    public IRelayCommand NewArtistCommand { get; set; }
    public IRelayCommand EditArtistCommand { get; set; }
    public IRelayCommand DeleteArtistCommand { get; set; }

    public bool ArtistIsSelected => SelectedArtist is not null;

    public ViewArtistsViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;

        InitCommands();

    }

    public void InitCommands()
    {
        NewArtistCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditArtistViewModel(_dbContext, _navigationStore);
        });

        EditArtistCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditArtistViewModel(_dbContext, _navigationStore, SelectedArtist);
        });

        DeleteArtistCommand = new RelayCommand(() =>
        {
            if (System.Windows.MessageBox.Show(
                    $"Är du säker på att du vill ta bort {SelectedArtist.Name}? Alla artistens låtar & album kommer också att tas bort",
                    "Delete Confirmation", System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            foreach (var album in SelectedArtist.Albums)
            {
                foreach (var track in album.Tracks)
                {
                    foreach (var playlist in track.Playlists)
                    {
                        playlist.Tracks.Remove(track);
                    }

                    _dbContext.Tracks.Remove(track);
                }

                _dbContext.Albums.Remove(album);
            }

            _dbContext.Artists.Remove(SelectedArtist);
            _dbContext.SaveChanges();
            _navigationStore.CurrentViewModel = new ViewArtistsViewModel(_dbContext, _navigationStore);
        });
    }
}