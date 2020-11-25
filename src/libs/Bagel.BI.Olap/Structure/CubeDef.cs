using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bagel.BI.Olap
{
    public class CubeDef : DatabaseObject
    {
        public CubeDef(string name, CubeDimensionDef[] dimensions, MeasureDef[] measures) : base(name)
        {
            Dimensions = new CubeDefObjectCollection<CubeDimensionDef>(this);
            Measures = new CubeDefObjectCollection<MeasureDef>(this);

            Dimensions.AddRange(dimensions);
            Measures.AddRange(measures);
        }

        public CubeDimensionDef this[string name]
        {
            get
            {
                return Dimensions[name];
            }
        }

        // Need to add constraint - all dimensions and measures must come from the same datasource
        public CubeDefObjectCollection<CubeDimensionDef> Dimensions { get; }
        public CubeDefObjectCollection<MeasureDef> Measures { get; }
        public bool IsReady
        {
            get { return Measures.Count > 0 && Dimensions.Count > 0; }
        }

        internal DataSource GetDataSource()
        {
            if (Measures.Count > 0)
                return Measures[0].Source.Table.DataSource;
            else
                throw new Exception("Unable to get DataSource.");
        }

        private void CloseConnection(IDbConnection cn)
        {
            if (cn != null && cn.State != ConnectionState.Closed) cn.Close();
        }


        public QueryContext Prepare()
        {
            if (!IsReady) throw new CubeNotReadyException(this);

            DataSource ds = GetDataSource();
            IDbConnection cn = ds.GetConnection();
            try
            {
                Cube cube = Runtime.Cubes[Name];
                if (cube == null)
                {
                    cube = new Cube(Name);
                    Runtime.Cubes.Add(cube);
                }

                foreach (MeasureDef md in Measures)
                {
                    if (cube.Measures[md.Name] == null)
                    {
                        Measure m = new Measure(md, md.Name);
                        cube.Measures.Add(m);
                    }
                }


                cn.Open();
                Dimensions.ForEach(cdd =>
                {
                    //cdd: CubeDimensionDef
                    //ad: AttributeDef
                    Dimension dim = Runtime.Dimensions[cdd.Name];
                    if (dim == null)
                    {
                        dim = new Dimension(cdd.Name);
                        Runtime.Dimensions.Add(dim);
                        cube.Dimensions.Add(dim);
                    }

                    cdd.Dimension.Attributes.ForEach(ad =>
                    {
                        Attribute attr = dim.Attributes[ad.Name];
                        if (attr == null)
                        {
                            attr = new Attribute(ad, ad.Name);
                            dim.Attributes.Add(attr);

                            AttributeQuery aq = new AttributeQuery(attr, cn);
                            aq.QueryMembers();
                        }
                    });

                    cdd.Dimension.Hierarchies.ForEach(ah =>
                    {
                        Hierarchy hier = dim.Hierarchies[ah.Name];
                        if (hier == null)
                        {
                            hier = new Hierarchy(ah, ah.Name);
                            dim.Hierarchies.Add(hier);

                            HierarchyQuery hq = new HierarchyQuery(hier, cn);
                            hq.QueryMembers();
                        }
                    });
                });

                return new QueryContext(cube, cn);
            }
            catch (Exception e)
            {
                CloseConnection(cn);
                throw e;
            }
        }
    }
}
