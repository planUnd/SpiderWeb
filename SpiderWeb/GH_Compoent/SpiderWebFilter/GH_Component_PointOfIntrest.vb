Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_PointOfIntrest
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Points of Interest", "POI", "Points of Interest", "Extra", "SpiderWebFilter")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("c50c36f3-ccd2-4f49-8778-8d3ea4f243e1")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.POI
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddNumberParameter("Values", "V", "Values to Filter", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Point Index", "PI", "Point Index")
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        Dim Values As New List(Of Double)

        If (Not DA.GetData("Graph", gVL)) Then Return
        If (Not DA.GetDataList("Values", Values)) Then Return

        If (Values.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input V(Values)")
            Return
        End If
        If (gVL.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        If (gVL.vertexCount <> Values.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input V. Number of graphVertex (G) must be equal to number of Values (V) ")
            Return
        End If

        DA.SetDataList(0, gVL.POI(Values))
    End Sub
End Class
