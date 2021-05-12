
using System.ComponentModel.DataAnnotations;

namespace MineSweeperAPI.Models
{
    public class Game
    {
        public string Id { get; set; }

        [Display(Name ="Number of Columns")]
        [Range(3,100)]
        public int XDimension { get; set; }

        [Display(Name = "Number of Rows")]
        [Range(3, 100)]
        public int YDimension { get; set; }

        public Cell[] MineCellCollection { get; set; }

        [Display(Name = "Number of Bombs")]
        public int NumberOfBombs { get; set; }

        [Display(Name = "Game is Over")]
        public bool GameIsOver { get; set; }
    }
}
