namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //string result = ExportAlbumsInfo(context, 9);

            string result = ExportSongsAboveDuration(context, 4);

            Console.WriteLine(result);

            //File.WriteAllText("../../../result.txt", result);

        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albumsInfo = context
                //.Albums.AsEnumerable()  // <- MUST to add -> .AsEnumerable()
                //.Where(p => p.ProducerId == producerId)
                //.Select ......

                //or Variant 2
                .Producers
                .FirstOrDefault(p => p.Id == producerId)
                .Albums
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    AlbumPrice = a.Price,
                    CurrAlbum = a.Songs
                        .Select(s => new
                        {
                            SongName = s.Name,
                            SongPrice = s.Price,
                            SongWriterName = s.Writer.Name
                        })
                        .OrderByDescending(x => x.SongName)
                        .ThenBy(x => x.SongWriterName)
                        .ToList()

                })
                .OrderByDescending(x => x.AlbumPrice)
                .ToList();
            
            foreach (var album in albumsInfo)
            {
                sb
                    .AppendLine($"-AlbumName: {album.AlbumName}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine($"-Songs:");

                int counter = 1;

                foreach (var song in album.CurrAlbum)
                {
                    sb
                        .AppendLine($"---#{counter++}")
                        .AppendLine($"---SongName: {song.SongName}")
                        .AppendLine($"---Price: {song.SongPrice:f2}")
                        .AppendLine($"---Writer: {song.SongWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }  // -- ok 100/100 --

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songsInfo = context
                // Variant 1:  to matirialize the data (put ToList() after the .Songs) and remove the last ToList())
                //.Songs
                //.ToList()
                //.Where .....

                //or Variant 2
                .Songs.AsEnumerable()        // <- MUST to add -> .AsEnumerable()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    Duration = s.Duration.ToString("c"),
                    SongWriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    SongPerformerFullName = s.SongPerformers
                        .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                        .FirstOrDefault()
                })
                .OrderBy(x => x.SongName)
                .ThenBy(x => x.SongWriterName)
                .ThenBy(x => x.SongPerformerFullName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            int counter = 1;

            foreach (var currSong in songsInfo)
            {
                sb
                    .AppendLine($"-Song #{counter++}")
                    .AppendLine($"---SongName: {currSong.SongName}")
                    .AppendLine($"---Writer: {currSong.SongWriterName}")
                    .AppendLine($"---Performer: {currSong.SongPerformerFullName}")
                    .AppendLine($"---AlbumProducer: {currSong.AlbumProducer}")
                    .AppendLine($"---Duration: {currSong.Duration}");
            }

            return sb.ToString().TrimEnd();
        }  // -- ok 100/100 --
    }
}
