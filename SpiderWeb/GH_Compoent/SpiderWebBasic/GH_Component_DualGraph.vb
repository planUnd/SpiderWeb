Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_DualGraph
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Dual Graph", "dualG", "Dual Graph", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("32d0b01c-5d2a-4ad9-bdab-0d86962a4027")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.dualGraph
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Dual Angular Graph (DA)", AddressOf DualAngularGraph, True, 0 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Dual Angular Vertex Graph (DAV)", AddressOf DualAngularVertexGraph, True, 1 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Dual Topologic Graph (DT)", AddressOf DualTypologicGraph, True, 2 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Dual Topologic Vertex Graph (DTV)", AddressOf DualTypologicVertexGraph, True, 3 = GetValue("methode", 0))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("methode", 0)
            Case 0
                Message = "DA"
            Case 1
                Message = "DAV"
            Case 2
                Message = "DT"
            Case 3
                Message = "DTV"
        End Select
    End Sub

    Private Sub DualAngularGraph(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub DualAngularVertexGraph(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub DualTypologicGraph(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub DualTypologicVertexGraph(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 3)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the Graph", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Dualgraph representation")
        pManager.Register_PointParam("dualGraphVertex", "dGV", "Dual Graph Vertex as Points")
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        Dim GP_L As New List(Of Rhino.Geometry.Point3d)

        If (Not DA.GetDataList("graphPoints", GP_L)) Then Return
        If (Not DA.GetData("Graph", gVL)) Then Return

        If (gVL.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GP_L.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input GV(GraphVertices)")
            Return
        End If
        If (gVL.vertexCount <> GP_L.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input GV. Number of branches (G) must be equal to number of Points (GV) ")
            Return
        End If

        Dim dGP_L As New List(Of Rhino.Geometry.Point3d)

        gVL = New GH_GraphVertexList(gVL)

        Select Case GetValue("methode", 0)
            Case 0
                dGP_L = gVL.dualGraphAngular(GP_L)
            Case 1
                dGP_L = gVL.dualVertexGraphAngular(GP_L)
            Case 2
                dGP_L = gVL.dualGraphTopological(GP_L, GH_Component.DocumentAngleTolerance)
            Case 3
                dGP_L = gVL.dualVertexGraphTopological(GP_L, GH_Component.DocumentAngleTolerance)
        End Select

        DA.SetData(0, gVL)
        DA.SetDataList(1, dGP_L)

    End Sub
End Class
