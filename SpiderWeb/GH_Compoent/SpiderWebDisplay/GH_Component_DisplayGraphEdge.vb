Option Explicit On

Imports Rhino.Geometry
Imports Rhino.Display
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphElements

Public Class GH_Component_DisplayGraphEdge
    Inherits GH_Component

    'callculated Items
    Private ArcList As List(Of Rhino.Geometry.Arc)
    Private txt As List(Of Rhino.Display.Text3d)
    Private gP As GH_graphStyle

    Public Sub New()
        MyBase.New("Display graphEdge", "DE", "Display graphEdge", "Extra", "SpiderWebDisplay")
        AddHandler Me.ObjectChanged, AddressOf PreviewChange
        txt = New List(Of Rhino.Display.Text3d)
        ArcList = New List(Of Rhino.Geometry.Arc)
        gP = New GH_graphStyle()
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("186003B5-47E1-431F-9BB8-B127EE2B43B1")
        End Get
    End Property

    Public Overrides Sub DrawViewportWires(ByVal args As Grasshopper.Kernel.IGH_PreviewArgs)
        ' MyBase.DrawViewportWires(args) '-> Disabled because nothing to Display
        If (Hidden) Then Return
        If (ArcList.Count = 0) Then Return

        Dim t As Double


        For Each item As Rhino.Geometry.Arc In ArcList
            args.Display.DrawArc(item, Me.gP.displayColor)
            t = item.ClosestParameter(item.EndPoint)
            args.Display.DrawArrowHead(item.EndPoint, item.TangentAt(t - 0.25), Me.gP.displayColor, 0, Me.gP.txtSize * 0.25)
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


        For Each item As Text3d In txt
            doc.Objects.AddText(item, att)
        Next

        att.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.EndArrowhead
        For Each item As Arc In ArcList
            doc.Objects.AddArc(item, att)
        Next
    End Sub

    Public Overrides Sub ClearData()
        MyBase.ClearData()
        txt = New List(Of Rhino.Display.Text3d)
        ArcList = New List(Of Rhino.Geometry.Arc)
    End Sub

    Protected Overrides Sub BeforeSolveInstance()
        MyBase.BeforeSolveInstance()
        txt = New List(Of Rhino.Display.Text3d)
        ArcList = New List(Of Rhino.Geometry.Arc)
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.DisplayGraphEdge
        End Get
    End Property


    Private Sub PreviewChange(ByVal sender As Object, ByVal e As GH_ObjectChangedEventArgs)
        If (e.Type = GH_ObjectEventType.Preview) Then
            SetValue("hidden", Me.Hidden)
            Me.ExpireSolution(True)
            Me.ExpirePreview(True)
        End If
    End Sub

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)
        Menu_AppendItem(menu, "showIndex (sI)", AddressOf showIndex, True, 0 = GetValue("show", 0))
        Menu_AppendItem(menu, "showAll (sA)", AddressOf showAll, True, 1 = GetValue("show", 0))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("show", 0)
            Case 0
                Message = "sI"
            Case 1
                Message = "sA"
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

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphEdgeListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the graph", GH_ParamAccess.list)
        pManager.Param(1).Optional = True
        pManager.AddIntegerParameter("graphEdgeIndex", "gEi", "Index of graphEdge to display", GH_ParamAccess.list)
        pManager.Param(2).Optional = True
        pManager.AddTextParameter("Value", "V", "Value to Display", GH_ParamAccess.list)
        pManager.Param(3).Optional = True
        pManager.AddParameter(New GH_graphStyleParam(), "graphStyle", "gS", "Style for Graph Drawing", GH_ParamAccess.item)
        pManager.Param(4).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("A", "A", "Vertex index")
        pManager.Register_IntegerParam("B", "B", "Vertex index")
        pManager.Register_DoubleParam("Cost", "C", "Edge cost")
        pManager.Register_LineParam("Edge", "E", "Edge as Line")
    End Sub



    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Me.Hidden = GetValue("hidden", True)

        Dim GP_L As New List(Of Rhino.Geometry.Point3d)
        Dim Val As New List(Of String)
        Dim tmpVal As String = ""
        Dim G As New GH_GraphEdgeList()
        Dim gE As graphEdge
        Dim gEL As New List(Of graphEdge)
        Dim gEi As New List(Of Integer)

        If (Not DA.GetData("Graph", G)) Then Return
        DA.GetDataList("graphPoints", GP_L)
        If (Not DA.GetData("graphStyle", Me.gP)) Then
            Me.gP = New GH_graphStyle()
        End If

        If (G.edgeCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GP_L.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "No valid data collected in input GP(Graph Points). No output at E(Edge).")
        End If
        If (G.vertexCount <> GP_L.Count And GP_L.Count > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Input G doesn't match input GP.")
        End If
        If (G.vertexCount > GP_L.Count And GP_L.Count > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "To less GP to represent G")
            Return
        End If

        If (Not DA.GetDataList("graphEdgeIndex", gEi) And GetValue("show", 0) = 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No graphEdgeIndex defined.")
            Return
        End If
        DA.GetDataList("Value", Val)


        If GetValue("show", 0) = 0 Then
            For Each i As Integer In gEi
                If (Not G.Item(i).isValid) Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Index to high.")
                Else
                    gEL.Add(G.Item(i))
                End If
            Next
        ElseIf GetValue("show", 0) = 1 Then
            gEL = G.getEdgeList
        End If

        Dim _A As New List(Of Integer)
        Dim _B As New List(Of Integer)
        Dim _cost As New List(Of Double)
        Dim _L As New List(Of Line)

        For i As Integer = 0 To gEL.Count - 1
            gE = gEL.Item(i)
            If Val.Count < gEL.Count Then
                tmpVal = Math.Round(gE.Cost, 2)
            Else
                tmpVal = Val.Item(i)
            End If


            _A.Add(gE.A)
            _B.Add(gE.B)
            _cost.Add(gE.Cost)


            If GP_L.Count = 0 Then Return


            _L.Add(New Line(GP_L.Item(gE.A), GP_L.Item(gE.B)))

            If Not GetValue("hidden", Me.Hidden) Then
                InsertEdge(GP_L.Item(gE.A), GP_L.Item(gE.B), Me.gP.txtSize, tmpVal)
            End If
        Next

        DA.SetDataList(0, _A)
        DA.SetDataList(1, _B)
        DA.SetDataList(2, _cost)
        DA.SetDataList(3, _L)

    End Sub

    Private Sub InsertEdge(ByVal P1 As Point3d, ByVal P2 As Point3d, ByVal S As Double, ByVal Val As String)

        Dim PM As Point3d
        Dim V As Vector3d = Point3d.Subtract(P1, P2)
        Dim cY As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraY
        Dim cX As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraX
        Dim cZ As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraZ
        Dim TextPlane As Plane = New Plane(Point3d.Origin, V, cY)

        If V.Length > 0 Then ' Normale Kante
            V = Vector3d.CrossProduct(V, cZ)
            V = Vector3d.Multiply(Math.Sqrt(Me.gP.txtSize) * 0.05, V)

            PM = Point3d.Add(P1, P2)
            PM = Point3d.Divide(PM, 2.0)
            PM = Point3d.Add(PM, V)
            ArcList.Add(New Rhino.Geometry.Arc(P1, PM, P2))
        Else 'Eigenkante
            V = Vector3d.Add(-cX, cY)
            V.Unitize()
            V = Vector3d.Multiply(S * (0.4 * Math.Sqrt(2)), V)
            PM = Rhino.Geometry.Point3d.Add(P1, V)
            ArcList.Add(New Rhino.Geometry.Arc(New Plane(PM, cX, cY), S * 0.4, Math.PI * 1.5))
            PM = Rhino.Geometry.Point3d.Add(PM, V)
        End If

        V.Unitize()
        TextPlane = New Plane(Point3d.Origin, cX, cY)

        V = Rhino.Geometry.Vector3d.Multiply(S / 5.0, V)
        TextPlane.Origin = Rhino.Geometry.Point3d.Add(PM, V)

        txt.Add(New Rhino.Display.Text3d(Val, TextPlane, S / 2.0))

    End Sub

End Class
