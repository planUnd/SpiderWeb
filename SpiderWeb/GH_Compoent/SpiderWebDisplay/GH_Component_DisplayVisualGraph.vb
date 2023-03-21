Option Explicit On

Imports Grasshopper
Imports Rhino.Geometry
Imports Rhino.Display
Imports Grasshopper.Kernel
Imports Grasshopper.GUI
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphElements

Public Class GH_Component_DisplayVisualGraph
    Inherits GH_Component

    Private gC As List(Of Rectangle3d)
    Private c As List(Of System.Drawing.Color)

    Private ReadOnly cL() As System.Drawing.Color = New System.Drawing.Color(3) {Drawing.Color.Blue, Drawing.Color.Green, Drawing.Color.Yellow, Drawing.Color.Red}

    Public Sub New()
        MyBase.New("Display visualGraph", "dVG", "Display visualGraph", "Extra", "SpiderWebDisplay")
        AddHandler Me.ObjectChanged, AddressOf PreviewChange
        Me.gC = New List(Of Rectangle3d)
        Me.c = New List(Of System.Drawing.Color)
    End Sub

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "cellState (invalid=-1; void=0; solid=1;)", AddressOf setCellState, True, 0 = GetValue("display", 0))
        Menu_AppendItem(menu, "area", AddressOf setArea, True, 1 = GetValue("display", 0))
        Menu_AppendItem(menu, "boundary-4NB", AddressOf setBoundary4, True, 2 = GetValue("display", 0))
        Menu_AppendItem(menu, "boundary-8NB", AddressOf setBoundary8, True, 3 = GetValue("display", 0))
    End Sub

    Private Sub setCellState(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("display", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setArea(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("display", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setBoundary4(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("display", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setBoundary8(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("display", 3)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("display", 0)
            Case 0
                Message = "cellState"
            Case 1
                Message = "area"
            Case 2
                Message = "boundary-4NB"
            Case 3
                Message = "boundary-8NB"
        End Select
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Public Overrides Sub DrawViewportWires(args As IGH_PreviewArgs)
        For i As Integer = 0 To Me.gC.Count - 1
            args.Display.DrawPolyline(Me.gC.Item(i).ToPolyline(), Me.c.Item(i), 1)
        Next
    End Sub

    Public Overrides Sub DrawViewportMeshes(args As IGH_PreviewArgs)
        Dim M As Mesh
        For i As Integer = 0 To Me.gC.Count - 1
            M = Mesh.CreateFromClosedPolyline(Me.gC.Item(i).ToPolyline())
            args.Display.DrawMeshShaded(M, New Rhino.Display.DisplayMaterial(Me.c.Item(i)))
        Next
    End Sub

    Public Overrides Sub ClearData()
        MyBase.ClearData()
        Me.gC = New List(Of Rectangle3d)
        Me.c = New List(Of System.Drawing.Color)
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.VGProperties
        End Get
    End Property

    Public Overrides Sub BakeGeometry(ByVal doc As Rhino.RhinoDoc, ByVal att As Rhino.DocObjects.ObjectAttributes, ByVal obj_ids As System.Collections.Generic.List(Of System.Guid))
        Me.Hidden = False
        SetValue("hidden", Me.Hidden)
        Me.ExpireSolution(True)

        att.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject
        For i As Integer = 0 To Me.gC.Count - 1
            att.ObjectColor = Me.c.Item(i)
            doc.Objects.AddPolyline(Me.gC.Item(i).ToPolyline(), att)
        Next
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("7FAA13C0-CC52-49E3-8A2A-825175E9D5A9")
        End Get
    End Property

    Protected Overrides Sub BeforeSolveInstance()
        MyBase.BeforeSolveInstance()
        Me.gC = New List(Of Rectangle3d)
        Me.c = New List(Of System.Drawing.Color)
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_visualGraphParam(), "visualGraph", "vG", "visualGraph representation", GH_ParamAccess.item)
        pManager.AddRectangleParameter("gridCells", "gC", "Grid Cells", GH_ParamAccess.list)
        pManager.Param(1).Optional = True
        pManager.AddNumberParameter("Value", "V", "Value to Display", GH_ParamAccess.list, -1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("vertexCount", "VC", "number of Vertices in the Graph")
        pManager.Register_IntegerParam("edgeCount", "EC", "number of Edges in the Graph")
        pManager.Register_IntegerParam("W", "W", "Width")
        pManager.Register_IntegerParam("H", "H", "Height")
        pManager.Register_DoubleParam("V", "V", "Values")
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

        Dim Size As Double = GetValue("Size", 1.0)
        Dim vG As New GH_visualGraph()
        Dim displayVal As New List(Of Double)
        Dim Val As New List(Of Double)
        Dim maxVal, minVal As Double

        If (Not DA.GetData("visualGraph", vG)) Then Return
        DA.GetDataList("gridCells", Me.gC)

        If (vG.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input vG(visualGraph)")
            Return
        End If
        If (Me.gC.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "No valid data collected in input gC(gridCells).")
        End If
        If (vG.vertexCount <> gC.Count And Me.gC.Count > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input vG doesn't match input gC.")
            Return
        End If

        DA.SetData(0, vG.vertexCount)
        DA.SetData(1, vG.edgeCount)
        DA.SetData(2, vG.width)
        DA.SetData(3, vG.height)

        Dim dis As Integer = GetValue("display", 0)

        Select Case dis
            Case 1
                For Each gV As graphVertex In vG.getVertexList
                    Val.Add(gV.outDegree)
                Next
            Case 2
                For i As Integer = 0 To vG.vertexCount - 1
                    Val.Add(vG.circumference(i))
                Next
            Case 3
                For i As Integer = 0 To vG.vertexCount - 1
                    Val.Add(vG.circumference(i, 8))
                Next
            Case Else
                For Each v As Integer In vG.gridCellList
                    Val.Add(v)
                Next
        End Select


        DA.SetDataList(4, Val)

        If Not GetValue("hidden", Me.Hidden) And Me.gC.Count >= vG.vertexCount Then
            If (Not DA.GetDataList("Value", displayVal)) Then Return

            If displayVal.Count <> Me.gC.Count Then  'display area state
                Dim curr As Integer = 0
                displayVal = New List(Of Double)
                minVal = Val.Min()
                maxVal = Val.Max()
                For Each s As GH_visualGraph.State In vG.gridCellList
                    If s = GH_visualGraph.State.void Then
                        displayVal.Add(Val.Item(curr))
                    ElseIf s = GH_visualGraph.State.solid Then
                        displayVal.Add(Double.PositiveInfinity)
                    Else
                        displayVal.Add(Double.NegativeInfinity)
                    End If
                    curr += 1
                Next
            Else
                minVal = displayVal.Min()
                maxVal = minVal
                For Each item As Double In displayVal
                    If Not Double.IsPositiveInfinity(item) And maxVal < item Then
                        maxVal = item
                    End If
                Next
            End If

            Dim v As Double
            Dim dom As Integer
            For i As Integer = 0 To displayVal.Count - 1
                If Double.IsPositiveInfinity(displayVal.Item(i)) Then
                    c.Add(System.Drawing.Color.Black)
                ElseIf Double.IsNegativeInfinity(displayVal.Item(i)) Then
                    c.Add(System.Drawing.Color.LightGray)
                Else
                    If maxVal = minVal Then
                        dom = 1
                    Else
                        v = (displayVal.Item(i) - minVal) / (maxVal - minVal)
                        dom = Math.Floor(v / 0.25)
                        dom = Math.Min(dom, 3)
                        dom = Math.Max(dom, 1)
                    End If
                    c.Add(GH_GraphicsUtil.BlendColour(Me.cL(dom - 1), Me.cL(dom), (v - (dom * 0.25)) / 0.25))
                End If
            Next

        End If

    End Sub

End Class
