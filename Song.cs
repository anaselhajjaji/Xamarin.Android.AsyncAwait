using System;
namespace Xamarin.Droid.AsyncAwait
{
    /// <summary>
    /// Song.
    /// </summary>
    public class Song
    {
        /// <summary>
        /// Gets or sets the track identifier.
        /// </summary>
        /// <value>The track identifier.</value>
        public string TrackId { get; set; }

        /// <summary>
        /// Gets or sets the artist full name.
        /// </summary>
        /// <value>The artist.</value>
        public string Artist { get; set; }

        /// <summary>
        /// Gets or sets the song date.
        /// </summary>
        /// <value>The song date.</value>
        public DateTime SongDate { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the track image URL.
        /// </summary>
        /// <value>The track image.</value>
        public string TrackImage { get; set; }
    }
}
