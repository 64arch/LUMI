
namespace LUMI.Models;

public class Movie {
    public int ID { get; set; }
    public string Poster { get; set; }
    public string LargePoster { get; set; }
    public string Title { get; set; }
    public List<string> Tags { get; set; }
    public string Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public float Rating { get; set; }
    public List<string> Media { get; set; }
    public string Duration { get; set; }
    public string DownloadLink { get; set; }
    public DateTime CreatedAt { get; set; }
}
