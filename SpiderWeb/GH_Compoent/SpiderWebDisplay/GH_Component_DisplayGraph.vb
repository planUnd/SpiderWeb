Option Explicit On

Imports Rhino.Geometry
Imports Rhino.Display
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_DisplayGraph
    Inherits GH_Component

    Private Circle As List(Of Circle)
    Private ArcList As List(Of Arc)
    Private txt As List(Of Text3d)
    Private gP As GH_graphStyle

    Public Sub New()
        MyBase.New("Display Graph", "DG", "Display Graph", "Extra", "SpiderWebDisplay")
        AddHandler Me.ObjectChanged, AddressOf PreviewChange
        Circle = New List(Of Circle)
        txt = New List(Of Text3d)
        ArcList = New List(Of Arc)
        gP = New GH_graphStyle()
    End Sub

    Public Overrides Sub DrawViewportWires(ByVal args As Grasshopper.Kernel.IGH_PreviewArgs)
        ' MyBase.DrawViewportWires(args) '-> Disabled because nothing to Display
        Dim t As Double

        If (Me.Hidden) Then Return
        If (Circle.Count = 0) Then Return
        For Each item As Circle In Circle
            args.Display.DrawCircle(item, Me.gP.displayColor, 3)
        Next

        For Each item As Arc In ArcList
            args.Display.DrawArc(item, Me.gP.displayColor)
            t = item.ClosestParameter(item.EndPoint)
            args.Display.DrawArrowHead(item.EndPoint, item.TangentAt(t - 0.25), Me.gP.displayColor, 0, gP.txtSize * 0.25)
        Next

        For Each item As Text3d In txt
            args.Display.Draw3dText(item, Me.gP.displayColor)
        Next
    End Sub

    Public Overrides Sub ClearData()
        MyBase.ClearData()
        Circle = New List(Of Circle)
        txt = New List(Of Text3d)
        ArcList = New List(Of Arc)
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.DisplayIcon
        End Get
    End Property

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

        att.ObjectDecoration = Rhino.DocObjects.ObjectDecoration.EndArrowhead
        For Each item As Arc In ArcList
            doc.Objects.AddArc(item, att)
        Next
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("ffe68ae7-f86d-403e-8ae1-e31912061e03")
        End Get
    End Property

    Protected Overrides Sub BeforeSolveInstance()
        MyBase.BeforeSolveInstance()
        Circle = New List(Of Circle)
        txt = New List(Of Text3d)
        ArcList = New List(Of Arc)
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the graph", GH_ParamAccess.list)
        pManager.Param(1).Optional = True
        pManager.AddParameter(New GH_graphStyleParam(), "graphStyle", "gS", "Style for Graph Drawing", GH_ParamAccess.item)
        pManager.Param(2).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("vertexCount", "VC", "number of Vertices in the Graph")
        pManager.Register_IntegerParam("edgeCount", "EC", "number of Edges in the Graph")
        pManager.Register_LineParam("Edge", "E", "Edge as Line")
    End Sub

    Private Sub PreviewChange(ByVal sender As Object, ByVal e As GH_ObjectChangedEventArgs)
        If (e.Type = GH_ObjectEventType.Preview) Then
            SetValue("hidden", Me.Hidden)
            Me.ExpireSolution(True)
            Me.ExpirePreview(True)
        End If
    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Me.Hidden = GetValue("hidden", True)

        Dim GP_L As New List(Of Point3d)
        Dim gVL As New GH_GraphVertexList()

        Dim LineList As New List(Of Line)

        If (Not DA.GetData("Graph", gVL)) Then Return

        DA.GetDataList("graphPoints", GP_L)
        If (Not DA.GetData("graphStyle", Me.gP)) Then
            Me.gP = New GH_graphStyle()
        End If

        If (gVL.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GP_L.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "No valid data collected in input GP(Graph Points). No output at E(Edge).")
        End If
        If (gVL.vertexCount <> GP_L.Count And GP_L.Count > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Input G doesn't match input GP.")
        End If
        If (gVL.vertexCount > GP_L.Count And GP_L.Count > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "To less GP to represent G")
            Return
        End If

        DA.SetData(0, gVL.vertexCount)
        DA.SetData(1, gVL.edgeCount)

        If GP_L.Count = 0 Then Return

        For Each gV As SpiderWebLibrary.graphElements.graphVertex In gVL.getVertexList
            For i As Integer = 0 To gV.outDegree - 1
                LineList.Add(New Line(GP_L.Item(gV.index), GP_L.Item(gV.neighbours(i))))
            Next
        Next

        DA.SetDataList(2, LineList)

        If Not GetValue("hidden", Me.Hidden) Then
            For Each gV As SpiderWebLibrary.graphElements.graphVertex In gVL.getVertexList
                InsertNode(gV.index, gP.txtSize, GP_L.Item(gV.index))
                For Each nb As Integer In gV.neighbours
                    InsertEdge(GP_L.Item(gV.index), GP_L.Item(nb), gP.txtSize)
                Next
            Next
        End If

    End Sub

    Private Sub InsertEdge(ByVal P1 As Point3d, ByVal P2 As Point3d, ByVal S As Double)
        Dim PM As Point3d
        Dim V As Vector3d = Point3d.Subtract(P1, P2)
        Dim cY As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraY
        Dim cX As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraX
        Dim cZ As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraZ

        If V.Length > 0 Then ' Normale Kante
            V = Vector3d.CrossProduct(V, cZ)
            V = Vector3d.Multiply(Math.Sqrt(gP.txtSize) * 0.05, V)

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
        End If


    End Sub

    Private Sub InsertNode(ByVal index As Integer, _
                       ByVal S As Double, _
                       ByVal P As Point3d)
        Dim cX As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraX
        Dim cY As Vector3d = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.MainViewport.CameraY
        Dim TextPlane As Plane = New Plane(Point3d.Origin, cX, cY)

        Dim tmpC As New Circle(P, S * 0.4)
        TextPlane.Origin = P
        tmpC.Plane = TextPlane
        Circle.Add(tmpC)

        TextPlane.Origin = Point3d.Add(P, Vector3d.Add(Vector3d.Multiply(S * 0.4, cX), Vector3d.Multiply(S * 0.4, cY)))
        txt.Add(New Text3d(index, TextPlane, S))
    End Sub
End Class
