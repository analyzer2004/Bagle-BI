using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class CellSet
    {
        public CellSet()
        {
            Axes.Add(new Axis("Columns"));
            Axes.Add(new Axis("Rows"));
        }

        [JsonConstructor()]
        public CellSet(int i) { }

        public double QueryTime { get; set; } = 0;
        public List<Axis> Axes { get; } = new List<Axis>();
        public List<Cell> Cells { get; } = new List<Cell>();
        public Cell this[int index]
        {
            get { return Cells[index]; }
        }
        public Cell this[int cIndex, int rIndex]
        {
            get
            {
                int index = cIndex + Axes[0].Positions.Count * rIndex;
                return Cells[index];
            }
        }
    }
}
