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

public class ViewTracksViewModel : ObservableObject
{
    private Labb2LfContext _dbContext;
    private NavigationStore _navigationStore;
    private Track? _selectectTrack;

    public IEnumerable<Track> Tracks
    {
        get => _dbContext.Tracks
            .Include( t => t.Album)
            .Include(a => a.Album.Artist)
            .ToList();
    }

    public Track? SelectedTrack
    {
        get => _selectectTrack;
        set
        {
            SetProperty(ref _selectectTrack, value);
            OnPropertyChanged(nameof(TrackIsSelected));
        }

    }
    public bool TrackIsSelected => SelectedTrack is not null;

    public IRelayCommand NewTrackCommand { get; set; }
    public IRelayCommand EditTrackCommand { get; set; }
    public IRelayCommand DeleteTrackCommand { get; set; }

    public ViewTracksViewModel(Labb2LfContext dbContext, NavigationStore navigationStore)
    {
        _dbContext = dbContext;
        _navigationStore = navigationStore;

        InitCommands();
    }

    private void InitCommands()
    {
        NewTrackCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditTrackViewModel(_dbContext, _navigationStore);
        });

        EditTrackCommand = new RelayCommand(() =>
        {
            _navigationStore.CurrentViewModel = new EditTrackViewModel(_dbContext, _navigationStore, SelectedTrack);
        });

        DeleteTrackCommand = new RelayCommand(() =>
        {
            if (System.Windows.MessageBox.Show(
                    $"Är du säker på att du vill ta bort {SelectedTrack.Name}?",
                    "Delete Confirmation", System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }


            var playlistsWithTrack = _dbContext.Playlists.Where(p => p.Tracks.Contains(SelectedTrack)).Include(p => p.Tracks);

            foreach (var playlist in playlistsWithTrack)
            {
                playlist.Tracks.Remove(SelectedTrack);
            }

            _dbContext.Tracks.Remove(SelectedTrack);
            _dbContext.SaveChanges();
            _navigationStore.CurrentViewModel = new ViewTracksViewModel(_dbContext, _navigationStore);
        });
    }
}