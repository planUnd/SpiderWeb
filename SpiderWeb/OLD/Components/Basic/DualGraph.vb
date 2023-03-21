Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class DualGraph
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

        Menu_AppendItem(menu, "dual angular graph", AddressOf DualAngularGraph, True, 0 = GetValue("methode", 0))
        Menu_AppendItem(menu, "dual angular vertex graph", AddressOf DualAngularVertexGraph, True, 1 = GetValue("methode", 0))
        Menu_AppendItem(menu, "dual topologic graph", AddressOf DualTypologicGraph, True, 2 = GetValue("methode", 0))
        Menu_AppendItem(menu, "dual topologic vertex graph", AddressOf DualTypologicVertexGraph, True, 3 = GetValue("methode", 0))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "VertexList", AddressOf setVL, True, False = GetValue("representation", False))
        Menu_AppendItem(menu, "EdgeList", AddressOf setEL, True, True = GetValue("representation", False))
    End Sub



    Private Sub SetMessage()
        Select Case GetValue("representation", False)
            Case False
                Message = "VL/"
            Case True
                Message = "EL/"
        End Select

        Select Case GetValue("methode", 0)
            Case 0
                Message = Message & "dual angular graph"
            Case 1
                Message = Message & "dual angular vertex graph"
            Case 2
                Message = Message & "dual topologic graph"
            Case 3
                Message = Message & "dual topologic vertex graph"
        End Select
    End Sub

    Private Sub setVL()
        SetValue("representation", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setEL()
        SetValue("representation", True)
        SetMessage()
        ExpireSolution(True)
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
        pManager.AddGenericParameter("Graph", "G", "Graph representation", GH_ParamAccess.item)
        pManager.AddPointParameter("GraphVertices", "GV", "Graph Vertices as Points", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Dualgraph representation")
        pManager.Register_PointParam("dualGraphVertex", "dGV", "Dual Graph Vertex as Points")
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim GV_L As New List(Of Rhino.Geometry.Point3d)
        Dim rep As Boolean = GetValue("representation", False)

        If (Not DA.GetDataList("GraphVertices", GV_L)) Then Return
        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return

        If (G_DATATREE.Branches.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Callulation might return false results. Graph limited to 65534 vertices!")
        End If

        If (G_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GV_L.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input GV(GraphVertices)")
            Return
        End If
        If (G_DATATREE.PathCount <> GV_L.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input GV. Number of branches (G) must be equal to number of Points (GV) ")
            Return
        End If

        Dim M As Integer = GetValue("methode", 0)

        Dim gS As Integer = 0
        If (M Mod 2) = 1 Then
            gS = G_DATATREE.PathCount
        End If

        Dim dG_DataTree As New Grasshopper.DataTree(Of Integer)
        Dim dGEC_DataTree As New Grasshopper.DataTree(Of Double)
        Dim dGV_L As New List(Of Rhino.Geometry.Point3d)

        Dim Edge_L As New List(Of graphEdge)
        Dim dEdge_L As New List(Of graphEdge)

        GH_GraphBasic.getEdgeRepresentation(G_DATATREE, False, Edge_L)

        Dim tol As Double = GH_Component.DocumentTolerance()

        Select Case M
            Case 0
                dEdge_L = GH_GraphBasic.dualAngularGraph(Edge_L, G_DATATREE, GV_L, tol, gS, (M Mod 2) = 1)
            Case 1
                dEdge_L = GH_GraphBasic.dualAngularGraph(Edge_L, G_DATATREE, GV_L, tol, gS, (M Mod 2) = 1)
            Case 2
                dEdge_L = GH_GraphBasic.dualTypologicGraph(Edge_L, G_DATATREE, GV_L, tol, gS, (M Mod 2) = 1)
            Case 3
                dEdge_L = GH_GraphBasic.dualTypologicGraph(Edge_L, G_DATATREE, GV_L, tol, gS, (M Mod 2) = 1)
        End Select

        dGV_L = GH_GraphBasic.dualGraphVertex(Edge_L, GV_L, (M Mod 2) = 1)

        GH_GraphBasic.getVertexRepresentation(dEdge_L, 0, False, dG_DataTree, dGEC_DataTree)

        DA.SetDataTree(0, dG_DataTree)
        DA.SetDataTree(1, dGEC_DataTree)
        DA.SetDataList(2, dGV_L)

    End Sub
End Class
