Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class DisplayGraph
    Inherits GH_Component

    'collected Items
    Private GC As Drawing.Color


    'callculated Items
    Private Circle As List(Of Rhino.Geometry.Circle)
    Private Start As List(Of Rhino.Geometry.Circle)
    Private ArcList As List(Of Rhino.Geometry.Arc)
    Private txt As List(Of Rhino.Display.Text3d)


    Public Sub New()
        MyBase.New("Display Graph", "DG", "Display Graph", "Extra", "SpiderWebDisplay")
        Circle = New List(Of Rhino.Geometry.Circle)
        txt = New List(Of Rhino.Display.Text3d)
        ArcList = New List(Of Rhino.Geometry.Arc)
    End Sub

    Public Overrides Sub DrawViewportWires(ByVal args As Grasshopper.Kernel.IGH_PreviewArgs)
        ' MyBase.DrawViewportWires(args) '-> Disabled because nothing to Display

        If (Hidden) Then Return
        If (Circle.Count = 0) Then Return
        For Each item As Rhino.Geometry.Circle In Circle
            args.Display.DrawCircle(item, GC, 3)
        Next

        For Each item As Rhino.Geometry.Circle In Start
            args.Display.DrawCircle(item, GC, 3)
        Next

        For Each item As Rhino.Geometry.Arc In ArcList
            args.Display.DrawArc(item, GC)
        Next


        For Each item As Rhino.Display.Text3d In txt
            args.Display.Draw3dText(item, GC)
        Next
    End Sub

    Public Overrides Sub ClearData()
        MyBase.ClearData()
        Circle = New List(Of Rhino.Geometry.Circle)
        Start = New List(Of Rhino.Geometry.Circle)
        txt = New List(Of Rhino.Display.Text3d)
        ArcList = New List(Of Rhino.Geometry.Arc)
    End Sub


    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.DisplayIcon
        End Get
    End Property

    Public Overrides Sub BakeGeometry(ByVal doc As Rhino.RhinoDoc, ByVal att As Rhino.DocObjects.ObjectAttributes, ByVal obj_ids As System.Collections.Generic.List(Of System.Guid))
        ' MyBase.BakeGeometry(doc, att, obj_ids)

        For Each item As Rhino.Geometry.Circle In Circle
            doc.Objects.AddCircle(item)
        Next

        For Each item As Rhino.Geometry.Circle In Start
            doc.Objects.AddCircle(item)
        Next

        For Each item As Rhino.Geometry.Arc In ArcList
            doc.Objects.AddArc(item)
        Next

        For Each item As Rhino.Display.Text3d In txt
            doc.Objects.AddText(item)
        Next

    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("ffe68ae7-f86d-403e-8ae1-e31912061e03")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "directed", AddressOf setDirected, True, False = GetValue("undirected", False))
        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", False))
    End Sub

    Private Sub setDirected()
        SetValue("undirected", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setUndirected()
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("undirected", False)
            Case False
                Message = "directed"
            Case True
                Message = "undirected"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddPointParameter("GraphVertex", "GV", "Graph Vertex as Points", GH_ParamAccess.list)
        ' pManager.AddNumberParameter("Values", "V", "Values to Display", GH_ParamAccess.list, -1)
        pManager.AddTextParameter("Value", "V", "Value to Display", GH_ParamAccess.list, -1)
        pManager.AddNumberParameter("GraphEdgeCost", "GEC", "Edge Cost to Display", GH_ParamAccess.tree, -1)
        pManager.AddNumberParameter("Size", "S", "Size of Labels", GH_ParamAccess.item, 1.0)
        pManager.AddColourParameter("GraphColor", "GC", "Color of Graph", GH_ParamAccess.item, Drawing.Color.DeepPink)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_CircleParam("VRep", "VR", "Vertex Representation")
        pManager.Register_ArcParam("ConRep", "CR", "Connection Representation")
        pManager.Register_LineParam("GraphLines", "GL", "Lines Between Graph Vertecies")
    End Sub

    Protected Overrides Sub BeforeSolveInstance()
        MyBase.BeforeSolveInstance()


        Circle = New List(Of Rhino.Geometry.Circle)
        Start = New List(Of Rhino.Geometry.Circle)
        txt = New List(Of Rhino.Display.Text3d)
        ArcList = New List(Of Rhino.Geometry.Arc)

    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim S As Double
        Dim GV_L As New List(Of Rhino.Geometry.Point3d)
        Dim Val As New List(Of String)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)
        Dim GEC_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)

        Dim tmpCircle As New List(Of Rhino.Geometry.Circle)
        Dim tmpArcList As New List(Of Rhino.Geometry.Arc)

        G_DATATREE = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)()
        GEC_DATATREE = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)()


        If (Not DA.GetDataList("GraphVertex", GV_L)) Then Return
        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return
        If (Not DA.GetData("Size", S)) Then Return
        If (Not DA.GetData("GraphColor", GC)) Then Return
        If (Not DA.GetDataList("Value", Val)) Then Return


        If (G_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If
        If (GV_L.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input GV(GraphVertices)")
            Return
        End If
        If (G_DATATREE.PathCount <> GV_L.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input GV. Number of branches (G) must be equal to number of Points (GV) ")
            Return
        End If

        If (G_DATATREE.Branches.Count > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Callulation might return false results. Graph limited to 65534 vertices!")
            Return
        End If

        DA.GetDataTree("GraphEdgeCost", GEC_DATATREE)

        Dim undirected As Boolean = GetValue("undirected", False)

        Dim EdgeList As New List(Of graphEdge)
        Dim LineList As New List(Of Rhino.Geometry.Line)
        Dim V, V1, V2 As Rhino.Geometry.Vector3d
        Dim P1, P2, PM As Rhino.Geometry.Point3d
        Dim i, ip, iTemp As Integer
        Dim tempTXT As Rhino.Display.Text3d
        Dim tempV As Double

        Dim E As graphEdge

        Dim Plane As New Rhino.Geometry.Plane(New Rhino.Geometry.Point3d(0, 0, 0), New Rhino.Geometry.Vector3d(0, 0, 1))

        For ip = 0 To G_DATATREE.PathCount - 1

            tmpCircle.Add(New Rhino.Geometry.Circle(GV_L.Item(ip), S))

            tempTXT = New Rhino.Display.Text3d(ip)
            tempTXT.Height = S * (1 - 0.2 * tempTXT.Text.Length)
            V = Rhino.Geometry.Vector3d.Multiply(-0.5, tempTXT.BoundingBox.Diagonal)
            Plane.Origin = Rhino.Geometry.Point3d.Add(GV_L.Item(ip), V)
            tempTXT.TextPlane = Plane

            txt.Add(tempTXT)


            Plane.Origin = Rhino.Geometry.Point3d.Add(GV_L.Item(ip), New Rhino.Geometry.Point3d(S * 0.8, S * 0.8, 0))
            If Val.Count = GV_L.Count Then
                txt.Add(New Rhino.Display.Text3d(Val.Item(ip), Plane, S))
            End If

            For i = 0 To G_DATATREE.DataList(ip).Count - 1
                G_DATATREE.DataList(ip).Item(i).CastTo(iTemp)
                If GEC_DATATREE.PathCount = G_DATATREE.PathCount Then
                    GEC_DATATREE.DataList(ip).Item(i).CastTo(tempV)
                    E = New graphEdge(ip, iTemp, tempV, undirected)
                Else
                    E = New graphEdge(ip, iTemp, -1, undirected)
                End If
                iTemp = EdgeList.BinarySearch(E, E)
                If iTemp < 0 Then
                    iTemp = iTemp Xor -1
                    EdgeList.Insert(iTemp, E)
                End If
            Next
        Next

        For Each item As graphEdge In EdgeList
            P1 = GV_L.Item(item.A)
            P2 = GV_L.Item(item.B)
            V = Rhino.Geometry.Point3d.Subtract(P1, P2)

            If V.Length > 0 Then ' Normale Kante
                V = Rhino.Geometry.Vector3d.Multiply(0.05, V)
                V = New Rhino.Geometry.Vector3d(V.Y, -V.X, 0)

                PM = Rhino.Geometry.Point3d.Add(P1, P2)
                PM = Rhino.Geometry.Point3d.Divide(PM, 2.0)
                PM = Rhino.Geometry.Point3d.Add(PM, V)

                V1 = Rhino.Geometry.Point3d.Subtract(PM, P1)
                V1.Z = 0
                V1.Unitize()
                V1 = Rhino.Geometry.Vector3d.Multiply(S, V1)

                V2 = Rhino.Geometry.Point3d.Subtract(PM, P2)
                V2.Z = 0
                V2.Unitize()
                V2 = Rhino.Geometry.Vector3d.Multiply(S, V2)
            Else 'Eigenkante
                V = New Rhino.Geometry.Vector3d(-S * 2, S * 2, 0)
                V.Unitize()
                V = Rhino.Geometry.Vector3d.Multiply(S * 1.5, V)

                V1 = New Rhino.Geometry.Vector3d(-S, S * 2, 0)
                V1.Unitize()
                V1 = Rhino.Geometry.Vector3d.Multiply(S, V1)

                V2 = New Rhino.Geometry.Vector3d(-S * 2, S, 0)
                V2.Unitize()
                V2 = Rhino.Geometry.Vector3d.Multiply(S, V2)

                PM = Rhino.Geometry.Point3d.Add(P1, V)
            End If

            If item.Cost <> -1 Then
                V.Unitize()

                Plane = New Rhino.Geometry.Plane(New Rhino.Geometry.Point3d(0, 0, 0), New Rhino.Geometry.Vector3d(0, 0, 1))

                Plane.Rotate(V.Y, V.X, Plane.ZAxis)
                Plane.Rotate(-1, 0, Plane.ZAxis)

                V = Rhino.Geometry.Vector3d.Multiply(S / 5.0, V)
                Plane.Origin = Rhino.Geometry.Point3d.Add(PM, V)

                txt.Add(New Rhino.Display.Text3d(item.Cost, Plane, S / 2.0))
            End If

            tmpArcList.Add(New Rhino.Geometry.Arc(Rhino.Geometry.Point3d.Add(P1, V1), PM, Rhino.Geometry.Point3d.Add(P2, V2)))
            LineList.Add(New Rhino.Geometry.Line(P1, P2))

            Start.Add(New Rhino.Geometry.Circle(Rhino.Geometry.Point3d.Add(P1, V1), S / 6))
            If undirected Then
                Start.Add(New Rhino.Geometry.Circle(Rhino.Geometry.Point3d.Add(P2, V2), S / 6))
            End If
        Next

        Circle.AddRange(tmpCircle)
        ArcList.AddRange(tmpArcList)

        DA.SetDataList(0, tmpCircle)
        DA.SetDataList(1, tmpArcList)
        DA.SetDataList(2, LineList)
    End Sub
End Class
