using System;
using System.Collections.Generic;

namespace Bagel.BI.Olap
{
    public class Table : NamedObject
    {
        public Table(DataSource dataSource, string name, Column[] columns, bool isFact = false) : base(name)
        {
            Columns = new ColumnCollection(this);

            IsFact = isFact;
            DataSource = dataSource;
            Columns.AddRange(columns);
        }

        public DataSource DataSource { get; set; }
        public ColumnCollection Columns { get; }
        // Alias - 正式版本不能這樣做, 會有multithreading的問題, Instance要切開
        public string Alias { get; set; }
        public bool IsFact { get; set; } = false;        
        public List<Relationship> Relationships { get; } = new List<Relationship>();
        public Column this[object index]
        {
            get
            {
                if (index is int)
                {
                    return Columns[(int)index];
                }
                else if (index is string)
                {
                    string name = (string)index;
                    foreach(Column c in Columns)
                    {
                        if (string.Compare(name, c.Name, true) == 0)
                            return c;
                    }
                }
                throw new Exception(string.Format("{0} not found or out of range.", index));
            }
        }

        // Both tables have the same key column name
        public Relationship AddRelationship(Table dest, string column)
        {
            return AddRelationship(dest, column, column);
        }

        public Relationship AddRelationship(Table dest, string column, string destColumn)
        {
            Relationship rel = new Relationship(this, dest, this[column], dest[destColumn]);
            Relationships.Add(rel);
            return rel;
        }

        public List<Table> GetDestinations()
        {
            List<Table> list = new List<Table>();
            foreach (Relationship rel in Relationships)
                if (!list.Contains(rel.Destination))
                    list.Add(rel.Destination);
            return list;
        }

        public Relationship FindRelationshipByDestination(Table dest)
        {
            foreach (Relationship rel in Relationships)
                if (rel.Destination == dest)
                    return rel;
            return null;
        }
    }
}
