Option Explicit On

Imports Rhino.Geometry
Imports Rhino.Display
Imports Grasshopper
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphRepresentaions

Public Class GH_Component_findEdge
    Inherits GH_Component

    Public Sub New()
        MyBase.New("find Edges", "fE", "get the index of a graphEdge within a Graph", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("6C1FD4E6-5F2F-43A1-89FD-E4310BC9628E")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.findeEdge
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddIntegerParameter("fromA", "A", "index of a graphVertex", GH_ParamAccess.list)
        pManager.AddIntegerParameter("toB", "B", "index of a graphVertex", GH_ParamAccess.list)

    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("graphEdge index", "gEi", "indices of graphEdges including the given parameter")
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim gEL As New GH_GraphEdgeList()
        Dim A As New List(Of Integer)
        Dim B As New List(Of Integer)
        If (Not DA.GetData("Graph", gEL)) Then Return
        If (Not DA.GetDataList("fromA", A)) Then Return
        If (Not DA.GetDataList("toB", B)) Then Return

        If (gEL.edgeCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        If (A.Count <> B.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input A doesn't match Input B")
            Return
        End If

        Dim gE As graphEdge
        Dim gEi As New List(Of Integer)

        For i As Integer = 0 To A.Count - 1
            gE = New graphEdge(A.Item(i), B.Item(i))
            gEi.Add(gEL.find(gE))
        Next

        DA.SetDataList(0, gEi)

    End Sub
End Class
