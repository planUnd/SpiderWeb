Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class EdgeRepresentation
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Edge Representation", "ER", "Edge Representation", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("5aff5544-643c-4bda-9f3e-6783d144ce37")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.EdgeRep
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "sum", AddressOf setSum, True, 0 = GetValue("methode", 1))
        Menu_AppendItem(menu, "average", AddressOf setAverage, True, 1 = GetValue("methode", 1))
        Menu_AppendItem(menu, "max", AddressOf setMax, True, 2 = GetValue("methode", 1))
        Menu_AppendItem(menu, "min", AddressOf setMin, True, 3 = GetValue("methode", 1))
        Menu_AppendItem(menu, "A", AddressOf setA, True, 4 = GetValue("methode", 1))
        Menu_AppendItem(menu, "B", AddressOf setB, True, 5 = GetValue("methode", 1))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("methode", 1)
            Case 0
                Message = "sum"
            Case 1
                Message = "average"
            Case 2
                Message = "max"
            Case 3
                Message = "min"
            Case 4
                Message = "A"
            Case 5
                Message = "B"
        End Select
    End Sub



    Private Sub setSum(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setAverage(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setMax(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setMin(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 3)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setA(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 4)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setB(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 5)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddNumberParameter("GraphEdgeCost", "GEC", "Graph Edge Costs", GH_ParamAccess.tree)
        pManager(1).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Edgerepräsentation of the Graph", "EG", "Edgerepräsentation of the Graph")
        pManager.Register_DoubleParam("Remaped Edge Costs", "EC", "Values Matching Edgerepräsentation Datatree Structure")
        ' pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim GEC_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = Nothing

        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return
        If (Not DA.GetDataTree("GraphEdgeCost", GEC_DATATREE)) Then
            GEC_DATATREE = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)()
        End If

        If (G_DATATREE.Branches.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Callulation might return false results. Graph limited to 65534 vertices!")
        End If

        If (G_DATATREE.PathCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GEC_DATATREE.PathCount > 0 And GEC_DATATREE.PathCount <> G_DATATREE.PathCount) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input GEC. Number of branches and data (G) must be equal to number of branches and data (GEC)")
            Return
        End If

        Dim GH_gEL As New GH_graphEdgeList()

        If GEC_DATATREE.PathCount = G_DATATREE.PathCount Then
            GH_gEL.parseVertexDataTree(G_DATATREE, GEC_DATATREE, GetValue("methode", 1))
        Else
            GH_gEL.parseVertexDataTree(G_DATATREE)
        End If

        DA.SetDataTree(0, GH_gEL.EG_DATATREE())
        DA.SetDataTree(1, GH_gEL.EC_DATATREE())
    End Sub

End Class
