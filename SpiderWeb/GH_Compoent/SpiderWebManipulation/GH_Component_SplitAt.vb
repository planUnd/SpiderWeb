Option Explicit On

Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports GH_IO.Types
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphRepresentaions

Public Class GH_Component_Split
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Split", "S", "Split a graph at a given Distance", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("76253672-636C-44FA-9445-AF433CB1620F")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.CutAt
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphEdgeListParam(), "Graph", "G", "Graph (graphEdgeList)", GH_ParamAccess.item)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the graph", GH_ParamAccess.list)
        pManager.AddNumberParameter("Values", "V", "graphVertex Values", GH_ParamAccess.list)
        pManager.AddNumberParameter("v", "v", "cut Graph at value v", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("GraphSmaller", "G<", "Graph Larger (graphEdgeList)")
        pManager.Register_GenericParam("GraphLarger", "<G", "Graph Larger (graphEdgeList)")
        pManager.Register_PointParam("graphPoints", "GP", "3d points of the graph")
        pManager.Register_DoubleParam("t", "t", "parameter of graphEdges t of G")
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim gEL As New GH_GraphEdgeList()
        Dim GP_L As New List(Of Point3d)
        Dim Values As New List(Of Double)
        Dim v As Double

        If (Not DA.GetData("Graph", gEL)) Then Return
        If (Not DA.GetDataList("graphPoints", GP_L)) Then Return
        If (Not DA.GetDataList("Values", Values)) Then Return
        If (Not DA.GetData("v", v)) Then Return

        If (gEL.vertexCount > GP_L.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "To Less GP in the list.")
            Return
        End If
        If (gEL.vertexCount <> GP_L.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Input G doesn't match input GP.")
        End If
        If (gEL.vertexCount <> Values.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input V. Number of graphVertex (G) must be equal to number of Values (V) ")
            Return
        End If

        ' return variables
        Dim t As List(Of Double) = gEL.tAt(v, Values)
        Dim gEL_New() As Graph = gEL.split_tAt(v, Values)
        Dim GP_New As New List(Of Point3d)

        GP_New.AddRange(GP_L)

        Dim gE As graphEdge
        Dim tmpT As Double
        Dim tmpVec As Vector3d

        For i As Integer = 0 To gEL.edgeCount - 1
            gE = gEL.Item(i)
            tmpT = t.Item(i)
            If 0 < tmpT And tmpT < 1 Then
                tmpVec = Vector3d.Subtract(GP_L.Item(gE.B), GP_L.Item(gE.A))
                tmpVec = Vector3d.Multiply(tmpT, tmpVec)
                GP_New.Add(Point3d.Add(GP_L.Item(gE.A), tmpVec))
            End If
        Next

        ' rework...
        DA.SetData(0, gEL_New(0))
        DA.SetData(1, gEL_New(1))
        DA.SetDataList(2, GP_New)
        DA.SetDataList(3, t)
    End Sub
End Class
