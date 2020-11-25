namespace Bagel.BI.Olap
{
    public class DataSourceView : DatabaseObject
    {
        public DataSourceView(string name, Table[] tables) : base(name)
        {
            Tables.AddRange(tables);
        }

        private TableCollection Tables { get; } = new TableCollection();
    }
}
