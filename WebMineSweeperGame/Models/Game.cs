
namespace MineSweeperAPI.Models
{
    public class Game
    {
        public string Id { get; set; }

        public int XDimension { get; set; }

        public int YDimension { get; set; }

        public Cell[] MineCellCollection { get; set; }

        public int NumberOfBombs { get; set; }

        public bool GameIsOver { get; set; }
    }
}
