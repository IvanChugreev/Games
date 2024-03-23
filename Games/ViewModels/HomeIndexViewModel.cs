using Games.Models;

namespace Games.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Game> Games { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortState CurrentSort { get; set; }
    }

    public enum SortState
    {
        NameAsc,
        NameDesc,
        ScoreAsc,
        ScoreDesc,
        NumberOfScoresAsc,
        NumberOfScoresDesc,
        DateAsc,
        DateDesc
    }
}
