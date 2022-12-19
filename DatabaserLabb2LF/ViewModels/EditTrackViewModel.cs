using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.DataAccess.DbModels;
using DatabaserLabb2LF.Stores;
using Microsoft.EntityFrameworkCore;
using Track = DatabaserLabb2LF.DataAccess.DbModels.Track;

namespace DatabaserLabb2LF.ViewModels;

public class EditTrackViewModel : ObservableObject
{
    private readonly Labb2LfContext _dbContext;
    private readonly NavigationStore _navigationStore;
    private readonly Track _track;

    private string _name;
    private Artist? _artist;
    private Album? _album;
    private MediaType? _mediaType;
    private Genre? _genre;
    private string? _composer;
    private int _totalMilliseconds;
    private int? _bytes = 0;
    private double _unitPrice = 0.0D;



    public string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            SaveCommand.NotifyCanExecuteChanged();
        }

    }
    public Artist? Artist
    {
        get => _artist;
        set
        {
            if (value is null) return;
            if (!value.Equals(_artist))
            {
                Album = null;
            }
            SetProperty(ref _artist, value);
            OnPropertyChanged(nameof(AlbumList));
            OnPropertyChanged(nameof(ArtistIsSelected));
            SaveCommand.NotifyCanExecuteChanged();
        }

    }
    public Album? Album
    {
        get => _album;
        set
        {
            SetProperty(ref _album, value);
            SaveCommand.NotifyCanExecuteChanged();
        }

    }

    public MediaType? MediaType
    {
        get => _mediaType;
        set
        {
            SetProperty(ref _mediaType, value);
            SaveCommand.NotifyCanExecuteChanged();
        }

    }
    public Genre? Genre
    {
        get => _genre;
        set
        {
            SetProperty(ref _genre, value);
            SaveCommand.NotifyCanExecuteChanged();
        }

    }

    public string? Composer
    {
        get => _composer;
        set
        {
            SetProperty(ref _composer, value);
            SaveCommand.NotifyCanExecuteChanged();
        }

    }

    public int TotalMilliseconds
    {
        get => _totalMilliseconds;
        set
        {
            SetProperty(ref _totalMilliseconds, value);
            NotifyMillisChanged();
            SaveCommand.NotifyCanExecuteChanged();
        }

    }
    public int? Bytes
    {
        get => _bytes;
        set
        {
            SetProperty(ref _bytes, value);
            SaveCommand.NotifyCanExecuteChanged();
        }

    }

    public double UnitPrice
    {
        get => _unitPrice;
        set
        {
            SetProperty(ref _unitPrice, value);
            SaveCommand.NotifyCanExecuteChanged();
            var test = (!string.IsNullOrEmpty(Name)) &&
                (Artist is not null) &&
                (Album is not null) &&
                (MediaType is not null) &&
                (Genre is not null) &&
                (Composer is not null) &&
                (TotalMilliseconds >= 0) &&
                (Bytes is not null) &&
                (UnitPrice >= 0);
        }

    }

    public int Hours
    {
        get => TotalMilliseconds / 3600000;
        set
        {
            if (value >= 0)
            {
                TotalMilliseconds = Milliseconds +
                                    Seconds * 1000 +
                                    Minutes * 60000 +
                                    value * 3600000;
                NotifyMillisChanged();
            }
        }

    }
    public int Minutes
    {
        get => (TotalMilliseconds % 3600000) / 60000;
        set
        {
            if (value >= 0)
            {
                TotalMilliseconds = Milliseconds +
                                    Seconds * 1000 +
                                    value * 60000 +
                                    Hours * 3600000;
                NotifyMillisChanged();
            }
        }

    }
    public int Seconds
    {
        get => (TotalMilliseconds%60000)/1000;
        set
        {
            if (value >= 0)
            {
                TotalMilliseconds = Milliseconds +
                                    (value * 1000) +
                                    Minutes * 60000 +
                                    Hours * 3600000;
            }

            NotifyMillisChanged();
        }

    }

    public int Milliseconds
    {
        get => TotalMilliseconds % 1000;
        set
        {
            if (value>=0)
            {
                TotalMilliseconds = value +
                                    Seconds * 1000 +
                                    Minutes * 60000 +
                                    Hours * 3600000;
            }

            NotifyMillisChanged();
        }

    }

    public IEnumerable<Artist> ArtistList => _dbContext.Artists.ToList();
    public IEnumerable<Album> AlbumList => _dbContext.Albums.Where(a => a.Artist.Equals(_artist)).ToList();
    public IEnumerable<Genre> GenreList => _dbContext.Genres.ToList();
    public IEnumerable<MediaType> MediaTypeList => _dbContext.MediaTypes.ToList();

    public bool ArtistIsSelected => (Artist is not null);

    public IRelayCommand CancelCommand { get; set; }
    public IRelayCommand SaveCommand { get; set;  }

    public EditTrackViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;

        int nextId = _dbContext.Tracks.Max(t => t.TrackId) + 1;
        _track = new Track(){TrackId = nextId};

        InitCommands();
    }
    public EditTrackViewModel(Labb2LfContext dbContext, NavigationStore navigationStore, Track track)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;
        _track = _dbContext.Tracks
            .Include(t => t.Genre)
            .Include(t => t.MediaType)
            .Include(t => t.Album)
            .ThenInclude(a => a.Artist)
            .First( t => t.Equals(track));

        InitCommands();
        InitProperties(track);
    }

    public void InitProperties(Track track)
    {
        Name = track.Name;

        if (track.Album is not null)
        {
            Artist = track.Album.Artist;
            Album = track.Album;
        }

        MediaType = track.MediaType;
        Genre = track.Genre;
        Composer = track.Composer;
        TotalMilliseconds = track.Milliseconds;
        Bytes = track.Bytes;
        UnitPrice = track.UnitPrice;
    }

    public void InitCommands()
    {
        CancelCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewTracksViewModel(_dbContext, _navigationStore);
        });

        SaveCommand = new RelayCommand(() =>
            {
                _track.Name = Name;
                _track.Album = Album;
                _track.MediaType = MediaType;
                _track.Genre = Genre;
                _track.Composer = Composer;
                _track.Milliseconds = TotalMilliseconds;
                _track.Bytes = Bytes;
                _track.UnitPrice = UnitPrice;
                

                if (!_dbContext.Tracks.Any(a => a.Equals(_track)))
                {
                    _dbContext.Tracks.Add(_track);
                }

                _dbContext.SaveChanges();
                _navigationStore.CurrentViewModel = new ViewTracksViewModel(_dbContext, _navigationStore);
            },
            () =>
            {
                return (!string.IsNullOrEmpty(Name)) &&
                       (Artist is not null) &&
                       (Album is not null) &&
                       (MediaType is not null) &&
                       (Genre is not null) &&
                       (Composer is not null) &&
                       (TotalMilliseconds >= 0) &&
                       (Bytes is not null) &&
                       (UnitPrice >= 0);
            });
    }

    public void NotifyMillisChanged()
    {
        OnPropertyChanged(nameof(Hours));
        OnPropertyChanged(nameof(Minutes));
        OnPropertyChanged(nameof(Seconds));
        OnPropertyChanged(nameof(Milliseconds));
    }

}