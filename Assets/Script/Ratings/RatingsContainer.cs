using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using YARG.Core.Logging;
using YARG.Core.Song;
using YARG.Helpers;

namespace YARG.Ratings
{
    public class SongRating
    {
        public int Fun { get; set; } = 0;
        public int Difficulty { get; set; } = 0;
    }

    public static class RatingsContainer
    {
        public static string RatingsDirectory { get; private set; }
        private static string _ratingsFilePath;

        private static Dictionary<string, SongRating> _ratings = new();

        public static void Initialize()
        {
            RatingsDirectory = Path.Combine(PathHelper.PersistentDataPath, "ratings");
            _ratingsFilePath = Path.Combine(RatingsDirectory, "ratings.json");

            Directory.CreateDirectory(RatingsDirectory);

            if (File.Exists(_ratingsFilePath))
            {
                LoadRatings();
            }
        }

        public static SongRating GetRating(HashWrapper hash)
        {
            var key = hash.ToString();
            return _ratings.TryGetValue(key, out var rating) ? rating : null;
        }

        public static void SetRating(HashWrapper hash, int fun, int difficulty)
        {
            var key = hash.ToString();
            _ratings[key] = new SongRating { Fun = fun, Difficulty = difficulty };
            SaveRatings();
        }

        private static void SaveRatings()
        {
            try
            {
                var text = JsonConvert.SerializeObject(_ratings, Formatting.Indented);
                File.WriteAllText(_ratingsFilePath, text);
            }
            catch (Exception ex)
            {
                YargLogger.LogException(ex, "Failed to save ratings");
            }
        }

        private static void LoadRatings()
        {
            try
            {
                var text = File.ReadAllText(_ratingsFilePath);
                _ratings = JsonConvert.DeserializeObject<Dictionary<string, SongRating>>(text)
                    ?? new Dictionary<string, SongRating>();
            }
            catch (Exception ex)
            {
                YargLogger.LogException(ex, "Failed to load ratings");
                _ratings = new Dictionary<string, SongRating>();
            }
        }
    }
}