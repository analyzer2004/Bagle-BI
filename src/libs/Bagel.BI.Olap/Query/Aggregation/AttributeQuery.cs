using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Bagel.BI.Olap
{
    public class AttributeQuery : BaseQuery
    {
        public AttributeQuery(Attribute attribute, IDbConnection connection) : base(connection)
        {
            Attribute = attribute;
        }

        public Attribute Attribute { get; set; }

        private string GetSql()
        {
            AttributeDef def = Attribute.Definition;

            // Need to rewrite for multiple key columns
            string key = def.KeyColumns.Expression;
            string name = def.NameColumn == null ? key : string.Format("[{0}]", def.NameColumn.Name);

            return string.Format(
                "select distinct {0} as [___aq_key], {1} as [___aq_name] from [{2}] order by [{3}]",
                key,
                name,
                def.KeyColumns[0].Table.Name,
                def.OrderBy == AttributeOrderBy.Key ? "___aq_key" : "___aq_name"
                );
        }

        public void QueryMembers()
        {
            IDataReader reader = null;
            try
            {
                Attribute.Clear();                
                AttributeMember all = new AttributeMember(Attribute.Definition.Dimension.AttributeAllMemberName);
                all.IsTotal = true;
                Attribute.Levels[0].Members.Add(all);

                AttributeLevel level = (AttributeLevel)Attribute.Levels[1];
                reader = ExecuteReader(GetSql());
                while (reader.Read())
                {
                    AttributeMember m = new AttributeMember(reader[0], reader.GetString(1));
                    all.Children.Add(m);
                    level.Members.Add(m);                    
                }
            }
            finally
            {
                CloseReader(reader);
            }
        }
    }
}
