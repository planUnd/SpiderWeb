Option Explicit On

Imports Grasshopper
Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphRepresentaions
Imports SpiderWebLibrary.graphTools
Imports SpiderWebLibrary.graphElements

Public Class GH_Component_PWP
    Inherits GH_Component

    Public Sub New()
        MyBase.New("ShortestPathBetweenPoints", "SPBP", "Callculates the shortest path between points", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("9c1d9eff-537e-4227-9713-01816c8d5c0d")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Find All (A)", AddressOf setFindAll, True, 0 = GetValue("methode", 1))
        Menu_AppendItem(menu, "Find First (F)", AddressOf setFindFirst, True, 1 = GetValue("methode", 1))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("methode", 1)
            Case 0
                Message = "A"
            Case 1
                Message = "F"
        End Select
    End Sub

    Private Sub setFindAll(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setFindFirst(ByVal sender As Object, ByVal e As EventArgs)
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
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddIntegerParameter("StartingPoint", "SP", "Index of Starting Points", GH_ParamAccess.list)
        pManager.AddIntegerParameter("EndPoint", "EP", "Index of End Points", GH_ParamAccess.list)
        pManager.AddNumberParameter("Weight", "W", "Weight of Path", GH_ParamAccess.list)
        pManager.Param(3).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("graphEdgeIndex", "gEi", "Indices of the used graphEdges")
        pManager.Register_DoubleParam("graphEdgeWeight", "gEw", "Weight of the used graphEdges (how often an edge was used)")
        pManager.Register_IntegerParam("graphVertexIndex", "gVi", "Indices of the used graphVertices")
        pManager.Register_DoubleParam("graphVertexWeight", "gVw", "Weight of the used graphVertices (how often a vertex was used)")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.CBP
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        Dim SP, EP As New List(Of Integer)
        Dim W As New List(Of Double)

        If (Not DA.GetData("Graph", gVL)) Then Return

        If (Not DA.GetDataList("StartingPoint", SP)) Then Return
        If (Not DA.GetDataList("EndPoint", EP)) Then Return
        DA.GetDataList("Weight", W)

        If (W.Count <> SP.Count) Then
            W = New List(Of Double)
            For i As Integer = 0 To SP.Count - 1
                W.Add(1)
            Next
        End If

        If (gVL.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (EP.Count <> SP.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input From doesn't match Input To")
            Return
        End If

        If gVL.negativeCost(GH_Component.DocumentTolerance()) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "graphEdges with costs <= 0 found. Please check the costs / document tolerance")
            Return
        End If

        Dim gVi As New List(Of Integer)
        Dim gVw As New List(Of Double)

        Dim gEi As New List(Of Integer)
        Dim gEw As New List(Of Double)

        Dim SSSP As SSSP
        Dim SSSPList As New List(Of SSSP)
        Dim SpSet As New List(Of Integer)
        Dim cursor, index, old As Integer

        Dim gEl As New graphEdgeList(gVL)
        Dim tmpGVL As graphVertexList
        Dim tmpGEL As graphEdgeList

        index = GetValue("methode", 1)

        'callculate all SSSP 
        For i As Integer = 0 To SP.Count - 1
            cursor = SpSet.BinarySearch(SP.Item(i))
            If cursor < 0 Then
                cursor = cursor Xor -1
                SpSet.Insert(cursor, SP.Item(i))
                SSSP = New SSSP(gVL)
                Select index
                    Case 0
                        SSSP.findALL(SP.Item(i), GH_Component.DocumentTolerance())
                    Case 1
                        SSSP.find(SP.Item(i), GH_Component.DocumentTolerance())
                End Select
                SSSPList.Insert(cursor, SSSP)
            End If
        Next

        old = SP.Item(0)
        cursor = SpSet.BinarySearch(SP.Item(0))
        SSSP = SSSPList.Item(cursor)

        For i As Integer = 0 To SP.Count - 1
            If old <> SP.Item(i) Then
                cursor = SpSet.BinarySearch(SP.Item(i))
                SSSP = SSSPList.Item(cursor)
            End If

            tmpGVL = SSSP.gVLPath(EP.Item(i))
            tmpGEL = SSSP.gELPath(EP.Item(i))

            For Each gV As graphVertex In tmpGVL.getVertexList
                If gV.outDegree <> 0 Or gV.index = EP.Item(i) Then
                    cursor = gVi.BinarySearch(gV.index)
                    If cursor < 0 Then
                        cursor = cursor Xor -1
                        gVi.Insert(cursor, gV.index)
                        gVw.Insert(cursor, W.Item(i))
                    Else
                        gVw.Item(cursor) += W.Item(i)
                    End If
                End If
            Next

            For Each gE As graphEdge In tmpGEL.getEdgeList
                index = gEl.find(gE)
                cursor = gEi.BinarySearch(index)
                If cursor < 0 Then
                    cursor = cursor Xor -1
                    gEi.Insert(cursor, index)
                    gEw.Insert(cursor, W.Item(i))
                Else
                    gEw.Item(cursor) += W.Item(i)
                End If
            Next
            old = SP.Item(i)
        Next

        DA.SetDataList(0, gEi)
        DA.SetDataList(1, gEw)

        DA.SetDataList(2, gVi)
        DA.SetDataList(3, gVw)
    End Sub

End Class