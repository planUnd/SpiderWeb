Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
Imports SpiderWebLibrary

Public Interface GH_Graph
    ' Inherits IGH_Goo das musz noch eingebunden werden...

    ' Graph From Geometry
    Sub GraphFromDatatree(ByVal P_DATATREE As GH_Structure(Of IGH_Goo))
    Sub GraphFromDatatree(ByVal P_DATATREE As DataTree(Of System.Object))

    Function GraphFromLines(ByVal L_List As List(Of Line), _
                            ByVal undirected As Boolean, _
                            ByVal tol As Double) As List(Of Point3d)

    Sub GraphFromPoint(ByVal Vertex As List(Of Point3d), _
                       ByVal conDist As Double, _
                       ByVal tol As Double)

    Function GraphFromCells(ByVal C_List As List(Of Curve), _
                            ByVal tol As Double) As List(Of Polyline)

    Sub EdgeGraphFromMesh(ByVal M As Rhino.Geometry.Mesh)

    Function FaceGraphFromMesh(ByVal M As Rhino.Geometry.Mesh) As List(Of Point3d)
    ' Parse In
    Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer))
    Sub parseVertexDataTree(ByVal gDT As GH_Structure(Of GH_Integer))

    Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer), _
                                 ByVal gecDT As DataTree(Of Double), _
                                 Optional ByVal methode As Integer = 1)
    Sub parseVertexDataTree(ByVal gDT As GH_Structure(Of GH_Integer), _
                                 ByVal gecDT As GH_Structure(Of GH_Number), _
                                 Optional ByVal methode As Integer = 1)

    Sub parseEdgeDataTree(ByVal egDT As DataTree(Of Integer))
    Sub parseEdgeDataTree(ByVal egDT As GH_Structure(Of GH_Integer))

    Sub parseEdgeDataTree(ByVal egDT As DataTree(Of Integer), _
                               ByVal ecDT As DataTree(Of Double))
    Sub parseEdgeDataTree(ByVal egDT As GH_Structure(Of GH_Integer), _
                               ByVal ecDT As GH_Structure(Of GH_Number))

    ' Parse Out
    ReadOnly Property EG_DATATREE() As DataTree(Of Integer)

    ReadOnly Property EC_DATATREE() As DataTree(Of Double)

    ReadOnly Property EH_DATATREE() As DataTree(Of Double)

    ReadOnly Property G_DATATREE() As DataTree(Of Integer)

    ReadOnly Property GEC_DATATREE() As DataTree(Of Double)
    ReadOnly Property GEH_DATATREE() As DataTree(Of Double)

End Interface
