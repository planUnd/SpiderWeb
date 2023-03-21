Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry

Namespace GH_graphRepresentaions

    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : GH_Graph
    ''' 
    ''' <summary>
    ''' Interface providing methodes across different GH_graphRepresentations
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Interface GH_Graph
        ' ----------------------- Graph From Geometry ----------------------- 

        ''' <summary>
        ''' Adds the topological information contained in the Path of a DataTree to an existing Graph.
        ''' </summary>
        ''' <param name="pDT">Path information used to construct the Edges of a Graph. e.g.: {0;1} will add a graphEdge leading from graphVertex 0 to graphVertex 1.</param>
        ''' <remarks>Only considers Paths with a length of two.</remarks>
        Sub GraphFromDatatree(ByVal pDT As GH_Structure(Of IGH_Goo))

        ''' <summary>
        ''' Adds the topological information contained in the Path of a DataTree to an existing Graph.
        ''' </summary>
        ''' <param name="pDT">Path information used to construct the Edges of a Graph. e.g.: {0;1} will add a graphEdge leading from graphVertex 0 to graphVertex 1.</param>
        ''' <param name="gecDT">Costs of the edge</param>
        ''' <remarks>Only considers Paths with a length of two.</remarks>
        Sub GraphFromDatatree(ByVal pDT As GH_Structure(Of IGH_Goo), ByVal gecDT As GH_Structure(Of GH_Number))

        ''' <summary>
        ''' Adds the topological information contained in the Path of a DataTree to an existing Graph.
        ''' </summary>
        ''' <param name="pDT">Path information used to construct the Edges of a Graph. e.g.: {0;1} will add a graphEdge leading from graphVertex 0 to graphVertex 1.</param>
        ''' <remarks>Only considers Paths with a length of two.</remarks>
        Sub GraphFromDatatree(ByVal pDT As DataTree(Of System.Object))

        ''' <summary>
        ''' Adds the topological information contained in the Path of a DataTree to an existing Graph.
        ''' </summary>
        ''' <param name="pDT">Path information used to construct the Edges of a Graph. e.g.: {0;1} will add a graphEdge leading from graphVertex 0 to graphVertex 1.</param>
        ''' <param name="gecDT">Costs of the edge</param>
        ''' <remarks>Only considers Paths with a length of two.</remarks>
        Sub GraphFromDatatree(ByVal pDT As DataTree(Of System.Object), ByVal gecDT As DataTree(Of Double))

        ''' <summary>
        ''' Adds the topological information contained in the List(of Line) to an existing Graph.
        ''' </summary>
        ''' <param name="L_List"></param>
        ''' <param name="undirected">If false, one edge will be added per line. 
        ''' The drawing direction of the line is important.</param>
        ''' <param name="tol">Tolerance</param>
        ''' <returns>Returns a List of Point3d representing the world (x,y,z) position of the graphVertex.</returns>
        ''' <remarks></remarks>
        Function GraphFromLines(ByVal L_List As List(Of Line), _
                                ByVal undirected As Boolean, _
                                ByVal tol As Double) As List(Of Point3d)

        ''' <summary>
        ''' Creates a graph from a set of Lines. Adds an edge between two lines if they intersect.
        ''' </summary>
        ''' <param name="L_List"></param>
        ''' <param name="tol">Tolerance</param>
        ''' <returns>Returns a List of Point3d representing the world (x,y,z) position of the graphVertex.</returns>
        ''' <remarks></remarks>
        Function GraphFromLineIntersection(ByVal L_List As List(Of Line), _
                                ByVal tol As Double) As List(Of Point3d)

        ''' <summary>
        ''' Adds all possible connections (distance &#8804; conDist) to an existing Graph.
        ''' </summary>
        ''' <param name="Vertex"></param>
        ''' <param name="conDist"></param>
        ''' <param name="tol">Tolerance</param>
        ''' <remarks></remarks>
        Sub GraphFromPoint(ByVal Vertex As List(Of Point3d), _
                           ByVal conDist As Double, _
                           ByVal tol As Double)

        ''' <summary>
        ''' Adds a connection to an existing Graph when the outlines of two curves a overlapping parts.
        ''' </summary>
        ''' <param name="C_List">List of Curves</param>
        ''' <param name="tol"></param>
        ''' <returns>Returns the resorted outlines as list of polylines</returns>
        ''' <remarks></remarks>
        Function GraphFromCells(ByVal C_List As List(Of Curve), _
                                ByVal tol As Double, _
                                Optional ByVal minDist As Double = 0, _
                                Optional ByVal edgeCost As Boolean = False) As List(Of Polyline)

        ''' <summary>
        ''' Adds the topological information of the mesh edges to an existing Graph.
        ''' </summary>
        ''' <param name="M"></param>
        ''' <remarks></remarks>
        Sub EdgeGraphFromMesh(ByVal M As Rhino.Geometry.Mesh)

        ''' <summary>
        ''' Adds the topological information of the mesh faces adjacencies to an existing Graph.
        ''' </summary>
        ''' <param name="M"></param>
        ''' <returns>Returns the centroids of the faces as List(of Point3d)</returns>
        ''' <remarks></remarks>
        Function FaceGraphFromMesh(ByVal M As Rhino.Geometry.Mesh) As List(Of Point3d)

        ''' <summary>
        ''' Convert the Graph into its dualGraph. 
        ''' </summary>
        ''' <param name="GP_L"></param>
        ''' <returns>Returns a List of Point3d representing the new world (x,y,z) position of the graphVertex.</returns>
        ''' <remarks>The costs of each Edge is defined by the angle between the original edges.</remarks>
        Function dualGraphAngular(ByVal GP_L As List(Of Point3d)) As List(Of Point3d)

        ''' <summary>
        ''' Convert the Graph into its dualGraph and connects it to the vertices of the original Graph.
        ''' </summary>
        ''' <param name="GP_L"></param>
        ''' <returns>Returns a List of Point3d representing the new world (x,y,z) position of the graphVertex.</returns>
        ''' <remarks>The costs of Edges between the newly added vertices is defined by the angle between the original edges.
        ''' The costs from the old vertices to the new vertices are either set to infinity (ingoing) or 0 (outgoing).
        ''' </remarks>
        Function dualVertexGraphAngular(ByVal GP_L As List(Of Point3d)) As List(Of Point3d)

        ''' <summary>
        ''' Convert the Graph into its dualGraph. 
        ''' </summary>
        ''' <param name="GP_L"></param>
        ''' <returns>Returns a List of Point3d representing the new world (x,y,z) position of the graphVertex.</returns>
        ''' <remarks>The costs of each Edge is either 0 if two edges align, otherwise 1</remarks>
        Function dualGraphTopological(ByVal GP_L As List(Of Point3d), ByVal tol As Double) As List(Of Point3d)

        ''' <summary>
        ''' Convert the Graph into its dualGraph and connects it to the vertices of the original Graph.
        ''' </summary>
        ''' <param name="GP_L"></param>
        ''' <returns>Returns a List of Point3d representing the new world (x,y,z) position of the graphVertex.</returns>
        ''' <remarks>The costs of Edges between the newly added vertices is either 0 if two edges align, otherwise 1.
        ''' The costs from the old vertices to the new vertices are either set to infinity (ingoing) or 0 (outgoing).
        ''' </remarks>
        Function dualVertexGraphTopological(ByVal GP_L As List(Of Point3d), ByVal tol As Double) As List(Of Point3d)

        ' -------------------------- parse IN Vertex -------------------

        ''' <summary>
        ''' Parses a DataTree as a Vertex List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Vertex List representation</param>
        ''' <remarks>Each Branch of the DataTree (gDT) is understood as a graphVertex.
        ''' The elements within each Branch representing its neighbours.
        ''' </remarks>
        Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer))

        ''' <summary>
        ''' Parses a DataTree as a Vertex List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Vertex List representation</param>
        ''' <remarks>Each Branch of the DataTree (gDT) is understood as a graphVertex.
        ''' The elements within each Branch representing its neighbours.
        ''' </remarks>
        Sub parseVertexDataTree(ByVal gDT As GH_Structure(Of GH_Integer))

        ''' <summary>
        ''' Parses a DataTree as a Vertex List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Vertex List representation</param>
        ''' <param name="gecDT">Costs</param>
        ''' <param name="methode">0 Sum, 1 Average, 2 Max, 3 Min, 4 A, 5 B</param>
        ''' <remarks>Each Branch of the DataTree (gDT) is understood as a graphVertex.
        ''' The elements within each Branch representing its neighbours.
        ''' Depending on the choosen methode the costs are callculated from the DataTree gecDT.
        ''' The structure of gDT has to match the structure of gecDT, if not a empty Graph will be returned.
        ''' </remarks>
        Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer), _
                                     ByVal gecDT As DataTree(Of Double), _
                                     Optional ByVal methode As Integer = 4)

        ''' <summary>
        ''' Parses a DataTree as a Vertex List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Vertex List representation</param>
        ''' <param name="gecDT">Costs</param>
        ''' <param name="methode">0 Sum, 1 Average, 2 Max, 3 Min, 4 A, 5 B</param>
        ''' <remarks>Each Branch of the DataTree (gDT) is understood as a graphVertex.
        ''' The elements within each Branch representing its neighbours.
        ''' Depending on the choosen methode the costs are callculated from the DataTree gecDT.
        ''' The structure (pathCount and dataCount) of gDT has to match the structure of gecDT, if not a empty Graph will be returned.
        ''' </remarks>
        Sub parseVertexDataTree(ByVal gDT As GH_Structure(Of GH_Integer), _
                                     ByVal gecDT As GH_Structure(Of GH_Number), _
                                     Optional ByVal methode As Integer = 4)

        ' -------------------------- parse IN Edge -------------------

        ''' <summary>
        ''' Parses a DataTree as a Edge List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Edge List representation</param>
        ''' <remarks>The length of each Branch of the DataTree (egDT) has to be 2.
        ''' </remarks>
        Sub parseEdgeDataTree(ByVal gDT As DataTree(Of Integer))

        ''' <summary>
        ''' Parses a DataTree as a Edge List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Edge List representation</param>
        ''' <remarks>The length of each Branch of the DataTree (egDT) has to be 2</remarks>
        Sub parseEdgeDataTree(ByVal gDT As GH_Structure(Of GH_Integer))

        ''' <summary>
        ''' Parses a DataTree as a Edge List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Edge List representation</param>
        ''' <param name="gecDT">Costs</param>
        ''' <remarks>The length of each Branch of the DataTree (egDT) has to be 2.
        ''' The structure (pathCount) of egDT has to match the structure of ecDT, if not a empty Graph will be returned.
        ''' </remarks>
        Sub parseEdgeDataTree(ByVal gDT As DataTree(Of Integer), _
                                   ByVal gecDT As DataTree(Of Double))

        ''' <summary>
        ''' Parses a DataTree as a Edge List representation of a Graph.
        ''' </summary>
        ''' <param name="gDT">Edge List representation</param>
        ''' <param name="gecDT">Costs</param>
        ''' <remarks>The length of each Branch of the DataTree (egDT) has to be 2.
        ''' The structure (pathCount) of egDT has to match the structure of ecDT, if not a empty Graph will be returned.
        ''' </remarks>
        Sub parseEdgeDataTree(ByVal gDT As GH_Structure(Of GH_Integer), _
                                   ByVal gecDT As GH_Structure(Of GH_Number))


        ' -------------------------- parse OUT  -------------------


        ''' <summary>
        ''' Get the Graphs (Edge List representation) as DataTree
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property EG_DATATREE() As DataTree(Of Integer)

        ''' <summary>
        ''' Get the Graphs Costs (Edge List representation) as DataTree
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property EC_DATATREE() As DataTree(Of Double)


        ''' <summary>
        ''' Get the Graph (Vertex List representation) as DataTree
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property G_DATATREE() As DataTree(Of Integer)

        ''' <summary>
        ''' Get the Graphs Costs (Vertex List representation) as DataTree
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property GEC_DATATREE() As DataTree(Of Double)


    End Interface
End Namespace