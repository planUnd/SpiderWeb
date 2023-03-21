Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class CBP_COPY
    Inherits GH_Component

    Public Sub New()
        MyBase.New("SPBP", "ShortestPathBetweenPoints", "Callculates How Often a Graph Edge and Graph Vertex is Part of the Shortest Path Between Points", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("9c1d9eff-537e-4227-9713-01816c8d5c0d")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as datatree", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("previouseGraph", "pG", "Directed Graph Pointing to the Previouse Item", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("StartingPoints", "SP", "Index of Starting Points. Must be the Same as the Shorthest Path", GH_ParamAccess.tree)
        pManager.AddNumberParameter("StartingPointsWeight", "SPW", "Weight of Starting Points", GH_ParamAccess.tree)
        pManager(3).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_DoubleParam("Choice Between Points", "CBP", "Choice of Segments Between Points")
        pManager.Register_DoubleParam("Choice Between Points", "CBPV", "Choice of Vertex Between Points")
        ' pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.CBP
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim pG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim SP_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim SPW_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = Nothing

        ' Dim DebuggStr As New List(Of String)

        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return
        If (Not DA.GetDataTree("previouseGraph", pG_DATATREE)) Then Return
        If (Not DA.GetDataTree("StartingPoints", SP_DATATREE)) Then Return
        If (G_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (pG_DATATREE.PathCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input pG(previouseGraph)")
            Return
        End If
        If (SP_DATATREE.PathCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input SP(StartPoints)")
            Return
        End If
        If (G_DATATREE.Branches.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Callulation might return false results. Graph limited to 65534 vertices!")
            Return
        End If

        DA.GetDataTree("StartingPointsWeight", SPW_DATATREE)

        'CODE HERE
        Dim SP_L As New List(Of Grasshopper.Kernel.Types.GH_Integer)
        Dim SPW_L As New List(Of Grasshopper.Kernel.Types.GH_Number)
        Dim Edge_L As New List(Of graphEdge)


        Dim EH_Datatree As New Grasshopper.DataTree(Of Double)
        Dim VH_Datatree As New Grasshopper.DataTree(Of Double)

        GH_GraphBasic.getEdgeRepresentation(G_DATATREE, False, Edge_L)
        Dim gEL As New graphEdgeList(Edge_L)

        Dim pathLength As Integer = G_DATATREE.PathCount

        For iSP As Integer = 0 To SP_DATATREE.PathCount - 1
            If (SP_DATATREE.PathCount = SPW_DATATREE.PathCount) Then
                GH_GraphTools.CBP(gEL.DeepCopy().getEdgeList(), pG_DATATREE, SP_DATATREE.Branch(iSP), SPW_DATATREE.Branch(iSP), iSP, pathLength, EH_Datatree, VH_Datatree)
            Else
                GH_GraphTools.CBP(gEL.DeepCopy().getEdgeList(), pG_DATATREE, SP_DATATREE.Branch(iSP), 1.0, iSP, pathLength, EH_Datatree, VH_Datatree)
            End If
        Next

        DA.SetDataTree(0, EH_Datatree)
        DA.SetDataTree(1, VH_Datatree)
    End Sub

End Class