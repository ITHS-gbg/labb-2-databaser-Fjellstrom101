using System;
using System.Collections.Generic;

namespace DatabaserLabb2LF.DataAccess.DbModels;

public partial class Artist
{
    public int ArtistId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Album> Albums { get; } = new List<Album>();
}
