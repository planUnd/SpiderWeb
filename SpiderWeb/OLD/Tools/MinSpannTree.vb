Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class MinSpannTree
    Inherits GH_Component

    Public Sub New()
        MyBase.New("MinST", "MinST", "Mininmal Spanning Tree", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("971e332d-6595-43f0-a91a-f138b6b5ebf8")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddNumberParameter("GraphEdgeCost", "GEC", "Graph Edge Costs", GH_ParamAccess.tree)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("MininmalSpanningTree", "MinST", "Mininmal Spanning Tree")
        pManager.Register_DoubleParam("GraphEdgeCost", "GEC", "Graph Edge Costs")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.minTree
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim GEC_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = Nothing

        Dim MG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)()
        Dim MSPG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)()

        If (Not DA.GetDataTree("GraphEdgeCost", GEC_DATATREE)) Then Return
        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return

        If (G_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GEC_DATATREE.PathCount > 0 And GEC_DATATREE.PathCount <> G_DATATREE.PathCount) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input GEC. Number of branches and data (G) must be equal to number of branches and data (GEC)")
            Return
        End If

        If (G_DATATREE.Branches.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Callulation might return false results. Graph limited to 65534 vertices!")
            Return
        End If

        Dim EdgeList As New List(Of graphEdge)

        For i As Integer = 0 To GEC_DATATREE.PathCount - 1
            MG_DATATREE.EnsurePath(i)
            MSPG_DATATREE.EnsurePath(i)
        Next

        GH_GraphBasic.getEdgeRepresentation(G_DATATREE, GEC_DATATREE, 4, True, EdgeList)

        EdgeList = GH_GraphTools.MinSP(EdgeList)

        GH_GraphBasic.getVertexRepresentation(EdgeList, 0, True, MG_DATATREE, MSPG_DATATREE)

        DA.SetDataTree(0, MG_DATATREE)
        DA.SetDataTree(1, MSPG_DATATREE)
    End Sub
End Class
