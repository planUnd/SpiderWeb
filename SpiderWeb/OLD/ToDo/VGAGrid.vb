Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class VGAGrid
    Inherits GH_Component


    Public Sub New()
        MyBase.New("Visual Graph Grid", "visualGraphGrid", "Creates a Grid of Points Within the Boundingbox of a Curve", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("4bce8a5e-0416-4f0d-b407-5ef1bf855293")
        End Get
    End Property


    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddCurveParameter("Area", "A", "Area Included within the visual graph grid", GH_ParamAccess.item)
        pManager.AddCurveParameter("Obstacles", "O", "Visual obstacles within the area", GH_ParamAccess.list)
        pManager(1).Optional = True
        pManager.AddNumberParameter("Grid Size", "GS", "Grid size", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("x", "x", "Number of elements in X-direction")
        pManager.Register_IntegerParam("y", "y", "Number of elements in Y-direction")
        pManager.Register_IntegerParam("Value", "V", "solid = -1; accessible = 0; boundary = 1,2,4,5,6,8,9,10;")
        pManager.Register_PointParam("VisualGraphVertex", "VGV", "Visual graph vertex as points")
        pManager.Register_RectangleParam("VisualGraphRectangle", "VGR", "Visual graph vertex as rectangle")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.VGAGrid
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolstripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "MidPoint", AddressOf setMidpoint, True, 0 = GetValue("GR", 0))
        Menu_AppendItem(menu, "Rectangle", AddressOf setRectangle, True, 1 = GetValue("GR", 0))
        Menu_AppendItem(menu, "Boundary", AddressOf setBoundary, True, 2 = GetValue("GR", 0))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("GR", 0)
            Case 1
                Message = "Rectangle"
            Case 2
                Message = "Boundary"
            Case 0
                Message = "MidPoint"
        End Select
    End Sub

    Private Sub setMidpoint(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("GRUndoEvent")
        SetValue("GR", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setRectangle(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("GRUndoEvent")
        SetValue("GR", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setBoundary(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("GRUndoEvent")
        SetValue("GR", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim A As Rhino.Geometry.Curve

        Dim O_List As New List(Of Rhino.Geometry.Curve)
        Dim sXY As Double
        Dim Gr As Byte = GetValue("GR", 0)

        If (Not DA.GetData("Area", A)) Then Return
        If (Not DA.GetData("Grid Size", sXY)) Then Return
        DA.GetDataList("Obstacles", O_List)

        Dim BB As Rhino.Geometry.BoundingBox
        Dim tmpX, tmpY As Integer
        Dim sX, sY, minX, minY As Double

        Dim tmpP As Rhino.Geometry.Point3d
        Dim PList As New List(Of Rhino.Geometry.Point3d)
        Dim VList As New List(Of SByte)
        Dim RList As New List(Of Rhino.Geometry.Rectangle3d)
        Dim tmpR As Rhino.Geometry.Rectangle3d

        BB = A.GetBoundingBox(True)

        tmpX = Math.Round((BB.Max().X - BB.Min().X) / sXY)
        tmpY = Math.Round((BB.Max().Y - BB.Min().Y) / sXY)


        sX = (BB.Max.X - BB.Min.X) / tmpX
        sY = (BB.Max.Y - BB.Min.Y) / tmpY

        If sY < sX Then
            sY = sX
        Else
            sX = sY
        End If

        minX = BB.Min().X + sX / 2.0
        minY = BB.Min().Y + sY / 2.0

        For iy As Integer = 0 To tmpY - 1
            For ix As Integer = 0 To tmpX - 1
                tmpP = New Rhino.Geometry.Point3d(minX + sX * ix, minY + sY * iy, 0)
                PList.Add(tmpP)
                tmpR = New Rhino.Geometry.Rectangle3d(New Rhino.Geometry.Plane(tmpP, New Rhino.Geometry.Vector3d(0, 0, 1)), New Rhino.Geometry.Interval(-sX / 2.0, sX / 2.0), New Rhino.Geometry.Interval(-sY / 2.0, sY / 2.0))
                RList.Add(tmpR)
                VList.Add(Inside(O_List, A, tmpP, tmpR, GR))
            Next
        Next

        DA.SetData(0, tmpX)
        DA.SetData(1, tmpY)
        DA.SetDataList(2, VList)
        DA.SetDataList(3, PList)
        DA.SetDataList(4, RList)
    End Sub

    Private Function Inside(ByVal O As List(Of Rhino.Geometry.Curve), ByVal A As Rhino.Geometry.Curve, ByVal p As Rhino.Geometry.Point3d, ByVal tmpR As Rhino.Geometry.Rectangle3d, ByVal GR As Integer) As SByte
        Dim tmpC As New Rhino.Geometry.PolylineCurve(tmpR.ToPolyline())
        Dim cI As Rhino.Geometry.Intersect.CurveIntersections

        Dim retValue As SByte = 0

        ' Search area
        cI = Rhino.Geometry.Intersect.Intersection.CurveCurve(A, tmpC, 0, 0)
        If A.Contains(p) = Rhino.Geometry.PointContainment.Outside Then
            Return -1
        ElseIf GR = 2 And cI.Count > 0 Then
            retValue = getDirection(cI, p)
        ElseIf GR = 1 And cI.Count > 0 Then
            Return -1
        End If

        ' Search Obsticals
        For Each c As Rhino.Geometry.Curve In O
            cI = Rhino.Geometry.Intersect.Intersection.CurveCurve(c, tmpC, 0, 0)
            If c.Contains(p) = Rhino.Geometry.PointContainment.Inside Then
                Return -1
            ElseIf GR = 2 And cI.Count > 0 Then
                retValue = getDirection(cI, p)
            ElseIf GR = 1 And cI.Count > 0 Then
                Return -1
            End If
        Next

        Return retValue
    End Function

    Private Function getDirection(ByVal cI As Rhino.Geometry.Intersect.CurveIntersections, ByVal p As Rhino.Geometry.Point3d) As SByte
        Dim b As SByte = 0
        Dim tol As Double = GH_Component.DocumentTolerance()
        Dim x, y As Double
        Dim tmpV As Rhino.Geometry.Vector3d
        Dim tmpP As New Rhino.Geometry.Point3d

        If cI.Count > 2 Then
            Return -1
        End If
        For i As Integer = 0 To cI.Count - 1
            tmpP = Rhino.Geometry.Point3d.Add(tmpP, cI.Item(i).PointB)
            tmpP = Rhino.Geometry.Point3d.Add(tmpP, cI.Item(i).PointB2)
        Next

        tmpP = Rhino.Geometry.Point3d.Divide(tmpP, 2 * cI.Count)
        tmpV = Rhino.Geometry.Vector3d.Subtract(tmpP, p)

        x = Math.Round(tmpV.X / tol)
        y = Math.Round(tmpV.Y / tol)

        If x > 0 Then
            b += 8
        ElseIf x < 0 Then
            b += 4
        End If
        If y > 0 Then
            b += 2
        ElseIf y < 0 Then
            b += 1
        End If
        Return b


    End Function

End Class
