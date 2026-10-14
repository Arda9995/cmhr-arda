using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.VideoPlayers")]
public class VideoPlayer
{
    public int ID { get; set; }
    public string Name { get; set; }
}

public enum VideoPlayerEnum
{
    Youtube = 1,
    Dailymotion = 2
}