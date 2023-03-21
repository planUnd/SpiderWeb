Imports Grasshopper.Kernel

Public Class NBFilters
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Neighbourhood Filter", "NBF", "Neighbourhood Filter", "Extra", "SpiderWebFilter")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("a7d3b7f6-46e1-46f4-ac07-2c5fd093ec49")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "median", AddressOf setMedian, True, 0 = GetValue("filter", 1))
        Menu_AppendItem(menu, "average", AddressOf setAverage, True, 1 = GetValue("filter", 1))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("filter", 1)
            Case 0
                Message = "median"
            Case 1
                Message = "average"
        End Select
    End Sub

    Private Sub setMedian(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("filterUndoEvent")
        SetValue("filter", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setAverage(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("filterUndoEvent")
        SetValue("filter", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddNumberParameter("Values", "V", "Values to Filter", GH_ParamAccess.list)
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.NBFilter
        End Get
    End Property

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_DoubleParam("Filtered Values", "FV", "Filtered Values")
    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim Values As New List(Of Double)
        Dim retValues As New List(Of Double)
        Dim F As Integer

        If (Not DA.GetDataList("Values", Values)) Then Return
        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return

        F = GetValue("filter", 1)

        If (Values.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input V(Values)")
            Return
        End If
        If (G_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        If (G_DATATREE.Branches.Count <> Values.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input V. Number of branches (G) must be equal to number of Values (V) ")
            Return
        End If

        Select Case F
            Case 0
                GH_GraphFilter.Median(G_DATATREE, Values, retValues)
            Case 1
                GH_GraphFilter.Average(G_DATATREE, Values, retValues)
            Case Else
                retValues = Values
        End Select

        DA.SetDataList(0, retValues)
    End Sub
End Class
