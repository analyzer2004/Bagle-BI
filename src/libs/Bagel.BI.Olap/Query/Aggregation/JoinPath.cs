using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class JoinPath
    {
        // A JoinPath represents all the relationships from fact table to specified dimension table.
        // It could be crossed many tables, for instance
        // FactSales -> (ProductKey) -> DimProduct -> (SubcategoryKey) -> DimSubcategory -> (CategoryKey) -> DimCategory
        // JoinPath(FactSales, DimCategory) = 
        // {
        //      FactSales -> (ProductKey) -> DimProduct,
        //      DimProduct -> (SubcategoryKey) -> DimSubcategory,
        //      DimSubcategory -> (CategoryKey) -> DimCategory
        // }
        // Later, it can be used in a SQL query.
        public JoinPath(Table factTable, Table dimensionTable, List<Relationship> path)
        {
            FactTable = factTable;
            DimensionTable = dimensionTable;
            Path = path;
        }
        
        public Table FactTable { get; private set; }
        public Table DimensionTable { get; private set; }
        public List<Relationship> Path { get; private set; }

        public bool Equals(Table dimensionTable, Table factTable)
        {
            return factTable == FactTable && dimensionTable == DimensionTable;
        }

        public List<Relationship> Merge(List<JoinPath> paths)
        {
            List<Relationship> list = new List<Relationship>();
            foreach(JoinPath path in paths)
            {
                foreach(Relationship rel in path.Path)
                {
                    if (!list.Contains(rel))
                        list.Add(rel);
                }
            }
            return list;
        }
    }
}
