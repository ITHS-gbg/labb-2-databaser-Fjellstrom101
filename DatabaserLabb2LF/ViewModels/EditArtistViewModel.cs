using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.DataAccess.DbModels;
using DatabaserLabb2LF.Stores;
using Microsoft.IdentityModel.Tokens;

namespace DatabaserLabb2LF.ViewModels;

public class EditArtistViewModel : ObservableObject
{
    private readonly Labb2LfContext _dbContext;
    private readonly NavigationStore _navigationStore;
    private readonly Artist _artist;
    private Album? _selectedAlbum;

    public string? Name { get; set; }

    public bool ArtistHasAlbums => _dbContext.Albums.Any(a => a.Artist.Equals(_artist));

    public bool AlbumIsNotSelected => (SelectedAlbum is null);

    public IEnumerable<Album> ArtistAlbums => _dbContext.Albums.Where(a => a.Artist.Equals(_artist)).ToList();

    public IEnumerable<Track> ArtistTracks
    {
        get
        {
            return SelectedAlbum is not null ? _dbContext.Tracks.Where(t => t.Album.Equals(SelectedAlbum)).ToList() :
                _dbContext.Tracks.Where(t => t.Album.Artist.Equals(_artist)).ToList();
        }
    }

    public Album? SelectedAlbum
    {
        get => _selectedAlbum;
        set
        {
            SetProperty(ref _selectedAlbum, value);
            OnPropertyChanged(nameof(ArtistTracks));
        }
    }

    public IRelayCommand CancelCommand { get; set;  }
    public IRelayCommand SaveCommand { get; set;  }

    public EditArtistViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {

        _dbContext = dbContext;
        _navigationStore = navigationStore;

        int nextId = _dbContext.Artists.Max(a => a.ArtistId) + 1;
        _artist = new Artist() { ArtistId = nextId };

        InitCommands();
    }
    public EditArtistViewModel(Labb2LfContext dbContext, NavigationStore navigationStore, Artist artist)
    {

        _dbContext = dbContext;
        _navigationStore = navigationStore;

        _artist = artist;
        Name = artist.Name;

        InitCommands();
    }

    public void InitCommands()
    {
        CancelCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewArtistsViewModel(_dbContext, _navigationStore);
        });

        SaveCommand = new RelayCommand(() =>
        {
            if (_dbContext.Artists.Any(a => !string.IsNullOrEmpty(a.Name) && a.Name.ToLower().Equals(Name.ToLower())))
            {
                if (System.Windows.MessageBox.Show(
                        $"Det finns redan en artist med namnet \"{Name}\". Är du säker på att du vill skapa en ny artist med samma namn?",
                        "Delete Confirmation", System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            _artist.Name = Name;

            if (!_dbContext.Artists.Any(a => a.Equals(_artist)))
            {
                _dbContext.Artists.Add(_artist);
            }

            _dbContext.SaveChanges();
            _navigationStore.CurrentViewModel = new ViewArtistsViewModel(_dbContext, _navigationStore);
        });
    }


}