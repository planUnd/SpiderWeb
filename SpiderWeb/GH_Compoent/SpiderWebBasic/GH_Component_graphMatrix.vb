Option Explicit On

Imports Grasshopper
Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphRepresentaions

Public Class GH_Component_graphMatrix
    Inherits GH_Component
    Public Sub New()
        MyBase.New("graphMatrix", "gM", "Creates a graphMatrix from a graph", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("52698A98-C2C8-4397-8B68-CA1DEDB292A2")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Laplacian Matrix (LM)", AddressOf setLaplacianMatrix, True, 1 = GetValue("matrix", 1))
        Menu_AppendItem(menu, "Relaxed Laplacian Matrix RLM)", AddressOf setRelaxedLaplacianMatrix, True, 2 = GetValue("matrix", 1))
        Menu_AppendItem(menu, "Weighted Laplacian Matrix (WLM)", AddressOf setWeightedLaplacianMatrix, True, 3 = GetValue("matrix", 1))
        Menu_AppendItem(menu, "Normalized Laplacian Matrix (NLM)", AddressOf setNormalizedLaplacianMatrix, True, 4 = GetValue("matrix", 1))
        Menu_AppendItem(menu, "Gausian Weighted Matrix (GWM)", AddressOf setGausianWeightedMatrix, True, 5 = GetValue("matrix", 1))
        Menu_AppendItem(menu, "Adjacency Matrix (AM)", AddressOf setAdjacencyMatrix, True, 6 = GetValue("matrix", 1))
    End Sub


    Private Sub SetMessage()
        Select Case GetValue("matrix", 1)
            Case 1
                Message = "LM"
            Case 2
                Message = "RLM"
            Case 3
                Message = "WLM"
            Case 4
                Message = "NLM"
            Case 5
                Message = "GWM"
            Case 6
                Message = "AM"
        End Select
    End Sub


    Private Sub setLaplacianMatrix(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("matrix", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setRelaxedLaplacianMatrix(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("matrix", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setWeightedLaplacianMatrix(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("matrix", 3)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setNormalizedLaplacianMatrix(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("matrix", 4)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setGausianWeightedMatrix(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("matrix", 5)
        SetMessage()
        ExpireSolution(True)
    End Sub


    Private Sub setAdjacencyMatrix(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("matrix", 6)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (graphVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddNumberParameter("parameter", "p", "Will only influence the relaxedLaplacianMatrix and the gausianweightedMatrix", GH_ParamAccess.item, 1.0)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("graphMatrix", "gM", "Graphmatrix representation of the graph.")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphMatrixFromGraph
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        Dim p As Double

        If (Not DA.GetData("Graph", gVL)) Then Return
        If (Not DA.GetData("parameter", p)) Then Return

        Dim gM As New GH_graphMatrix(gVL, GetValue("matrix", 1), p)

        DA.SetData(0, gM)
    End Sub
End Class
