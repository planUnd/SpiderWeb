Imports Grasshopper.Kernel

Public Class rndSGC
    Inherits GH_Component

    Public Sub New()
        MyBase.New("randomized Sequential Graph Coloring", "rndSGC", "Randomized Sequential Graph Coloring", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("75b7f357-29e9-4613-8d35-bbe21be006f1")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", True))
    End Sub

    Private Sub setUndirected()
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("undirected", True)
            Case False
                Message = "directed"
            Case True
                Message = "undirected"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("maximum itterations", "maxItt", "Maximum Itterations", GH_ParamAccess.item, 100)
        pManager.AddIntegerParameter("aimed Colors", "aC", "Aimed Colors", GH_ParamAccess.item, 4)
        pManager.AddIntegerParameter("preset Colors", "pC", "Preset Colors [Optinal, Same Length as Elements Within the Graph]  ", GH_ParamAccess.list, -1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("VC", "VC", "Vertexes / Colors")
        pManager.Register_IntegerParam("CV", "CV", "Color / Vertex")
        ' pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.rndSGC
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim tmpG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim itt As Integer
        Dim aC As Integer
        Dim pC As New List(Of Integer)

        If (Not DA.GetDataTree("Graph", tmpG_DATATREE)) Then Return
        If (Not DA.GetData("maximum itterations", itt)) Then Return
        If (Not DA.GetData("aimed Colors", aC)) Then Return
        If (Not DA.GetDataList("preset Colors", pC)) Then Return
        If (pC.Count > 1 And pC.Count <> tmpG_DATATREE.PathCount) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input pC. When setting pC, number of branches and data (G) must be equal to number of values (pC)")
            Return
        End If

        Dim CG_LL As New List(Of List(Of Integer))
        Dim G_DATATREE As New Grasshopper.DataTree(Of Integer)
        Dim CG_DATATREE As New Grasshopper.DataTree(Of Integer)
        Dim VC_L As New List(Of Integer)

        G_DATATREE = GH_GraphBasic.ensureUndirected(tmpG_DATATREE)
        CG_LL = GH_GraphTools.rndSGC(G_DATATREE, pC, aC, itt)
        GH_GraphBasic.G_LLtoG_Datatree(CG_LL, 0, CG_DATATREE)

        Dim col As Integer = 0

        For i As Integer = 0 To G_DATATREE.BranchCount - 1
            VC_L.Add(-1)
        Next

        For Each ColorList As List(Of Integer) In CG_LL
            For Each i As Integer In ColorList
                VC_L.Item(i) = col
            Next
            col += 1
        Next

        DA.SetDataTree(0, CG_DATATREE)
        DA.SetDataList(1, VC_L)
        ' DA.SetDataList(2, Dbg)
    End Sub

End Class
