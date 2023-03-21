Option Explicit On

Imports Rhino.Geometry
Imports Rhino.Display
Imports Grasshopper
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphRepresentaions

Public Class GH_Component_DisplayGraphVertex
    Inherits GH_Component

    'callculated Items
    Private Circle As List(Of Rhino.Geometry.Circle)
    Private txt As List(Of Rhino.Display.Text3d)
    Private gP As GH_graphStyle

    Public Sub New()
        MyBase.New("Display graphVertex", "DV", "Display graphVertex", "Extra", "SpiderWebDisplay")
        AddHandler Me.ObjectChanged, AddressOf PreviewChange
        Me.Circle = New List(Of Rhino.Geometry.Circle)
        Me.txt = New List(Of Rhino.Display.Text3d)
        Me.gP = New GH_graphStyle()
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("1E4C2CBF-ED4B-41A4-9EB4-50CE385E5311")
        End Get
    End Property

    Public Overrides Sub DrawViewportWires(ByVal args As Grasshopper.Kernel.IGH_PreviewArgs)
        ' MyBase.DrawViewportWires(args) '-> Disabled because nothing to Display
        If (Hidden) Then Return
        If (Circle.Count = 0) Then Return

        For Each item As Rhino.Geometry.Circle In Circle
            args.Display.DrawCircle(item, Me.gP.displayColor, 3)
        Next

        For Each item As Rhino.Display.Text3d In txt
            args.Display.Draw3dText(item, Me.gP.displayColor)
        Next
    End Sub

    Public Overrides Sub BakeGeometry(ByVal doc As Rhino.RhinoDoc, ByVal att As Rhino.DocObjects.ObjectAttributes, ByVal obj_ids As System.Collections.Generic.List(Of System.Guid))
        ' MyBase.BakeGeometry(doc, att, obj_ids)
        Me.Hidden = False
        SetValue("hidden", Me.Hidden)
        Me.ExpireSolution(True)

        att.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
        att.ObjectColor = Me.gP.displayColor

        For Each item As Circle In Circle
            doc.Objects.AddCircle(item, att)
        Next

        For Each item As Text3d In txt
            doc.Objects.AddText(item, att)
        Next

    End Sub

    Public Overrides Sub ClearData()
        MyBase.ClearData()
        Circle = New List(Of Rhino.Geometry.Circle)
        txt = New List(Of Rhino.Display.Text3d)
    End Sub

    Protected Overrides Sub BeforeSolveInstance()
        MyBase.BeforeSolveInstance()
        Circle = New List(Of Rhino.Geometry.Circle)
        txt = New List(Of Rhino.Display.Text3d)
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.DisplayGraphVertex
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)
        Menu_AppendItem(menu, "vertexOut (O)", AddressOf vIO, True, 0 = GetValue("indexTyp", 0))
        Menu_AppendItem(menu, "vertexIn (I)", AddressOf vII, True, 1 = GetValue("indexTyp", 0))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "showIndex (sI)", AddressOf showIndex, True, 0 = GetValue("show", 0))
        Menu_AppendItem(menu, "showAll (sA)", AddressOf showAll, True, 1 = GetValue("show", 0))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("indexTyp", 0)
            Case 0
                Message = "O"
            Case 1
                Message = "I"
        End Select
        Select Case GetValue("show", 0)
            Case 0
                Message = Message & "/sI"
            Case 1
                Message = Message & "/sA"
        End Select
    End Sub

    Private Sub showIndex(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("setShow")
        SetValue("show", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub showAll(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("setShow")
        SetValue("show", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub vIO(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("setIndexTyp")
        SetValue("indexTyp", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub vII(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("setIndexTyp")
        SetValue("indexTyp", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub PreviewChange(ByVal sender As Object, ByVal e As GH_ObjectChangedEventArgs)
        If (e.Type = GH_ObjectEventType.Preview) Then
            SetValue("hidden", Me.Hidden)
            Me.ExpireSolution(True)
            Me.ExpirePreview(True)
        End If
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the graph", GH_ParamAccess.list)
        pManager.Param(1).Optional = True
        pManager.AddIntegerParameter("graphVertexIndex", "gVi", "Index of graphVertex to display", GH_ParamAccess.list)
        pManager.Param(2).Optional = True
        pManager.AddTextParameter("Value", "V", "Value to Display", GH_ParamAccess.list)
        pManager.Param(3).Optional = True
        pManager.AddParameter(New GH_graphStyleParam(), "graphStyle", "gS", "Style for Graph Drawing", GH_ParamAccess.item)
        pManager.Param(4).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Degree", "D", "Degree")
        pManager.Register_IntegerParam("Neighbours", "NB", "Neighbours of Vertex")
        pManager.Register_DoubleParam("Cost", "C", "Edge cost")
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Me.Hidden = GetValue("hidden", True)

        Dim GP_L As New List(Of Rhino.Geometry.Point3d)

        Dim G As New GH_GraphVertexList()
        Dim gVL As New List(Of graphVertex)
        Dim gV As graphVertex
        Dim gVi As New List(Of Integer)
        Dim Val As New List(Of String)
        Dim tmpVal As String = ""

        If (Not DA.GetData("Graph", G)) Then Return
        DA.GetDataList("graphPoints", GP_L)
        If (Not DA.GetData("graphStyle", gP)) Then
            gP = New GH_graphStyle()
        End If

        If (G.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GP_L.Count = 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "No valid data collected in input GP(Graph Points). No output at E(Edge).")
        End If
        If (G.vertexCount <> GP_L.Count And GP_L.Count > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Input G doesn't match input GP.")
        End If
        If (G.vertexCount > GP_L.Count And GP_L.Count > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "To less GP to represent G")
            Return
        End If
        If (Not DA.GetDataList("graphVertexIndex", gVi) And GetValue("show", 0) = 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No graphVertexIndex defined.")
            Return
        End If
        DA.GetDataList("Value", Val)

        If GetValue("show", 0) = 0 Then
            For Each i As Integer In gVi
                If (Not G.Item(i).isValid) Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Index to high.")
                Else
                    gVL.Add(G.Item(i))
                End If
            Next
        ElseIf GetValue("show", 0) = 1 Then
            gVL = G.getVertexList
        End If

        For i As Integer = 0 To gVL.Count - 1
            gV = gVL.Item(i)
            Dim gDD As New DataTree(Of Integer)
            gDD.EnsurePath(gV.index)
            Dim gDT As New DataTree(Of Integer)
            gDT.EnsurePath(gV.index)
            Dim gCDT As New DataTree(Of Double)
            gCDT.EnsurePath(gV.index)

            Select Case GetValue("indexTyp", 0)
                Case 0
                    tmpVal = gV.outDegree

                    gDD.Branch(0).Add(gV.outDegree)
                    DA.SetDataTree(0, gDD)

                    gDT.Branch(0).AddRange(gV.neighbours)
                    DA.SetDataTree(1, gDT)

                    gCDT.Branch(0).AddRange(gV.cost)
                    DA.SetDataTree(2, gCDT)

                Case 1
                    Dim gE_L As List(Of graphEdge) = G.getInEdges(gV.index)
                    tmpVal = gE_L.Count

                    Dim nb As New List(Of Integer)
                    Dim cost As New List(Of Double)
                    For Each gE As graphEdge In gE_L
                        nb.Add(gE.A)
                        cost.Add(gE.Cost)
                    Next

                    gDD.Branch(0).Add(gE_L.Count)
                    DA.SetDataTree(0, gDD)

                    gDT.Branch(0).AddRange(nb)
                    DA.SetDataTree(1, gDT)

                    gCDT.Branch(0).AddRange(cost)
                    DA.SetDataTree(2, gCDT)
            End Select

            If Not GetValue("hidden", Me.Hidden) And GP_L.Count <> 0 Then
                If Val.Count > i Then
                    InsertNode(gV.index, Me.gP.txtSize, GP_L.Item(gV.index), Val.Item(i))
                ElseIf GetValue("indexTyp", 0) = 0 Then
                    InsertNode(gV.index, Me.gP.txtSize, GP_L.Item(gV.index), tmpVal)
                ElseIf GetValue("indexTyp", 0) = 1 Then
                    InsertNode(gV.index, Me.gP.txtSize, GP_L.Item(gV.index), tmpVal)
                End If
            End If
        Next

    End Sub

    Private Sub InsertNode(ByVal index As Integer, _
                       ByVal S As Double, _
                       ByVal P As Point3d, _
                       ByVal Val As String)


        Dim cX As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraX
        Dim cY As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraY
        Dim TextPlane As Plane = New Plane(Point3d.Origin, cX, cY)

        Dim tmpC As New Circle(P, S * 0.4)
        TextPlane.Origin = P
        tmpC.Plane = TextPlane
        Circle.Add(tmpC)

        TextPlane.Origin = Point3d.Add(P, Vector3d.Add(Vector3d.Multiply(S * 0.4, cX), Vector3d.Multiply(S * 0.4, cY)))
        txt.Add(New Text3d(index, TextPlane, S))

        TextPlane.Origin = Point3d.Add(P, Vector3d.Add(Vector3d.Multiply(S * 0.4, cX), Vector3d.Multiply(S * -0.4 - S * 0.66, cY)))
        txt.Add(New Text3d(Val, TextPlane, S * 0.66))
    End Sub

End Class
