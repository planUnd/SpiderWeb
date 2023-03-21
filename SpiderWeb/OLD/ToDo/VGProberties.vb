Imports Grasshopper.Kernel

Public Class VGProberties
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Visual Graph Proberties", " VGProberties", "Returns different properties of a visual graph", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("89559bd4-cff9-46d5-a668-bba4cf950384")
        End Get
    End Property


    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "4-Neighbourhood", AddressOf set4NB, True, 4 = GetValue("NB", 4))
        Menu_AppendItem(menu, "8-Neighbourhood", AddressOf set8NB, True, 8 = GetValue("NB", 4))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("NB", 4)
            Case 4
                Message = "4-NB"
            Case 8
                Message = "8-NB"
        End Select
    End Sub

    Private Sub set4NB(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("NB", 4)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub set8NB(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("NB", 8)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("x", "x", "Number of elements in X-direction", GH_ParamAccess.item)
        ' pManager.AddIntegerParameter("y", "y", "Number of elements in Y-direction", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Visual Graph", "VG", "Indices of the grid points that can be seen from the starting points", GH_ParamAccess.tree)
        pManager.AddPointParameter("VisualGraphVertex", "VGV", "Visual graph vertex as points", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Perimeter Indices", "PI", "Perimeter Indices")
        pManager.Register_DoubleParam("Perimeter Distances", "PD", "Perimeter Distances")
        pManager.Register_IntegerParam("Perimeter Length", "PL", "Perimeter Length")
        pManager.Register_IntegerParam("Area", "A", "Isovist Area")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.VGProperties
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim VG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)
        VG_DATATREE = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)()
        Dim VGV_L As New List(Of Rhino.Geometry.Point3d)
        Dim x As Integer

        If (Not DA.GetData("x", x)) Then Return
        If (Not DA.GetDataTree("Visual Graph", VG_DATATREE)) Then Return
        If (Not DA.GetDataList("VisualGraphVertex", VGV_L)) Then Return

        If (VG_DATATREE.PathCount Mod x <> 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input x doesn't match input VG.")
            Return
        End If
        If (VG_DATATREE.PathCount <> VGV_L.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input VG doesn't match input VGV. Number of branches (VG) must be equal to number of Points (VGV) ")
            Return
        End If

        Dim NB_L As List(Of Grasshopper.Kernel.Types.GH_Integer)
        Dim PI_DATATREE As New Grasshopper.DataTree(Of Integer)
        Dim PD_DATATREE As New Grasshopper.DataTree(Of Double)
        Dim PL_L As New List(Of Integer)
        Dim A_L As New List(Of Integer)
        Dim cursor As Integer = 0
        Dim boundary As Integer

        Dim nb_typ As Integer = GetValue("NB", 4)

        For Each NB_L In VG_DATATREE.Branches
            PI_DATATREE.EnsurePath(cursor)
            PD_DATATREE.EnsurePath(cursor)

            For Each item As Grasshopper.Kernel.Types.GH_Integer In NB_L
                boundary = getNBCount(item.Value, x, NB_L, nb_typ)
                If boundary <> nb_typ Then
                    PI_DATATREE.Branch(cursor).Add(item.Value)
                    PD_DATATREE.Branch(cursor).Add(VGV_L.Item(cursor).DistanceTo(VGV_L.Item(item.Value)))
                End If
            Next
            A_L.Add(NB_L.Count)
            PL_L.Add(PI_DATATREE.Branch(cursor).Count)
            cursor += 1
        Next


        DA.SetDataTree(0, PI_DATATREE)
        DA.SetDataTree(1, PD_DATATREE)
        DA.SetDataList(2, PL_L)
        DA.SetDataList(3, A_L)
        ' BD = BoundaryD
    End Sub

    Private Function getNBCount(ByVal item As Integer, ByVal x As Integer, _
                                ByVal NB_L As List(Of Grasshopper.Kernel.Types.GH_Integer), ByVal nb As Integer _
                                ) As Integer

        Dim count As Integer = 0
        Dim row As Integer = Math.Floor(item / x)
        Dim nbArr As New List(Of Integer)
        Dim compGHInt As New GH_compInteger()

        nbArr.Add(-x)
        nbArr.Add(x)

        If Math.Floor((item - 1) / x) = row Then
            nbArr.Add(-1)
            If nb = 8 Then
                nbArr.Add(-x - 1)
                nbArr.Add(x - 1)
            End If
        End If

        If Math.Floor((item + 1) / x) = row Then
            nbArr.Add(1)
            If nb = 8 Then
                nbArr.Add(-x + 1)
                nbArr.Add(x + 1)
            End If
        End If

        For Each I As Integer In nbArr
            If NB_L.BinarySearch(New Grasshopper.Kernel.Types.GH_Integer(item + I), compGHInt) >= 0 Then
                count += 1
            End If
        Next

        Return count
    End Function
End Class
