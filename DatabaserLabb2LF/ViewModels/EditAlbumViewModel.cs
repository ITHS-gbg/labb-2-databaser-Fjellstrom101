using CommunityToolkit.Mvvm.ComponentModel;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.DataAccess.DbModels;
using DatabaserLabb2LF.Stores;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;

namespace DatabaserLabb2LF.ViewModels;

public class EditAlbumViewModel : ObservableObject
{
    private readonly Labb2LfContext _dbContext;
    private readonly NavigationStore _navigationStore;
    private readonly Album _album;

    private string _title;
    private Artist _artist;

    public string Title
    {
        get { return _title; }
        set
        {
            SetProperty(ref _title, value);
            SaveCommand.NotifyCanExecuteChanged();
        }
    }

    public Artist? SelectedArtist
    {
        get => _artist;
        set
        {
            SetProperty(ref _artist, value);
            SaveCommand.NotifyCanExecuteChanged();
        }
    }
    public bool AlbumHasTracks => _dbContext.Tracks.Any(t => t.Album.Equals(_album));

    public IEnumerable<Track> AlbumTracks => _dbContext.Tracks.Where(t => t.Album.Equals(_album)).ToList();
    public IEnumerable<Artist> ArtistList => _dbContext.Artists.ToList();


    public IRelayCommand CancelCommand { get; set; }
    public IRelayCommand SaveCommand { get; set; }

    public EditAlbumViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;

        int nextId = _dbContext.Albums.Max(a => a.AlbumId) + 1;
        _album = new Album(){AlbumId = nextId};

        InitCommands();
    }

    public EditAlbumViewModel(Labb2LfContext dbContext, NavigationStore navigationStore, Album album)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;
        _album = _dbContext.Albums
            .Include(a => a.Artist)
            .First(a => a.Equals(album));


        InitCommands();


        Title = album.Title;
        SelectedArtist = album.Artist;
    }

    public void InitCommands()
    {
        CancelCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewAlbumsViewModel(_dbContext, _navigationStore);
        });

        SaveCommand = new RelayCommand(() =>
            {
                _album.Title = _title;
                _album.Artist = SelectedArtist;

                if (!_dbContext.Albums.Any(a => a.Equals(_album)))
                {
                    _dbContext.Albums.Add(_album);
                }
                _dbContext.SaveChanges();
                _navigationStore.CurrentViewModel = new ViewAlbumsViewModel(_dbContext, _navigationStore);
            },
            () =>
            {
                return (!string.IsNullOrEmpty(Title)) &&
                       (SelectedArtist is not null);
            });
    }


}