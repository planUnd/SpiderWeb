Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class VertexRepresentation
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Vertex Representation", "VR", "Vertex Representation", "Extra", "SpiderWebManipulation")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.VertexRep
        End Get
    End Property

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("ea0d9032-5b34-4a05-9471-86919d67ecce")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("GraphEdgerepräsentation", "EG", "Edgerepräsentation of the Graph", GH_ParamAccess.tree)
        pManager.AddNumberParameter("EdgeCosts", "EC", "Costs Matching Edgerepräsentation Datatree Structure", GH_ParamAccess.tree)
        pManager(1).Optional = True
        pManager.AddIntegerParameter("VertexCount", "VC", "number of graph vertices", GH_ParamAccess.item)
        pManager(2).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Graph", "G", "Graph as Datatree")
        pManager.Register_DoubleParam("GraphEdgeCost", "GEC", "Graph Edge Costs")
        'pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim EG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim EC_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = Nothing
        Dim vertexCount As Integer

        Dim G_DATATREE As New Grasshopper.DataTree(Of Integer)
        Dim GEC_DATATREE As New Grasshopper.DataTree(Of Double)

        If (Not DA.GetDataTree("GraphEdgerepräsentation", EG_DATATREE)) Then Return
        If (Not DA.GetDataTree("EdgeCosts", EC_DATATREE)) Then
            EC_DATATREE = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)
        End If
        If (DA.GetData("VertexCount", vertexCount)) Then
            vertexCount = vertexCount - 1
        Else
            vertexCount = 0
        End If

        If (G_DATATREE.Branches.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Callulation might return false results. Graph limited to 65534 vertices!")
        End If

        If (EG_DATATREE.PathCount <= 0) Then Return
        If (EC_DATATREE.PathCount > 0 And EG_DATATREE.PathCount <> EC_DATATREE.PathCount) Then Return

        'Dim DebuggStr As New List(Of String)
        Dim EdgeList As New List(Of graphEdge)
        GH_GraphBasic.EdgeListFromEG(EG_DATATREE, EC_DATATREE, EdgeList)
        GH_GraphBasic.getVertexRepresentation(EdgeList, vertexCount, False, G_DATATREE, GEC_DATATREE)

        DA.SetDataTree(0, G_DATATREE)
        DA.SetDataTree(1, GEC_DATATREE)
        ' DA.SetDataList(2, DebuggStr)
    End Sub
End Class
