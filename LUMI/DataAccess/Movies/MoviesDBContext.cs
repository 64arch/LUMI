using System.Data;
using LUMI.Models;
using MySql.Data.MySqlClient;

namespace LUMI.DataAccess.Movies;

public class MoviesDBContext {
    
    public static async Task<List<Movie?>> GetMoviesAsync() {
        var movies = new List<Movie?>();
        var connection = DBController.GetConnection();

        try {
            await DBController.OpenConnectionAsync();

            await using var cmd = new MySqlCommand("SELECT * FROM movies", connection);
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) {
                movies.Add(new Movie {
                    ID = rdr.GetInt32(rdr.GetOrdinal("id")),
                    Poster = rdr.GetString(rdr.GetOrdinal("poster")),
                    LargePoster = rdr.GetString(rdr.GetOrdinal("large_poster")),
                    Title = rdr.GetString(rdr.GetOrdinal("title")),
                    Tags = rdr
                        .GetString(rdr.GetOrdinal("tags"))
                        .Split(",")
                        .ToList() ?? new List<string>(),
                    Rating = rdr.GetFloat(rdr.GetOrdinal("rating")),
                    Description = rdr.GetString(rdr.GetOrdinal("description")),
                    CreatedAt = rdr.GetDateTime(rdr.GetOrdinal("created_at"))
                });
            }

        }
        catch (Exception ex) {
            Console.WriteLine($"Error: {ex}");
        }
        finally {
            await DBController.CloseConnectionAsync();
        }

        return movies;   
    }
    
    public static async Task<Movie> GetMovieByIDAsync(int movieID) {
        var movies = new List<Movie>();
        var connection = DBController.GetConnection();

        try {
            await DBController.OpenConnectionAsync();

            await using var cmd = new MySqlCommand("SELECT * FROM `movies` WHERE `id` = @movieID", connection);
            cmd.Parameters.AddWithValue("@movieID", movieID);
            await using var rdr = await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync()) {
                return new Movie {
                    ID = rdr.GetInt32(rdr.GetOrdinal("id")),
                    TmdbID = rdr.GetString(rdr.GetOrdinal("tmdb_id")),
                    Poster = rdr.GetString(rdr.GetOrdinal("poster")),
                    LargePoster = rdr.GetString(rdr.GetOrdinal("large_poster")),
                    Title = rdr.GetString(rdr.GetOrdinal("title")),
                    Tags = rdr
                        .GetString(rdr.GetOrdinal("tags"))
                        .Split(",")
                        .ToList() ?? new List<string>(),
                    Description = rdr.GetString(rdr.GetOrdinal("description")),
                    ReleaseDate = rdr.GetDateTime(rdr.GetOrdinal("release_date")),
                    Rating = rdr.GetFloat(rdr.GetOrdinal("rating")),
                    Media = rdr
                        .GetString(rdr.GetOrdinal("media"))
                        .Split(",")
                        .ToList(),
                    Duration = rdr.GetString(rdr.GetOrdinal("duration")),
                    DownloadLink = rdr.GetString(rdr.GetOrdinal("download_link")),
                    CreatedAt = rdr.GetDateTime(rdr.GetOrdinal("created_at"))
                };
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error: {ex}");
        }
        finally {
            await DBController.CloseConnectionAsync();
        }

        return null;
    }

    public static async Task AddMovieAsync(Movie movie) {
        var connection = DBController.GetConnection();
        try {
            await DBController.OpenConnectionAsync();

            string checkQuery = "SELECT COUNT(*) FROM `movies` WHERE `title` = @Title;";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Title", movie.Title);
            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

            if (exists) {
                Console.WriteLine("Movie already exists in the database.");
                return;
            }

            string query = @"
        INSERT INTO `movies` (
               `tmdb_id`,
               `poster`, 
               `large_poster`, 
               `title`, 
               `tags`, 
               `description`, 
               `release_date`, 
               `rating`, 
               `media`, 
               `duration`, 
               `download_link`, 
               `created_at`
        ) 
        VALUES (
               @TmdbID,
               @Poster, 
               @LargePoster, 
               @Title, 
               @Tags, 
               @Description, 
               @ReleaseDate, 
               @Rating, 
               @Media, 
               @Duration, 
               @DownloadLink, 
               @CreatedAt
        );";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TmdbID", movie.TmdbID);
            cmd.Parameters.AddWithValue("@Poster", movie.Poster);
            cmd.Parameters.AddWithValue("@LargePoster", movie.LargePoster);
            cmd.Parameters.AddWithValue("@Title", movie.Title);
            cmd.Parameters.AddWithValue("@Tags", string.Join(",", movie.Tags));
            cmd.Parameters.AddWithValue("@Description", movie.Description);
            cmd.Parameters.AddWithValue("@ReleaseDate", movie.ReleaseDate); // Corrected to ReleaseDate
            cmd.Parameters.AddWithValue("@Rating", movie.Rating);
            cmd.Parameters.AddWithValue("@Media", string.Join(",", movie.Media));
            cmd.Parameters.AddWithValue("@Duration", movie.Duration);
            cmd.Parameters.AddWithValue("@DownloadLink", movie.DownloadLink ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex) {
            Console.WriteLine($"Error while adding movie: {ex.Message}");
        }
        finally {
            await DBController.CloseConnectionAsync();
        }
    }

    public static async Task AddCommentToMovieAsync(Comment comment) {
        var connection = DBController.GetConnection();

        try {
            await DBController.OpenConnectionAsync();
            
            string query = @"
            INSERT INTO `comments` (`movie_id`,`user_name`,`comment`,`date_posted`) 
            VALUES (@MovieId,@Username,@Comment,@DatePosted);";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@MovieId", comment.MovieID);
            cmd.Parameters.AddWithValue("@Username",  comment.Username);
            cmd.Parameters.AddWithValue("@Comment",  comment.Text);
            cmd.Parameters.AddWithValue("@DatePosted", comment.DatePosted);

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex) {
            Console.WriteLine($"Error while adding comment: {ex.Message}");
        }
        finally {
            await DBController.CloseConnectionAsync();
        }
    }
    
    public static async Task<List<Comment>> GetCommentsToMovieAsync(int movieID) {
        var connection = DBController.GetConnection();
        var commetns = new List<Comment>();
        try {
            await DBController.OpenConnectionAsync();
            
            string query = "SELECT * FROM `comments` WHERE `movie_id` = @MovieId";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@MovieId", movieID);

            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) {
                commetns.Add(new Comment() {
                    MovieID = rdr.GetInt32(rdr.GetOrdinal("movie_id")),
                    Username = rdr.GetString(rdr.GetOrdinal("user_name")),
                    Text = rdr.GetString(rdr.GetOrdinal("comment")),
                    DatePosted = rdr.GetDateTime(rdr.GetOrdinal("date_posted"))
                });
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error while parsing comments: {ex.Message}");
        }
        finally {
            await DBController.CloseConnectionAsync();
        }

        return commetns;
    }

    public static async Task<List<int>> GetMoviesId() {
        var connection = DBController.GetConnection();
        var ids = new List<int>();
        try {
            await DBController.OpenConnectionAsync();
            
            string query = "SELECT id FROM `movies`";
            using var cmd = new MySqlCommand(query, connection);
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) {
                ids.Add(rdr.GetInt32(rdr.GetOrdinal("id")));
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error while fetching movie IDs: {ex.Message}");
        }
        finally {
            await DBController.CloseConnectionAsync();
        }

        return ids;
    }
}