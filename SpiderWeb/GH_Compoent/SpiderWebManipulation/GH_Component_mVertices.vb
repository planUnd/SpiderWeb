Option Explicit On

Imports Rhino.Geometry
Imports Rhino.Display
Imports Grasshopper
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphRepresentaions

Public Class GH_Component_mVertices
    Inherits GH_Component

    Public Sub New()
        MyBase.New("manipulateVertices", "mV", "Merge, Delete Insert Vertices", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("465781ff-6bff-4173-af59-a223bb7110d0")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Delete Edges of Vertices (dE)", AddressOf setDeleteVertices, True, 0 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Select Edges of Vertices (sE)", AddressOf setSelectEdges, True, 1 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Simplify Graph (S)", AddressOf setSimplifyGraph, True, 2 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Ensure Undirected (u)", AddressOf setEnsureUndirected, True, -1 = GetValue("methode", 0))
    End Sub

    Private Sub setDeleteVertices(ByVal sender As Object, ByVal e As EventArgs)
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

    Private Sub setEnsureUndirected(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", -1)
        SetMessage()
        ExpireSolution(True)
    End Sub


    Private Sub SetMessage()
        Select Case GetValue("methode", 0)
            Case -1
                Message = "u"
            Case 0
                Message = "dE"
            Case 1
                Message = "sE"
            Case 2
                Message = "S"
        End Select

    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.manipulateVertices

        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the graph", GH_ParamAccess.list)
        pManager(1).Optional = True
        pManager.AddIntegerParameter("graphVertexIndex", "gVi", "Index of graphVertex to display", GH_ParamAccess.list)
        pManager(2).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph (grpahVertexList)")
        pManager.Register_PointParam("graphPoints", "GP", "3d points of the graph")
        ' pManager.Register_DoubleParam("Map", "M", "Map")
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim GP_L As New List(Of Point3d)
        Dim gVL As New GH_GraphVertexList()

        Dim gVi As New List(Of Integer)

        If (Not DA.GetData("Graph", gVL)) Then Return
        DA.GetDataList("graphPoints", GP_L)
        DA.GetDataList("graphVertexIndex", gVi)

        If (gVL.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        Dim M As Integer = GetValue("methode", 0)

        Dim tmpgVL As New graphVertexList(gVL)
        Dim tmpGP_L As New List(Of Point3d)

        Select Case M
            Case -1
                tmpgVL.ensureUndirected()
                tmpGP_L.AddRange(GP_L)
            Case 0 ' deleteVertices
                tmpgVL.delete(gVi)
                tmpGP_L.AddRange(GP_L)
            Case 1 ' mergeEdgesOfVertices
                tmpgVL.subGraph(gVi)
                tmpGP_L.AddRange(GP_L)
            Case 2 'simplifyGraph
                Dim map As List(Of Integer) = tmpgVL.simplify()
                If GP_L.Count > 0 Then
                    For Each i In map
                        tmpGP_L.Add(GP_L.Item(i))
                    Next
                End If
        End Select

        DA.SetData(0, tmpgVL)
        DA.SetDataList(1, tmpGP_L)
    End Sub

End Class
