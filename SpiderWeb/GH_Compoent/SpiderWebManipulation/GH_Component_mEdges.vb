Option Explicit On

Imports Rhino.Geometry
Imports Rhino.Display
Imports Grasshopper
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphRepresentaions

Public Class GH_Component_mEdges
    Inherits GH_Component

    Public Sub New()
        MyBase.New("manipulateEdges", "mE", "Merge, Delete, Insert Edges", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("27caf46c-7c76-4b04-ba8c-a2c5c3a78452")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Delete Edges (dE)", AddressOf setDeleteEdges, True, 0 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Select Edges (sE)", AddressOf setSelectEdges, True, 1 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Simplify Graph (S)", AddressOf setSimplifyGraph, True, 2 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Flip Edges (fE)", AddressOf setflipEdges, True, 3 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Ensure Undirected (u)", AddressOf setEnsureUndirected, True, -1 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Flip Graph (fG)", AddressOf setflipGraph, True, -2 = GetValue("methode", 0))
    End Sub


    Private Sub setDeleteEdges(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setSelectEdges(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub


    Private Sub setSimplifyGraph(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setflipEdges(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 3)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setflipGraph(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", -2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setEnsureUndirected(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", -1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("methode", 0)
            Case -2
                Message = "fG"
            Case -1
                Message = "u"
            Case 0
                Message = "dE"
            Case 1
                Message = "sE"
            Case 2
                Message = "S"
            Case 3
                Message = "fE"
        End Select
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.manipulateEdges
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the graph", GH_ParamAccess.list)
        pManager(1).Optional = True
        pManager.AddIntegerParameter("graphEdgeIndices", "gEi", "Index of Edges to Manipulate", GH_ParamAccess.list)
        pManager(2).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph (graphEdgeList)")
        pManager.Register_PointParam("graphPoints", "GP", "3d points of the graph")
    End Sub

    ' ACHTUNG HIER FUNCTIONIERT ETWAS NOCH NICHT - Vielleicht INPUT ANPASSEN
    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim GP_L As New List(Of Point3d)
        Dim gEL As New GH_GraphEdgeList()

        Dim gEi As New List(Of Integer)

        If (Not DA.GetData("Graph", gEL)) Then Return
        DA.GetDataList("graphPoints", GP_L)
        DA.GetDataList("graphEdgeIndices", gEi)

        If (gEL.edgeCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        Dim M As Integer = GetValue("methode", 0)

        Dim tmpgEL As New GH_GraphEdgeList(gEL)
        Dim tmpGP_L As New List(Of Point3d)

        Select Case M
            Case -1
                tmpgEL.ensureUndirected()
                tmpGP_L.AddRange(GP_L)
            Case 0 ' delete Edges
                tmpgEL.delete(gEi)
                tmpGP_L.AddRange(GP_L)
            Case 1 ' select Edges
                tmpgEL.subGraph(gEi)
                tmpGP_L.AddRange(GP_L)
            Case 2 'simplify Graph
                Dim map As List(Of Integer) = tmpgEL.simplify()
                If GP_L.Count > 0 Then
                    For Each i In map
                        tmpGP_L.Add(GP_L.Item(i))
                    Next
                End If
            Case 3
                tmpgEL.flipEdges(gEi)
            Case -2
                tmpgEL.flipGraph()
        End Select

        DA.SetData(0, tmpgEL)
        DA.SetDataList(1, tmpGP_L)
    End Sub
End Class
