Imports Grasshopper.Kernel

Public Class BFS
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Breadth-first Search", "BFS", "Breadth-first Search", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("72fbdf5f-a5dc-424f-9451-3e1e1e36fe11")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "all", AddressOf setAll, True, 0 = GetValue("methode", 1))
        Menu_AppendItem(menu, "first", AddressOf setFirst, True, 1 = GetValue("methode", 1))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("methode", 1)
            Case 0
                Message = "all"
            Case 1
                Message = "first"
        End Select
    End Sub

    Private Sub setAll(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setFirst(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("StartPoints", "SP", "Index of Starting Points", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_DoubleParam("Cost", "C", "Cost from the Starting Points (step)")
        pManager.Register_IntegerParam("previouseGraph", "pG", "Directed Graph Pointing to the Previouse Item")
        ' pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.BFS
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim Start As New List(Of Integer)

        If (Not DA.GetDataList("StartPoints", Start)) Then Return
        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return

        If (Start.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input SP(StartPoints)")
            Return
        End If
        If (G_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        Dim methode As Byte = GetValue("methode", 1)

        Dim C_LL As New List(Of List(Of Grasshopper.Kernel.Types.GH_Number))
        Dim C_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)()

        Dim pG_LL As New List(Of List(Of Grasshopper.Kernel.Types.GH_Integer))
        Dim pG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)()

        Dim tmpSP As Integer

        For i As Integer = 0 To Start.Count - 1
            tmpSP = Start.Item(i)
            C_LL.Clear()
            pG_LL.Clear()
            GH_GraphTools.BFS(G_DATATREE, tmpSP, (methode = 0), pG_LL, C_LL)
            GH_GraphBasic.G_LLtoG_Datatree(pG_LL, tmpSP, pG_DATATREE)
            If Not GH_GraphBasic.V_LLtoV_Datatree(C_LL, tmpSP, C_DATATREE) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not all vertices are reachable from the starting points.")
            End If
        Next

        DA.SetDataTree(0, C_DATATREE)
        DA.SetDataTree(1, pG_DATATREE)
    End Sub
End Class
