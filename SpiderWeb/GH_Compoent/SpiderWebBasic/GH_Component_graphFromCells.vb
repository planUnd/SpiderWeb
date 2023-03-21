Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_graphFromCells
    Inherits GH_Component

    Public Sub New()
        MyBase.New("graphFromCells", "gFC", "Create a Graph From a Set of Ajoining Cells", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("16d90f79-2dd7-48b7-aef2-7bf3731aa895")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "undirected (u)", AddressOf setUndirected, True, True = GetValue("undirected", True))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "Distance Cost (dC)", AddressOf setDistCost, True, False = GetValue("cost", False))
        Menu_AppendItem(menu, "Edge Cost (eC)", AddressOf setEdgeCost, True, True = GetValue("cost", False))
    End Sub

    Private Sub setUndirected()
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setDistCost()
        SetValue("cost", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setEdgeCost()
        SetValue("cost", True)
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
                Message = "d"
            Case True
                Message = "u"
        End Select
        Select Case GetValue("cost", False)
            Case True
                Message = Message & " / eC"
            Case False
                Message = Message & " / dC"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddCurveParameter("Cells", "C", "Cells to Build the Graph From", GH_ParamAccess.list)
        pManager.AddNumberParameter("minDist", "mD", "Minimal Overlap Distance", GH_ParamAccess.item, 0)
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
        Dim md As Double = 0

        DA.GetData("minDist", md)

        If (Not DA.GetDataList("Cells", C_List)) Then Return
        If (C_List.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input C(Cells)")
            Return
        End If

        Dim CP_List_S As New List(Of Rhino.Geometry.Polyline)

        Dim gVL As New GH_GraphVertexList()
        CP_List_S = gVL.GraphFromCells(C_List, GH_Component.DocumentTolerance(), md, GetValue("cost", True))

        DA.SetData(0, gVL)
        DA.SetDataList(1, CP_List_S)
    End Sub

End Class
