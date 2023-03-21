Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
Imports SpiderWebLibrary

Public Class GH_graphVertexList
    Inherits graphVertexList
    Implements GH_Graph

    ' -------------------------- parse geometry -------------------
    Friend Sub GraphFromDatatree(ByVal P_DATATREE As GH_Structure(Of IGH_Goo)) Implements GH_Graph.GraphFromDatatree
        Dim P As New GH_Path()
        For i As Integer = 0 To P_DATATREE.Branches.Count - 1
            P = P_DATATREE.Path(i)
            If P.Length = 2 And P.Dimension(0) <> P.Dimension(1) Then
                MyClass.insert(New graphEdge(P.Dimension(0), P.Dimension(1), False))
            End If
        Next
    End Sub

    Public Sub GraphFromDatatree(ByVal P_DATATREE As DataTree(Of System.Object)) Implements GH_Graph.GraphFromDatatree
        Dim P As New GH_Path()

        For i As Integer = 0 To P_DATATREE.Branches.Count - 1
            P = P_DATATREE.Path(i)
            If P.Length = 2 And P.Dimension(0) <> P.Dimension(1) Then
                MyClass.insert(New graphEdge(P.Dimension(0), P.Dimension(1), False))
            End If
        Next
    End Sub

    Public Function GraphFromLines(ByVal L_List As List(Of Line), _
                            ByVal undirected As Boolean, _
                            ByVal tol As Double) As List(Of Point3d) Implements GH_Graph.GraphFromLines
        Dim Vertex As New List(Of Point3d)
        Dim cursor, cursorTo As Integer
        Dim VC As New R_compPoint3d(tol)

        For Each L As Rhino.Geometry.Line In L_List
            cursor = Vertex.BinarySearch(L.To, VC)
            If cursor < 0 Or Vertex.Count = 0 Then
                cursor = cursor Xor -1
                Vertex.Insert(cursor, L.To)
            End If
            cursor = Vertex.BinarySearch(L.From, VC)
            If cursor < 0 Or Vertex.Count = 0 Then
                cursor = cursor Xor -1
                Vertex.Insert(cursor, L.From)
            End If
        Next

        For Each L As Rhino.Geometry.Line In L_List
            cursor = Vertex.BinarySearch(L.From, VC)
            cursorTo = Vertex.BinarySearch(L.To, VC)
            MyClass.insert(New graphEdge(cursor, cursorTo, False, L.Length))
        Next

        Return Vertex
    End Function

    Sub GraphFromPoint(ByVal Vertex As List(Of Point3d), _
                   ByVal conDist As Double, _
                   ByVal tol As Double) Implements GH_Graph.GraphFromPoint
        Dim P, P1 As Rhino.Geometry.Point3d
        Dim dist As Double

        tol = 1 / tol
        conDist = Math.Round(conDist * tol)

        For j As Integer = 0 To Vertex.Count - 1
            P = Vertex.Item(j)
            For jj As Integer = j + 1 To Vertex.Count - 1
                P1 = Vertex.Item(jj)
                dist = P.DistanceTo(P1)
                If (Math.Round(dist * tol) <= conDist) Then
                    MyClass.insert(New graphEdge(j, jj, False, dist))
                    MyClass.insert(New graphEdge(jj, j, False, dist))
                End If
            Next
        Next
    End Sub

    Function GraphFromCells(ByVal C_List As List(Of Curve), _
                               ByVal tol As Double) As List(Of Polyline) Implements GH_Graph.GraphFromCells

        Dim Vertex As New List(Of Point3d)
        Dim CP_List_S As New List(Of Polyline)
        Dim CP_List As New List(Of Polyline)
        Dim CP3d As New R_compPoint3d(tol)
        Dim CC3d As New R_compPolyline(tol)
        Dim eventX As New List(Of Double)

        CP_List = ConvertToPolyline(C_List)
        CP_List.Sort(CC3d)
        eventX = getEvents(CP_List)

        getVertex(CP_List, CP3d, Vertex, CP_List_S)

        Dim Aktive As New List(Of Polyline)

        Dim p, pp As Point3d
        Dim c, cc As New Polyline
        Dim d, dd As Double

        'run throug all event positions
        For Each pX In eventX
            ' get Acitve Elements
            Aktive = New List(Of Polyline)
            For i As Integer = 0 To CP_List.Count - 1
                c = CP_List.Item(i)
                p = Vertex.Item(i)
                If Math.Round(c.BoundingBox.Min.X / tol) <= Math.Round(pX / tol) And Math.Round(pX / tol) <= Math.Round(c.BoundingBox.Max.X / tol) Then
                    Aktive.Add(c)
                ElseIf Math.Round(pX / tol) < Math.Round(c.BoundingBox.Min.X / tol) Then
                    Exit For
                End If
            Next
            'Dbg.Add("Aktive: " & Aktive.Count)
            ' create new graphEdges into element
            For i As Integer = 0 To Aktive.Count - 1
                c = Aktive.Item(i)
                For ii As Integer = 0 To Aktive.Count - 1
                    If (i <> ii) Then
                        cc = Aktive.Item(ii)
                        For Each l In cc.GetSegments()

                            p = c.ClosestPoint(l.To)
                            pp = c.ClosestPoint(l.From)

                            d = p.DistanceTo(l.To)
                            dd = pp.DistanceTo(l.From)

                            If d <= tol And Not c.Contains(pp) Or _
                             dd <= tol And Not c.Contains(p) Or _
                             d <= tol And dd <= tol Then
                                p = c.BoundingBox.Center
                                pp = cc.BoundingBox.Center
                                MyClass.insert(New graphEdge(Vertex.BinarySearch(p, CP3d), Vertex.BinarySearch(pp, CP3d), p.DistanceTo(pp)))
                            End If
                        Next
                    End If
                Next
            Next
        Next

        Return CP_List_S
    End Function

    Private Function ConvertToPolyline(ByVal C_List As List(Of Curve)) As List(Of Polyline)
        Dim CP_List As New List(Of Polyline)
        Dim c As New Polyline
        For Each CK As Curve In C_List
            If CK.TryGetPolyline(c) Then
                CP_List.Add(c)
            End If
        Next
        Return CP_List
    End Function

    Private Function getEvents(ByVal CP_List As List(Of Polyline)) As List(Of Double)
        Dim c As New Polyline
        Dim cursor As Integer
        Dim eventPosX As New List(Of Double)

        For Each c In CP_List
            cursor = eventPosX.BinarySearch(c.BoundingBox.Min.X)
            If cursor < 0 Then
                cursor = cursor Xor -1
                eventPosX.Insert(cursor, c.BoundingBox.Min.X)
            End If
        Next
        Return eventPosX
    End Function

    Private Sub getVertex(ByVal CP_List As List(Of Polyline), _
                                      ByVal CP3d As R_compPoint3d, _
                                      ByRef Vertex As List(Of Point3d), _
                                      ByRef CP_List_S As List(Of Polyline) _
                                      )

        Dim c As New Polyline
        Dim p As New Point3d
        Dim cursor As Integer

        For Each c In CP_List
            p = c.BoundingBox.Center
            cursor = Vertex.BinarySearch(p, CP3d)
            If cursor < 0 Or Vertex.Count = 0 Then
                cursor = cursor Xor -1
                Vertex.Insert(cursor, p)
                CP_List_S.Insert(cursor, c)
            End If
        Next
    End Sub

    Sub EdgeGraphFromMesh(ByVal M As Mesh) Implements GH_Graph.EdgeGraphFromMesh
        Dim Vertex As New List(Of Point3d)
        Vertex.AddRange(M.Vertices.ToPoint3dArray())

        For Each f As Rhino.Geometry.MeshFace In M.Faces
            MyClass.insert(New graphEdge(f.A, f.B, Vertex.Item(f.A).DistanceTo(Vertex.Item(f.B))))
            MyClass.insert(New graphEdge(f.B, f.C, Vertex.Item(f.B).DistanceTo(Vertex.Item(f.C))))
            If f.IsQuad Then
                MyClass.insert(New graphEdge(f.C, f.D, Vertex.Item(f.C).DistanceTo(Vertex.Item(f.D))))
                MyClass.insert(New graphEdge(f.D, f.A, Vertex.Item(f.D).DistanceTo(Vertex.Item(f.A))))
            Else
                MyClass.insert(New graphEdge(f.C, f.A, Vertex.Item(f.C).DistanceTo(Vertex.Item(f.A))))
            End If
        Next
    End Sub

    Function FaceGraphFromMesh(ByVal M As Rhino.Geometry.Mesh) As List(Of Point3d) Implements GH_Graph.FaceGraphFromMesh
        Dim Vertex As New List(Of Point3d)

        For A As Integer = 0 To M.Faces.Count - 1
            Vertex.Add(M.Faces.GetFaceCenter(A))
        Next

        For A As Integer = 0 To M.Faces.Count - 1
            For Each B As Integer In M.Faces.AdjacentFaces(A)
                MyClass.insert(New graphEdge(A, B, Vertex.Item(A).DistanceTo(Vertex.Item(B))))
            Next
        Next

        Return Vertex
    End Function

    ' -------------------------- parse Vertex -------------------
    Public Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer)) Implements GH_Graph.parseVertexDataTree
        MyClass.clear()

        For i As Integer = 0 To gDT.BranchCount - 1
            For Each ii As Integer In gDT.Branch(i)
                MyClass.insert(New graphEdge(i, ii, False))
            Next
        Next
    End Sub

    Public Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer), _
                                 ByVal gecDT As DataTree(Of Double), _
                                 Optional ByVal methode As Integer = 1) Implements GH_Graph.parseVertexDataTree

        Dim cursor, nb As Integer
        Dim Cost, A, B As Double

        MyClass.clear()

        For i As Integer = 0 To gDT.BranchCount - 1
            For ii As Integer = 0 To gDT.Branch(i).Count - 1
                nb = gDT.Branch(i).Item(ii)
                A = gecDT.Branch(i).Item(ii)
                B = -1
                cursor = gDT.Branch(nb).BinarySearch(i)
                If cursor >= 0 Then
                    B = gecDT.Branch(nb).Item(cursor)
                End If
                Select Case methode
                    Case 0
                        Cost = A + B
                    Case 1
                        Cost = (A + B) / 2.0
                    Case 2
                        Cost = Math.Max(A, B)
                    Case 3
                        Cost = Math.Min(A, B)
                    Case 4
                        Cost = A
                    Case 5
                        Cost = B
                End Select

                MyClass.insert(New graphEdge(i, nb, Cost))
            Next
        Next
    End Sub

    Friend Sub parseVertexDataTree(ByVal gDT As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)) Implements GH_Graph.parseVertexDataTree
        MyClass.clear()

        For i As Integer = 0 To gDT.PathCount - 1
            For Each tmpNB As Grasshopper.Kernel.Types.GH_Integer In gDT.DataList(i)
                MyClass.insert(New graphEdge(i, tmpNB.Value))
            Next
        Next
    End Sub

    Friend Sub parseVertexDataTree(ByVal gDT As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer), _
                   ByVal gecDT As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number), _
               Optional ByVal methode As Integer = 1) Implements GH_Graph.parseVertexDataTree

        Dim tmpA, tmpB As Grasshopper.Kernel.Types.GH_Number
        Dim tmpNB As Grasshopper.Kernel.Types.GH_Integer
        Dim compI As New GH_compInteger()

        Dim cost As Double
        Dim cursor As Integer

        MyClass.clear()

        For i As Integer = 0 To gDT.PathCount - 1
            For ii As Integer = 0 To gDT.Branch(i).Count - 1
                tmpNB = gDT.DataList(i).Item(ii)
                tmpA = gecDT.DataList(i).Item(ii)
                tmpB = New Grasshopper.Kernel.Types.GH_Number(-1)
                tmpA = gecDT.DataList(i).Item(ii)
                cursor = gDT.DataList(tmpNB.Value).BinarySearch(New Grasshopper.Kernel.Types.GH_Integer(i), compI)
                If cursor >= 0 Then
                    tmpB = gecDT.DataList(tmpNB.Value).Item(cursor)
                End If
                Select Case methode
                    Case 0
                        cost = tmpA.Value + tmpB.Value
                    Case 1
                        cost = (tmpA.Value + tmpB.Value) / 2.0
                    Case 2
                        cost = Math.Max(tmpA.Value, tmpB.Value)
                    Case 3
                        cost = Math.Min(tmpA.Value, tmpB.Value)
                    Case 4
                        cost = tmpA.Value
                    Case 5
                        cost = tmpB.Value
                End Select

                MyClass.insert(New graphEdge(i, tmpNB.Value, cost))
            Next
        Next
    End Sub

    ' -------------------------- parse Edge -------------------
    Public Sub parseEdgeDataTree(ByVal egDT As DataTree(Of Integer)) Implements GH_Graph.parseEdgeDataTree
        MyClass.clear()

        For i As Integer = 0 To egDT.BranchCount - 1
            If egDT.Branch(i).Count = 2 Then
                MyClass.insert(New graphEdge(egDT.Branch(i).Item(0), egDT.Branch(i).Item(1)))
            End If
        Next
    End Sub

    Public Sub parseEdgeDataTree(ByVal egDT As DataTree(Of Integer), _
                               ByVal ecDT As DataTree(Of Double)) Implements GH_Graph.parseEdgeDataTree
        MyClass.clear()

        For i As Integer = 0 To egDT.BranchCount - 1
            If egDT.Branch(i).Count = 2 And ecDT.Branch(i).Count = 1 Then
                MyClass.insert(New graphEdge(egDT.Branch(i).Item(0), egDT.Branch(i).Item(1), ecDT.Branch(i).Item(0)))
            End If
        Next
    End Sub

    Friend Sub parseEdgeDataTree(ByVal egDT As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer)) Implements GH_Graph.parseEdgeDataTree
        MyClass.clear()

        For i As Integer = 0 To egDT.PathCount - 1
            If egDT.Branch(i).Count = 2 Then
                MyClass.insert(New graphEdge(egDT.Branch(i).Item(0), egDT.Branch(i).Item(1)))
            End If
        Next
    End Sub

    Friend Sub parseEdgeDataTree(ByVal egDT As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer), _
               ByVal ecDT As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)) Implements GH_Graph.parseEdgeDataTree
        MyClass.clear()

        For i As Integer = 0 To egDT.PathCount - 1
            If egDT.Branch(i).Count = 2 And ecDT.Branch(i).Count = 1 Then
                MyClass.insert(New graphEdge(egDT.Branch(i).Item(0).value, egDT.Branch(i).Item(1).value, ecDT.Branch(i).Item(0).value))
            End If
        Next
    End Sub

    ' -------------------------- parse Datatree -------------------
    ReadOnly Property EC_DATATREE() As Grasshopper.DataTree(Of Double) Implements GH_Graph.EC_DATATREE
        Get
            Dim tmpDT As New Grasshopper.DataTree(Of Double)

            Return tmpDT
        End Get
    End Property

    ReadOnly Property EG_DATATREE() As Grasshopper.DataTree(Of Integer) Implements GH_Graph.EG_DATATREE
        Get
            Dim tmpDT As New Grasshopper.DataTree(Of Integer)

            Return tmpDT
        End Get
    End Property

    ReadOnly Property EH_DATATREE() As Grasshopper.DataTree(Of Double) Implements GH_Graph.EH_DATATREE
        Get
            Dim tmpDT As New Grasshopper.DataTree(Of Double)

            Return tmpDT
        End Get
    End Property

    ReadOnly Property G_DATATREE() As Grasshopper.DataTree(Of Integer) Implements GH_Graph.G_DATATREE
        Get
            Dim tmpDT As New Grasshopper.DataTree(Of Integer)

            Return tmpDT
        End Get
    End Property

    ReadOnly Property GEC_DATATREE() As Grasshopper.DataTree(Of Double) Implements GH_Graph.GEC_DATATREE
        Get
            Dim tmpDT As New Grasshopper.DataTree(Of Double)

            Return tmpDT
        End Get
    End Property

    ReadOnly Property GEH_DATATREE() As Grasshopper.DataTree(Of Double) Implements GH_Graph.GEH_DATATREE
        Get
            Dim tmpDT As New Grasshopper.DataTree(Of Double)

            Return tmpDT
        End Get
    End Property

End Class
