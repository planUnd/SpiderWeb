Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class manipulateVertices
    Inherits GH_Component

    Public Sub New()
        MyBase.New("manipulateVertices", "mV", "Merge, Delete Insert Vertices", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("465781ff-6bff-4173-af59-a223bb7110d0")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "delete Edges of Vertices", AddressOf setDeleteVertices, True, 0 = GetValue("methode", 0))
        Menu_AppendItem(menu, "merge Edges of Vertices", AddressOf setMergeEdgesofVertices, True, 1 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Simplify Graph", AddressOf setSimplifyGraph, True, 2 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Ensure Undirected", AddressOf setEnsureUndirected, True, -1 = GetValue("methode", 0))
    End Sub

    Private Sub setDeleteVertices(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setMergeEdgesofVertices(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub


    Private Sub setSimplifyGraph(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setEnsureUndirected(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", -1)
        SetMessage()
        ExpireSolution(True)
    End Sub


    Private Sub SetMessage()
        Select Case GetValue("methode", 0)
            Case -1
                Message = "Ensure Undirected"
            Case 0
                Message = "Delete Vertices"
            Case 1
                Message = "Merge Edges of Vertices"
            Case 2
                Message = "Simplify Graph"
        End Select

    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.manipulateVertices

        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddNumberParameter("GraphEdgeCosts", "GEC", "Values Matching Graph Datatree Structure", GH_ParamAccess.tree)
        pManager(1).Optional = True
        pManager.AddPointParameter("GraphVertices", "GV", "Graph Vertices as Points", GH_ParamAccess.list)
        pManager(2).Optional = True
        pManager.AddIntegerParameter("Vertex Indices", "VI", "Index of Vertices to Manipulate", GH_ParamAccess.list)
        pManager(3).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Graph", "G", "Graph")
        pManager.Register_DoubleParam("GraphEdgeCosts", "GEC", "Graph Edge Costs")
        pManager.Register_PointParam("GraphVertices", "GV", "Graph Vertices as Points")
    End Sub


    ' ACHTUNG HIER FUNCTIONIERT ETWAS NOCH NICHT - Vielleicht INPUT ANPASSEN
    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim GEC_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = Nothing
        Dim GV_L As New List(Of Rhino.Geometry.Point3d)
        Dim index_L As New List(Of Integer)

        If (Not DA.GetDataList("Vertices", index_L)) Then
            index_L = New List(Of Integer)
        End If

        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return
        If (Not DA.GetDataTree("GraphEdgeCosts", GEC_DATATREE)) _
            Or GEC_DATATREE.DataCount <> G_DATATREE.DataCount _
            Or GEC_DATATREE.PathCount <> G_DATATREE.PathCount Then
            GEC_DATATREE = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)()
        End If
        If Not DA.GetDataList("GraphVertices", GV_L) _
            Or GV_L.Count <> G_DATATREE.PathCount Then
            GV_L = New List(Of Rhino.Geometry.Point3d)
        End If

        If (G_DATATREE.Branches.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Callulation might return false results. Graph limited to 65534 vertices!")
        End If

        Dim tmpG_DATATREE As New Grasshopper.DataTree(Of Integer)
        Dim tmpGEC_DATATREE As New Grasshopper.DataTree(Of Double)
        Dim Edge_L As New List(Of graphEdge)
        Dim M As Integer = GetValue("methode", 0)

        GH_GraphBasic.getEdgeRepresentation(G_DATATREE, GEC_DATATREE, 4, False, Edge_L)

        Select Case M
            Case 0
                GH_GraphManipulation.DeleteVertices(index_L, Edge_L)
            Case 1
                GH_GraphManipulation.mergeEdgesofVertices(index_L, Edge_L)
            Case 2
                GH_GraphManipulation.simplifyGraph(G_DATATREE.PathCount - 1, Edge_L, GV_L)
        End Select

        Dim gEL As New graphEdgeList(Edge_L)

        If M = 2 Then
            GH_GraphBasic.getVertexRepresentation(Edge_L, gEL.vertexCount() - 1, False, tmpG_DATATREE, tmpGEC_DATATREE)
        Else
            GH_GraphBasic.getVertexRepresentation(Edge_L, G_DATATREE.PathCount() - 1, M = -1, tmpG_DATATREE, tmpGEC_DATATREE)
        End If

        DA.SetDataTree(0, tmpG_DATATREE)
        DA.SetDataTree(1, tmpGEC_DATATREE)
        DA.SetDataList(2, GV_L)
    End Sub

End Class
