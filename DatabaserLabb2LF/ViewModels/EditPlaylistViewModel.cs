using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaserLabb2LF.DataAccess;
using DatabaserLabb2LF.DataAccess.DbModels;
using DatabaserLabb2LF.Stores;
using Microsoft.EntityFrameworkCore;

namespace DatabaserLabb2LF.ViewModels;

public class EditPlaylistViewModel : ObservableObject
{
    private readonly Playlist _playlist;
    private readonly Labb2LfContext _dbContext;
    private readonly NavigationStore _navigationStore;

    private string? _name;
    private readonly ObservableCollection<Track> _playlistTrackList = new ObservableCollection<Track>();
    private readonly ObservableCollection<Track> _trackList = new ObservableCollection<Track>();
    private Track? _selectedPlaylistTrack;
    private Track? _selectedTrack;

    public IRelayCommand CancelCommand { get; set; }
    public IRelayCommand SaveCommand { get; set; }
    public IRelayCommand AddTrackCommand { get; set; }
    public IRelayCommand DeleteTrackCommand { get; set; }

    public string? Name
    {
        get { return _name; }
        set
        {
            SetProperty(ref _name, value);
            SaveCommand.NotifyCanExecuteChanged();
        }
    }

    public bool PlaylistTrackIsSelected => _selectedPlaylistTrack is not null;

    public bool TrackIsSelected =>  _selectedTrack is not null;

    public Track? SelectedPlayListTrack
    {
        get => _selectedPlaylistTrack;
        set
        {
            SetProperty(ref _selectedPlaylistTrack, value);
            DeleteTrackCommand.NotifyCanExecuteChanged();
        }
    }

    public Track? SelectedTrack
    {
        get => _selectedTrack;
        set
        {
            SetProperty(ref _selectedTrack, value);
            AddTrackCommand.NotifyCanExecuteChanged();
        }
    }

    public IEnumerable<Track> TrackList => _trackList;

    public IEnumerable<Track> PlaylistTrackList => _playlistTrackList;

    public EditPlaylistViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;

        int nextId = (_dbContext.Playlists.Max(p => p.PlaylistId as int?) ?? -1) + 1;
        _playlist = new Playlist(){PlaylistId = nextId};

        InitCommands();
        InitLists();

    }

    public EditPlaylistViewModel(Labb2LfContext dbContext, NavigationStore navigationStore, Playlist playlist)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;
        _playlist = _dbContext.Playlists
            .Include(p => p.Tracks)
            .ThenInclude(t => t.Album).ThenInclude(a => a.Artist)
            .First(p => p.Equals(playlist));


        InitCommands();
        InitLists();

        Name = _playlist.Name;
    }

    private void InitCommands()
    {
        CancelCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new ViewPlaylistsViewModel(_dbContext, _navigationStore);
        });

        SaveCommand = new RelayCommand(() =>
            {
                _playlist.Name = _name;
                _playlist.Tracks.Clear();

                foreach (var track in _playlistTrackList)
                {
                    _playlist.Tracks.Add(track);
                }


                if (!_dbContext.Playlists.Any(a => a.Equals(_playlist)))
                {
                    _dbContext.Playlists.Add(_playlist);
                }

                _dbContext.SaveChanges();
                _navigationStore.CurrentViewModel = new ViewPlaylistsViewModel(_dbContext, _navigationStore);
            },
            () => (!string.IsNullOrEmpty(_name)));
        AddTrackCommand = new RelayCommand(() =>
        {

            _playlistTrackList.Add(SelectedTrack);
            _trackList.Remove(_selectedTrack);

        }, () => TrackIsSelected);
        DeleteTrackCommand = new RelayCommand(() =>
        {

            _trackList.Add(_selectedPlaylistTrack);
            _playlistTrackList.Remove(SelectedPlayListTrack);

            
        }, () =>  PlaylistTrackIsSelected);

    }

    private void InitLists()
    {
        foreach (var track in _playlist.Tracks)
        {
            _playlistTrackList.Add(track);
        }

        var tempList = _dbContext.Tracks
            .Include(t => t.Album)
            .ThenInclude(a => a.Artist)
            .Where(t => !t.Playlists.Contains(_playlist));

        foreach (var track in tempList)
        {
            _trackList.Add(track);
        }
    }
}