using System;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;

using Newtonsoft.Json;
using Square.Picasso;

namespace Xamarin.Droid.AsyncAwait
{
    [Activity(Label = "AsyncAwait", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        const string DataURL = "https://raw.githubusercontent.com/anaselhajjaji/xamarin-samples/master/TestData/songs.json";

        List<Song> songs = new List<Song>();

        RecyclerView recyclerView;

        Button button;

        async void ClickOnButton(object sender, EventArgs e)
        {
            ProgressDialog dialog = new ProgressDialog(this);
            dialog.SetMessage("Loading...");
            dialog.Indeterminate = true;
            dialog.SetCancelable(false);
            dialog.Show();

            List<Song> songsFromBackend = await FetchSongs(DataURL);

            // Update recycler views
            songs.Clear();
            songs.AddRange(songsFromBackend);
            recyclerView.GetAdapter().NotifyDataSetChanged();

            dialog.Dismiss();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //Cheeseknife.Inject(this);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            button = FindViewById<Button>(Resource.Id.syncButton);

            // Initialize the recycler view
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(new SongsAdapter(songs));

            button.Click += ClickOnButton;
        }

        async Task<List<Song>> FetchSongs(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                Toast.MakeText(this, "Deserializing JSON", ToastLength.Short).Show();
                List<Song> fetchedSongs = await Task.Run(() => DeserializeJSON(response));
                Toast.MakeText(this, "JSON Deserialized.", ToastLength.Short).Show();
                return fetchedSongs;
            }
        }

        List<Song> DeserializeJSON(WebResponse response)
        {
            // Get a stream representation of the HTTP web response:
            using (Stream stream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (List<Song>)serializer.Deserialize(streamReader, typeof(List<Song>));
            }
        }

        #region Recycler Initialization

        public class SongsAdapter : RecyclerView.Adapter
        {
            /// <summary>
            /// Gets or sets the songs.
            /// </summary>
            /// <value>The songs.</value>
            public List<Song> Songs { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="AsyncAwait.SongsAdapter"/> class.
            /// </summary>
            /// <param name="songs">Songs.</param>
            public SongsAdapter(List<Song> songs)
            {
                Songs = songs;
            }

            /// <summary>
            /// The View Holder
            /// </summary>
            public class SongViewHolder : RecyclerView.ViewHolder
            {

                TextView artistTv;

                TextView timeTv;

                TextView trackIdTv;

                TextView titleTv;

                ImageView songImage;

                /// <summary>
                /// Initializes a new instance of the <see cref="AsyncAwait.SongsAdapter+SongViewHolder"/> class.
                /// </summary>
                /// <param name="view">View.</param>
                public SongViewHolder(View view) : base(view)
                {
                    artistTv = view.FindViewById<TextView>(Resource.Id.artistTv);
                    timeTv = view.FindViewById<TextView>(Resource.Id.timesTv);
                    trackIdTv = view.FindViewById<TextView>(Resource.Id.trackIdTv);
                    titleTv = view.FindViewById<TextView>(Resource.Id.titleTv);
                    songImage = view.FindViewById<ImageView>(Resource.Id.songImage);
                }

                /// <summary>
                /// Binds the view holder.
                /// </summary>
                /// <param name="song">Song.</param>
                public void BindViewHolder(Song song)
                {
                    artistTv.Text = song.Artist;
                    timeTv.Text = song.SongDate.ToString();
                    trackIdTv.Text = song.TrackId;
                    titleTv.Text = song.Title;

                    // Download image
                    Picasso.With(titleTv.Context)
                        .Load(song.TrackImage)
                        .Into(songImage);
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Item, parent, false);
                return new SongViewHolder(view);
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                Song song = Songs[position];
                (holder as SongViewHolder).BindViewHolder(song);
            }

            /// <summary>
            /// Gets the item count.
            /// </summary>
            /// <value>The item count.</value>
            public override int ItemCount
            {
                get
                {
                    return Songs.Count;
                }
            }
        }

        #endregion
    }
}

