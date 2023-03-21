Imports Grasshopper.Kernel

Public Class RecursiveShadowCasting
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Recursive Shadow Casting", "recursiveShadowCasting", "Preforms Recursive Shadow Casting Algorithm on visualGraphGrid From Different Starting Points", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("045bd97c-ba73-4601-93fe-0fe87afef7be")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("x", "x", "Number of elements in X-direction", GH_ParamAccess.item)
        pManager.AddIntegerParameter("y", "y", "Number of elements in Y-direction", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Value", "V", "-1 = solid; else accessible", GH_ParamAccess.list)
        pManager.AddPointParameter("VisualGraphVertex", "VGV", "Visual graph vertex as Points", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Starting Points", "SP", "Indices of points to preform rekursive shadow casting from", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Visual Graph", "VG", "Indices of the grid points that can be seen from the starting points")
        ' pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.RSC
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        ' Dim DebuggStr As New List(Of String)
        Dim V_List As New List(Of Integer)
        Dim P_List As New List(Of Rhino.Geometry.Point3d)
        Dim SP As New List(Of Integer)
        Dim x, y As Integer

        If (Not DA.GetData("x", x)) Then Return
        If (Not DA.GetData("y", y)) Then Return
        If (Not DA.GetDataList("Starting Points", SP)) Then Return
        If (Not DA.GetDataList("Value", V_List)) Then Return
        If (Not DA.GetDataList("VisualGraphVertex", P_List)) Then Return
        If (V_List.Count <> P_List.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input V doesn't match input VGV. Number of values in (V) must be equal to number of points (VGV) ")
            Return
        End If

        Dim tmp As New List(Of Integer)
        Dim tmpC As New List(Of Integer)
        Dim addList As New List(Of Integer)
        Dim cursor, index As Integer
        Dim OctList As List(Of List(Of Integer))

        Dim tmpBV() As Integer
        Dim bValues(,) As Integer = New Integer(7, 2) {{2, 4, 6}, {2, 8, 10}, {2, 8, 10}, {1, 9, 8}, {1, 9, 8}, {1, 5, 4}, {1, 5, 4}, {2, 4, 6}}


        Dim tmpVis As New Grasshopper.DataTree(Of Integer)
        Dim ghP As New Grasshopper.Kernel.Data.GH_Path()


        For iSP As Integer = 0 To SP.Count - 1
            index = SP.Item(iSP)
            ghP = New Grasshopper.Kernel.Data.GH_Path(index)
            tmpVis.EnsurePath(ghP)
            If V_List.Item(index) >= 0 Then
                tmp = New List(Of Integer)
                tmp.Add(index)
                For i As Integer = 1 To 8
                    tmpBV = New Integer(2) {bValues(i - 1, 0), bValues(i - 1, 1), bValues(i - 1, 2)}

                    If Not tmpBV.Contains(V_List.Item(SP.Item(iSP))) Then

                        ' DebuggStr.Add("Octant: " & i & " item: " & SP.Item(iSP))
                        OctList = getOctants(SP.Item(iSP), i, x, y)
                        addList = scan(OctList, 1.0, 0.0, P_List, V_List, 1, tmpBV)
                        For iA As Integer = 0 To addList.Count - 1
                            index = addList.Item(iA)
                            cursor = tmp.BinarySearch(index)
                            If cursor < 0 Then
                                cursor = cursor Xor -1
                                tmp.Insert(cursor, index)
                            End If
                        Next
                    End If
                Next
                tmpVis.Branch(ghP).AddRange(tmp)
            End If
        Next

        For i As Integer = 0 To P_List.Count - 1
            tmpVis.EnsurePath(i)
        Next

        tmpVis.SimplifyPaths()

        DA.SetDataTree(0, tmpVis)
        ' DA.SetDataList(2, DebuggStr)
    End Sub

    Private Function scan(ByVal OctList As List(Of List(Of Integer)), _
                          ByVal sSlop As Double, ByVal eSlop As Double, _
                          ByVal P As List(Of Rhino.Geometry.Point3d), _
                          ByVal V As List(Of Integer), _
                          ByVal row As Integer, _
                          ByVal vArr() As Integer _
                          ) As List(Of Integer)

        Dim retList As New List(Of Integer)

        If OctList.Count = 1 Or OctList.Count <= row Then
            Return retList
        End If

        Dim cursor As Integer
        Dim value As Integer
        Dim solid As Integer = 0
        Dim slop As Double
        Dim SP, CP As Rhino.Geometry.Point3d
        Dim dLVec, dRVec As Rhino.Geometry.Vector3d

        If OctList.Count > 1 Then
            dRVec = Rhino.Geometry.Point3d.Subtract(P.Item(OctList.Item(1).Last()), P.Item(OctList.Item(0).Item(0)))
            dRVec = Rhino.Geometry.Vector3d.Divide(dRVec, 2.0)
        End If
        If OctList.Item(row).Count > 1 Then
            dLVec = Rhino.Geometry.Point3d.Subtract(P.Item(OctList.Item(row).Item(1)), P.Item(OctList.Item(row).Item(0)))
            dLVec = Rhino.Geometry.Vector3d.Divide(dLVec, 2.0)
        End If

        Dim tol As Double = GH_Component.DocumentTolerance()

        SP = P.Item(OctList.Item(0).Item(0))

        For iL As Integer = row To OctList.Count - 1
            solid = 3
            For iR As Integer = 0 To OctList.Item(iL).Count - 1

                cursor = OctList.Item(iL).Item(iR)
                value = V.Item(cursor)
                CP = P.Item(cursor)
                slop = getSlop(SP, CP)

                If (slop <= sSlop + tol) Then
                    If (slop >= eSlop - tol) Then
                        If (value >= 0 And Not vArr.Contains(value)) Then
                            solid = 0
                            retList.Add(cursor)
                        ElseIf (value > 0) Then
                            retList.Add(cursor)
                            If solid = 0 Then
                                CP = P.Item(cursor)
                                CP = Rhino.Geometry.Point3d.Subtract(CP, dLVec)
                                retList.AddRange(scan(OctList, sSlop, getSlop(SP, CP), P, V, iL + 1, vArr))
                                solid = 2

                            ElseIf solid = 3 Or solid = 1 Then
                                solid = 2
                            End If
                        ElseIf solid = 0 Then 'Or solid = 2 Then
                            CP = P.Item(cursor)
                            CP = Rhino.Geometry.Point3d.Subtract(CP, dLVec)
                            retList.AddRange(scan(OctList, sSlop, getSlop(SP, CP), P, V, iL + 1, vArr))
                            solid = 1
                        ElseIf solid = 3 Then
                            solid = 1
                        End If
                        If solid = 1 Then
                            CP = P.Item(cursor)
                            CP = Rhino.Geometry.Point3d.Add(CP, dLVec)
                            sSlop = Math.Min(sSlop, getSlop(SP, CP))
                            If iR = OctList.Item(iL).Count - 1 Then
                                sSlop *= -1
                            End If
                        ElseIf solid = 2 Then
                            CP = P.Item(cursor)
                            CP = Rhino.Geometry.Point3d.Add(CP, dRVec)
                            sSlop = getSlop(SP, CP)
                            If iR = OctList.Item(iL).Count - 1 Then
                                sSlop *= -1
                            End If
                        End If
                    Else
                        If value = -1 Then
                            CP = P.Item(cursor)
                            CP = Rhino.Geometry.Point3d.Subtract(CP, dLVec)
                            eSlop = Math.Max(eSlop, getSlop(SP, CP))
                        End If
                        Exit For
                    End If

                ElseIf value = -1 Then
                    CP = P.Item(cursor)
                    CP = Rhino.Geometry.Point3d.Add(CP, dLVec)
                    sSlop = Math.Min(sSlop, getSlop(SP, CP))
                End If

            Next
            If Math.Abs(eSlop - sSlop) < tol Then
                Exit For
            End If
        Next

        Return retList
    End Function

    Private Function getSlop(ByVal SP As Rhino.Geometry.Point3d, _
  ByVal CP As Rhino.Geometry.Point3d) As Double
        Dim slop As Double

        Dim dX As Double = Math.Abs(SP.X - CP.X)
        Dim dY As Double = Math.Abs(SP.Y - CP.Y)
        If dX <> 0 And dY <> 0 Then
            slop = Math.Min(dX / dY, dY / dX)
        Else
            slop = 0
        End If

        Return slop
    End Function

    Private Function getOctants(ByVal s As Integer, ByVal Oct As Integer, ByVal ex As Integer, ByVal ey As Integer) As List(Of List(Of Integer))
        Dim retList As New List(Of List(Of Integer))
        Dim tmpList As New List(Of Integer)
        Dim c As Integer = 1
        Dim minC As Integer
        Dim cursor As Integer

        tmpList.Add(s)
        retList.Add(tmpList)
        Select Case Oct
            Case 1
                minC = s - ex * Math.Floor(s / ex)
                While (s + ex * c < ex * ey)
                    tmpList = New List(Of Integer)
                    For i As Integer = Math.Min(c, minC) To 0 Step -1
                        tmpList.Add(s + ex * c - i)
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
            Case 2
                minC = ex - (s Mod ex) - 1
                While (s + ex * c < ex * ey)
                    tmpList = New List(Of Integer)
                    For i As Integer = Math.Min(c, minC) To 0 Step -1
                        tmpList.Add(s + ex * c + i)
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
            Case 3
                minC = ex - (s Mod ex) - 1
                While (c <= minC)
                    tmpList = New List(Of Integer)
                    For i As Integer = 0 To Math.Min(c, minC)
                        cursor = (s + (ex + 1) * c - (i * ex))
                        If cursor < ex * ey Then
                            tmpList.Add(cursor)
                        End If
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
            Case 4
                minC = ex - (s Mod ex) - 1
                While (c <= minC)
                    tmpList = New List(Of Integer)
                    For i As Integer = 0 To Math.Min(c, minC)
                        cursor = s - (ex - 1) * c + ex * i
                        If cursor >= 0 Then
                            tmpList.Add(cursor)
                        End If
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
            Case 5
                minC = ex - (s Mod ex) - 1
                While (s - ex * c >= 0)
                    tmpList = New List(Of Integer)
                    For i As Integer = Math.Min(c, minC) To 0 Step -1
                        tmpList.Add(s - ex * c + i)
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
            Case 6
                minC = s - ex * Math.Floor(s / ex)
                While (s - ex * c >= 0)
                    tmpList = New List(Of Integer)
                    For i As Integer = Math.Min(c, minC) To 0 Step -1
                        tmpList.Add(s - ex * c - i)
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
            Case 7
                minC = s - ex * Math.Floor(s / ex)
                While (c <= minC)
                    tmpList = New List(Of Integer)
                    For i As Integer = 0 To Math.Min(c, minC)
                        cursor = (s - (ex + 1) * c + (i * ex))
                        If cursor >= 0 Then
                            tmpList.Add(cursor)
                        End If
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
            Case 8
                minC = s - ex * Math.Floor(s / ex)
                While (c <= minC)
                    tmpList = New List(Of Integer)
                    For i As Integer = 0 To Math.Min(c, minC)
                        cursor = (s + (ex - 1) * c - (i * ex))
                        If cursor < ex * ey Then
                            tmpList.Add(cursor)
                        End If
                    Next
                    retList.Add(tmpList)
                    c += 1
                End While
                Return retList
        End Select

        Return retList
    End Function
End Class
