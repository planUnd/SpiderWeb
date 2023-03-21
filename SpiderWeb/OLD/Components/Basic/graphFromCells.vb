Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class graphFromCells
    Inherits GH_Component

    Public Sub New()
        MyBase.New("graphFromCells", "graphFromCells", "Create a Graph From a Set of Ajoining Cells", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("16d90f79-2dd7-48b7-aef2-7bf3731aa895")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", True))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "VertexList", AddressOf setVL, True, False = GetValue("representation", False))
        Menu_AppendItem(menu, "EdgeList", AddressOf setEL, True, True = GetValue("representation", False))
    End Sub

    Private Sub setUndirected()
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
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

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("representation", False)
            Case False
                Message = "VL/"
            Case True
                Message = "EL/"
        End Select

        Select Case GetValue("undirected", False)
            Case False
                Message = Message & "directed"
            Case True
                Message = Message & "undirected"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddCurveParameter("Cells", "C", "Cells to Build the Graph From", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of cells")
        pManager.Register_CurveParam("sortedCells", "SC", "Sorted Cells")
        ' pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphFromCells
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim C_List As New List(Of Rhino.Geometry.Curve)
        Dim rep As Boolean = GetValue("representation", False)

        If (Not DA.GetDataList("Cells", C_List)) Then Return
        If (C_List.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input C(Cells)")
            Return
        End If
        If (C_List.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Callulation might return false results. Graph limited to 65534 vertices!")
        End If

        Dim CP_List_S As New List(Of Rhino.Geometry.Polyline)

        If rep Then
            Dim GH_gEL As New GH_graphEdgeList()
            CP_List_S = GH_gEL.GraphFromCells(C_List, GH_Component.DocumentTolerance())
            DA.SetData(0, GH_gEL)
        Else
            Dim GH_gVL As New GH_graphVertexList()
            CP_List_S = GH_gVL.GraphFromCells(C_List, GH_Component.DocumentTolerance())
            DA.SetData(0, GH_gVL)
        End If

        DA.SetDataList(1, CP_List_S)
    End Sub

End Class
