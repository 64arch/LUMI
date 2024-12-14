namespace LUMI.Models;

public class Comment {
    public int MovieID { get; set; }
    public string Username { get; set; }
    public string Text { get; set; }
    public DateTime DatePosted { get; set; }
}