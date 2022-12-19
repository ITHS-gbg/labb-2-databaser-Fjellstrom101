using System;
using System.Collections.Generic;

namespace DatabaserLabb2LF.DataAccess.DbModels;

public partial class Album
{
    public int AlbumId { get; set; }

    public string Title { get; set; } = null!;

    public int ArtistId { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual ICollection<Track> Tracks { get; } = new List<Track>();

    public override string ToString()
    {
        return Title;
    }
}
