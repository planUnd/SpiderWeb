Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
Imports SpiderWebLibrary.graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports GH_SpiderWebLibrary.GH_compare
Imports GH_SpiderWebLibrary.R_Compare


Namespace GH_graphRepresentaions

    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : GH_GraphEdgeList
    ''' 
    ''' <summary>
    ''' Edge List representation of a Graph.
    ''' </summary>
    ''' <remarks>
    ''' Extends the SpiderWebLibrary class graphEdgeList with additional methodes linked to Grasshopper and Rhino.
    ''' These function mainly help to convert from geometry to a topological representation (Graph).
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Class GH_GraphEdgeList
        Inherits graphEdgeList
        Implements IGH_Goo, GH_Graph

        ' ------------------------- Constructor ----------------------

        ''' <summary>
        '''  Construct a empty Graph represented by a list of graphEdges.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphEdges, based on another Graph.
        ''' </summary>
        ''' <param name="G">Graph implementing the Graph interface.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal G As Graph)
            MyBase.New(G)
        End Sub

        ' ------------------------- IGH_Goo ----------------------
        ''' <summary>
        ''' This method is called whenever the instance is required to deserialize itself. (Inherited from GH_ISerializable.)
        ''' </summary>
        ''' <returns>Returns <code>False</code></returns>
        ''' <remarks>Not implemented!</remarks>
        Public Function Read(reader As GH_IO.Serialization.GH_IReader) As Boolean Implements GH_IO.GH_ISerializable.Read
            Return False
        End Function

        ''' <summary>
        ''' This method is called whenever the instance is required to serialize itself. (Inherited from GH_ISerializable.) 
        ''' </summary>
        ''' <returns>Returns <code>False</code></returns>
        ''' <remarks>Not implemented!</remarks>
        Public Function Write(writer As GH_IO.Serialization.GH_IWriter) As Boolean Implements GH_IO.GH_ISerializable.Write
            Return False
        End Function

        ''' <summary>
        ''' Attempt a cast from generic object 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CastFrom(source As Object) As Boolean Implements IGH_Goo.CastFrom
            If source Is Nothing Then Return False
            If GetType(Graph).IsAssignableFrom(source.GetType) Then
                Dim gEL As Graph = DirectCast(source, Graph)
                Me.edgeList = gEL.getEdgeList
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Attempt a cast to type T 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="target"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CastTo(Of T)(ByRef target As T) As Boolean Implements IGH_Goo.CastTo
            If (GetType(T).IsAssignableFrom(GetType(GH_GraphEdgeList))) Then
                Dim tmpGraph As Object = Me
                target = DirectCast(tmpGraph, T)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Make a complete duplicate of this instance. No shallow copies. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Duplicate() As IGH_Goo Implements IGH_Goo.Duplicate
            Return New GH_GraphEdgeList(Me)
        End Function

        ''' <summary>
        ''' Create a new proxy for this instance. Return Null if this class does not support proxies. 
        ''' </summary>
        ''' <returns>Returns <code>Nothing</code></returns>
        ''' <remarks>Not implemented!</remarks>
        Public Function EmitProxy() As IGH_GooProxy Implements IGH_Goo.EmitProxy
            Return Nothing
        End Function

        ''' <summary>
        ''' Gets a value indicating whether or not the current value is valid. 
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns true.</returns>
        ''' <remarks> Graphs should always be valid, if not please contact the author.</remarks>
        Public ReadOnly Property IsValid As Boolean Implements IGH_Goo.IsValid
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Gets a string describing the state of "invalidness". If the instance is valid, then this property should return Nothing or String.Empty. 
        ''' </summary>
        ''' <value></value> 
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsValidWhyNot As String Implements IGH_Goo.IsValidWhyNot
            Get
                Return "If you can read this please contact the author of SpiderWeb and ask for help."
            End Get
        End Property

        ''' <summary>
        ''' Get the "naked" SpiderWebLibrary represnetation.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ScriptVariable() As Object Implements IGH_Goo.ScriptVariable
            Return New graphEdgeList(Me)
        End Function

        ''' <summary>
        ''' Creates a string description of the current instance value 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToStringGH() As String Implements IGH_Goo.ToString
            Return "GH_" & Me.ToString()
        End Function

        ''' <summary>
        ''' Gets the TypeDescription
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeDescription As String Implements IGH_Goo.TypeDescription
            Get
                Return "Graph represented as List of Edges"
            End Get
        End Property

        ''' <summary>
        ''' Gets the TypeName
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeName As String Implements IGH_Goo.TypeName
            Get
                Return "GH_graphEdgeList"
            End Get
        End Property

        ' ------------------------- GH_Graph ----------------------

        ''' <inheritdoc/>
        Public Sub GraphFromDatatree(ByVal P_DATATREE As GH_Structure(Of IGH_Goo)) Implements GH_Graph.GraphFromDatatree
            Dim P As New GH_Path()
            For i As Integer = 0 To P_DATATREE.Branches.Count - 1
                P = P_DATATREE.Path(i)
                If P.Length = 2 And P.Dimension(0) <> P.Dimension(1) Then
                    Me.insert(New graphEdge(P.Dimension(0), P.Dimension(1)))
                End If
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub GraphFromDatatree(ByVal P_DATATREE As GH_Structure(Of IGH_Goo), ByVal gecDT As GH_Structure(Of GH_Number)) Implements GH_Graph.GraphFromDatatree
            Dim P As New GH_Path()
            For i As Integer = 0 To P_DATATREE.Branches.Count - 1
                P = P_DATATREE.Path(i)
                If P.Length = 2 And P.Dimension(0) <> P.Dimension(1) Then
                    If gecDT.Branch(P).Count > 0 Then
                        Me.insert(New graphEdge(P.Dimension(0), P.Dimension(1), gecDT.Branch(P).Item(0).value))
                    Else
                        Me.insert(New graphEdge(P.Dimension(0), P.Dimension(1)))
                    End If
                End If
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub GraphFromDatatree(ByVal P_DATATREE As DataTree(Of System.Object)) Implements GH_Graph.GraphFromDatatree
            Dim P As New GH_Path()

            For i As Integer = 0 To P_DATATREE.Branches.Count - 1
                P = P_DATATREE.Path(i)
                If P.Length = 2 And P.Dimension(0) <> P.Dimension(1) Then
                    Me.insert(New graphEdge(P.Dimension(0), P.Dimension(1)))
                End If
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub GraphFromDatatree(ByVal P_DATATREE As DataTree(Of System.Object), ByVal gecDT As DataTree(Of Double)) Implements GH_Graph.GraphFromDatatree
            Dim P As New GH_Path()

            For i As Integer = 0 To P_DATATREE.Branches.Count - 1
                P = P_DATATREE.Path(i)
                If P.Length = 2 And P.Dimension(0) <> P.Dimension(1) Then
                    If gecDT.Branch(P).Count > 0 Then
                        Me.insert(New graphEdge(P.Dimension(0), P.Dimension(1), gecDT.Branch(P).Item(0)))
                    Else
                        Me.insert(New graphEdge(P.Dimension(0), P.Dimension(1)))
                    End If
                End If
            Next
        End Sub

        ''' <inheritdoc/>
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
                Me.insert(New graphEdge(cursor, cursorTo, L.Length))
            Next

            Return Vertex
        End Function

        ''' <inheritdoc/>
        Function GraphFromLineIntersection(ByVal L_List As List(Of Line), _
                ByVal tol As Double) As List(Of Point3d) Implements GH_Graph.GraphFromLineIntersection

            Dim graphPoint As New List(Of Point3d)
            Dim tA, tB As Double
            Dim comP As New R_compPoint3d(tol)

            For A As Integer = 0 To L_List.Count - 1
                For B As Integer = (A + 1) To L_List.Count - 1
                    If Intersect.Intersection.LineLine(L_List.Item(A), L_List.Item(B), tA, tB, tol, True) Or _
                        comP.Compare(L_List.Item(A).To, L_List.Item(B).To) = 0 Or _
                        comP.Compare(L_List.Item(A).To, L_List.Item(B).From) = 0 Or _
                        comP.Compare(L_List.Item(A).From, L_List.Item(B).From) = 0 Or _
                        comP.Compare(L_List.Item(A).From, L_List.Item(B).To) = 0 Then
                        Me.insert(New graphEdge(A, B, 1))
                        Me.insert(New graphEdge(B, A, 1))
                    End If
                Next
                graphPoint.Add(L_List.Item(A).PointAt(0.5))
            Next

            Return graphPoint
        End Function

        ''' <inheritdoc/>
        Public Sub GraphFromPoint(ByVal Vertex As List(Of Point3d), _
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
                        Me.insert(New graphEdge(j, jj, dist))
                        Me.insert(New graphEdge(jj, j, dist))
                    End If
                Next
            Next
        End Sub

        ''' <inheritdoc/>
        Public Function GraphFromCells(ByVal C_List As List(Of Curve), _
                                ByVal tol As Double, _
                                Optional ByVal minDist As Double = 0, _
                                Optional ByVal edgeCost As Boolean = False) As List(Of Polyline) Implements GH_Graph.GraphFromCells

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
            Dim d, dd, cost As Double
            Dim A, B As Integer
            Dim cursor As Integer

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
                    A = Vertex.BinarySearch(c.BoundingBox.Center, CP3d)
                    For ii As Integer = 0 To Aktive.Count - 1
                        If (i <> ii) Then
                            cc = Aktive.Item(ii)
                            B = Vertex.BinarySearch(cc.BoundingBox.Center, CP3d)
                            If Me.find(New graphEdge(A, B)) >= 0 Then
                                Continue For
                            End If
                            For Each l In cc.GetSegments()

                                p = c.ClosestPoint(l.To)
                                pp = c.ClosestPoint(l.From)

                                d = p.DistanceTo(l.To)
                                dd = pp.DistanceTo(l.From)

                                If p.DistanceTo(pp) >= minDist Then
                                    cost = p.DistanceTo(pp)

                                    If d <= tol And dd <= tol Or _
                                        dd <= tol And Math.Abs(l.Length - d - cost) <= tol And Not CP3d.Compare(p, pp) = 0 Or _
                                        d <= tol And Math.Abs(l.Length - dd - cost) <= tol And Not CP3d.Compare(p, pp) = 0 Then

                                        If edgeCost Then
                                            If Not Me.insert(New graphEdge(A, B, cost)) Then
                                                cursor = Me.find(New graphEdge(A, B))
                                                Me.Item(cursor).addCost = cost
                                            End If
                                            'If Not Me.insert(New graphEdge(B, A, cost)) Then
                                            ' cursor = Me.find(New graphEdge(B, A))
                                            '  Me.Item(cursor).addCost = cost
                                            ' End If
                                        Else
                                            cost = c.BoundingBox.Center.DistanceTo(cc.BoundingBox.Center)
                                            Me.insert(New graphEdge(A, B, cost))
                                            Exit For

                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next
            Next

            Me.ensureUndirected()

            Return CP_List_S
        End Function

        ''' <inheritdoc/>
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

        ''' <inheritdoc/>
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

        ''' <inheritdoc/>
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

        ''' <inheritdoc/>
        Public Sub EdgeGraphFromMesh(ByVal M As Mesh) Implements GH_Graph.EdgeGraphFromMesh
            Dim Vertex As New List(Of Point3d)
            Vertex.AddRange(M.Vertices.ToPoint3dArray())

            For Each f As Rhino.Geometry.MeshFace In M.Faces
                Me.insert(New graphEdge(f.A, f.B, Vertex.Item(f.A).DistanceTo(Vertex.Item(f.B))))
                Me.insert(New graphEdge(f.B, f.C, Vertex.Item(f.B).DistanceTo(Vertex.Item(f.C))))
                If f.IsQuad Then
                    Me.insert(New graphEdge(f.C, f.D, Vertex.Item(f.C).DistanceTo(Vertex.Item(f.D))))
                    Me.insert(New graphEdge(f.D, f.A, Vertex.Item(f.D).DistanceTo(Vertex.Item(f.A))))
                Else
                    Me.insert(New graphEdge(f.C, f.A, Vertex.Item(f.C).DistanceTo(Vertex.Item(f.A))))
                End If
            Next
        End Sub

        ''' <inheritdoc/>
        Public Function FaceGraphFromMesh(ByVal M As Rhino.Geometry.Mesh) As List(Of Point3d) Implements GH_Graph.FaceGraphFromMesh
            Dim Vertex As New List(Of Point3d)

            For A As Integer = 0 To M.Faces.Count - 1
                Vertex.Add(M.Faces.GetFaceCenter(A))
            Next

            For A As Integer = 0 To M.Faces.Count - 1
                For Each B As Integer In M.Faces.AdjacentFaces(A)
                    Me.insert(New graphEdge(A, B, Vertex.Item(A).DistanceTo(Vertex.Item(B))))
                Next
            Next

            Return Vertex
        End Function

        ''' <inheritdoc/>
        Public Function dualGraphAngular(ByVal GP_L As List(Of Point3d)) As List(Of Point3d) Implements GH_Graph.dualGraphAngular
            Dim dGVec_L As New List(Of Vector3d)
            Dim dGP_L As New List(Of Point3d)

            dualGraphVectorVertexList(GP_L, dGP_L, dGVec_L)

            Me.dualGraph()

            For Each gE As graphEdge In Me.getEdgeList
                gE.Cost = Vector3d.VectorAngle(dGVec_L.Item(gE.A), dGVec_L.Item(gE.B))
            Next

            Return dGP_L
        End Function

        ''' <inheritdoc/>
        Public Function dualVertexGraphAngular(ByVal GP_L As List(Of Point3d)) As List(Of Point3d) Implements GH_Graph.dualVertexGraphAngular
            Dim dGVec_L As New List(Of Vector3d)
            Dim dGP_L As New List(Of Point3d)
            Dim eN As Integer = Me.vertexCount

            dGP_L.AddRange(GP_L)

            dualGraphVectorVertexList(GP_L, dGP_L, dGVec_L)

            Me.dualVertexGraph()

            For Each gE As graphEdge In Me.getEdgeList
                If gE.A > eN Then
                    gE.Cost = Vector3d.VectorAngle(dGVec_L.Item(gE.A - eN), dGVec_L.Item(gE.B - eN))
                End If
            Next

            Return dGP_L
        End Function

        ''' <inheritdoc/>
        Public Function dualGraphTopological(ByVal GP_L As List(Of Point3d), _
                                             ByVal tol As Double) As List(Of Point3d) Implements GH_Graph.dualGraphTopological
            Dim dGVec_L As New List(Of Vector3d)
            Dim dGP_L As New List(Of Point3d)
            Dim ang As Double

            tol = 1 / tol

            dualGraphVectorVertexList(GP_L, dGP_L, dGVec_L)

            Me.dualGraph()

            For Each gE As graphEdge In Me.getEdgeList
                ang = Math.Round(Vector3d.VectorAngle(dGVec_L.Item(gE.A), dGVec_L.Item(gE.B)) * tol)
                gE.Cost = Convert.ToInt32(ang <> 0)
            Next

            Return dGP_L
        End Function

        ''' <inheritdoc/>
        Public Function dualVertexGraphTopological(ByVal GP_L As List(Of Point3d), _
                                                   ByVal tol As Double) As List(Of Point3d) Implements GH_Graph.dualVertexGraphTopological
            Dim dGVec_L As New List(Of Vector3d)
            Dim dGP_L As New List(Of Point3d)
            Dim eN As Integer = Me.vertexCount
            Dim ang As Double

            tol = 1 / tol

            dGP_L.AddRange(GP_L)

            dualGraphVectorVertexList(GP_L, dGP_L, dGVec_L)

            Me.dualVertexGraph()

            For Each gE As graphEdge In Me.getEdgeList
                If gE.A > eN Then
                    ang = Math.Round(Vector3d.VectorAngle(dGVec_L.Item(gE.A - eN), dGVec_L.Item(gE.B - eN)) * tol)
                    gE.Cost = Convert.ToInt32(ang <> 0)
                End If
            Next

            Return dGP_L
        End Function

        ' private
        Private Sub dualGraphVectorVertexList(ByVal GP_L As List(Of Point3d), _
                                                   ByRef dGP_L As List(Of Point3d), _
                                                    ByRef dGVec_L As List(Of Vector3d))
            Dim tmpVec As Vector3d
            For Each gE As graphEdge In Me.getEdgeList
                tmpVec = Vector3d.Subtract(GP_L.Item(gE.B), GP_L.Item(gE.A))
                tmpVec = Vector3d.Multiply((1D / 3D), tmpVec)
                dGVec_L.Add(tmpVec)
                dGP_L.Add(Point3d.Add(GP_L.Item(gE.A), tmpVec))
            Next
        End Sub

        ' -------------------------- parse IN Vertex -------------------

        ''' <inheritdoc/>
        Public Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer)) Implements GH_Graph.parseVertexDataTree
            Me.clear()
            Dim P As New GH_Path()
            For i As Integer = 0 To gDT.BranchCount - 1
                P = gDT.Path(i)
                For Each ii As Integer In gDT.Branch(i)
                    Me.insert(New graphEdge(P.Dimension(0), ii))
                Next
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub parseVertexDataTree(ByVal gDT As GH_Structure(Of GH_Integer)) Implements GH_Graph.parseVertexDataTree
            Me.clear()
            Dim P As New GH_Path()
            For i As Integer = 0 To gDT.PathCount - 1
                P = gDT.Path(i)
                For Each tmpNB As GH_Integer In gDT.DataList(i)
                    Me.insert(New graphEdge(P.Dimension(0), tmpNB.Value))
                Next
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub parseVertexDataTree(ByVal gDT As DataTree(Of Integer), _
                                     ByVal gecDT As DataTree(Of Double), _
                                     Optional ByVal methode As Integer = 4) Implements GH_Graph.parseVertexDataTree

            Me.clear()

            If gDT.BranchCount <> gecDT.BranchCount Or _
                gDT.DataCount <> gecDT.DataCount Then
                Exit Sub
            End If

            Dim P As New GH_Path()
            Dim cursor, nb As Integer
            Dim Cost, A, B As Double

            For i As Integer = 0 To gDT.BranchCount - 1
                P = gDT.Path(i)
                For ii As Integer = 0 To gDT.Branch(i).Count - 1
                    nb = gDT.Branch(i).Item(ii)
                    A = gecDT.Branch(i).Item(ii)
                    B = 0
                    If gDT.BranchCount > nb Then
                        cursor = gDT.Branch(nb).BinarySearch(i)
                        If cursor >= 0 Then
                            B = gecDT.Branch(nb).Item(cursor)
                        End If
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

                    Me.insert(New graphEdge(P.Dimension(0), nb, Cost))
                Next
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub parseVertexDataTree(ByVal gDT As GH_Structure(Of GH_Integer), _
                       ByVal gecDT As GH_Structure(Of GH_Number), _
                   Optional ByVal methode As Integer = 4) Implements GH_Graph.parseVertexDataTree
            Me.clear()

            If gDT.PathCount <> gecDT.PathCount Or _
                gDT.DataCount <> gecDT.DataCount Then
                Exit Sub
            End If

            Dim P As New GH_Path()
            Dim tmpA, tmpB As GH_Number
            Dim tmpNB As GH_Integer
            Dim compI As New GH_compInteger()

            Dim cost As Double
            Dim cursor As Integer

            For i As Integer = 0 To gDT.PathCount - 1
                P = gDT.Path(i)
                For ii As Integer = 0 To gDT.Branch(i).Count - 1
                    tmpNB = gDT.DataList(i).Item(ii)
                    tmpA = gecDT.DataList(i).Item(ii)
                    tmpA = gecDT.DataList(i).Item(ii)
                    tmpB = New GH_Number(0)
                    If gDT.PathCount > tmpNB.Value Then
                        cursor = gDT.DataList(tmpNB.Value).BinarySearch(New GH_Integer(i), compI)
                        If cursor >= 0 Then
                            tmpB = gecDT.DataList(tmpNB.Value).Item(cursor)
                        End If
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

                    Me.insert(New graphEdge(P.Dimension(0), tmpNB.Value, cost))
                Next
            Next
        End Sub

        ' -------------------------- parse IN Edge -------------------
        ''' <inheritdoc/>
        Public Sub parseEdgeDataTree(ByVal egDT As DataTree(Of Integer)) Implements GH_Graph.parseEdgeDataTree
            Me.clear()

            For i As Integer = 0 To egDT.BranchCount - 1
                If egDT.Branch(i).Count = 2 Then
                    Me.insert(New graphEdge(egDT.Branch(i).Item(0), egDT.Branch(i).Item(1)))
                End If
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub parseEdgeDataTree(ByVal egDT As GH_Structure(Of GH_Integer)) Implements GH_Graph.parseEdgeDataTree
            Me.clear()

            For i As Integer = 0 To egDT.PathCount - 1
                If egDT.Branch(i).Count = 2 Then

                    Me.insert(New graphEdge(egDT.Branch(i).Item(0).value, egDT.Branch(i).Item(1).value))
                End If
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub parseEdgeDataTree(ByVal egDT As DataTree(Of Integer), _
                                   ByVal ecDT As DataTree(Of Double)) Implements GH_Graph.parseEdgeDataTree
            Me.clear()
            If egDT.BranchCount <> ecDT.BranchCount Then
                Exit Sub
            End If
            For i As Integer = 0 To egDT.BranchCount - 1
                If egDT.Branch(i).Count = 2 And ecDT.Branch(i).Count = 1 Then
                    Me.insert(New graphEdge(egDT.Branch(i).Item(0), egDT.Branch(i).Item(1), ecDT.Branch(i).Item(0)))
                End If
            Next
        End Sub

        ''' <inheritdoc/>
        Public Sub parseEdgeDataTree(ByVal egDT As GH_Structure(Of GH_Integer), _
                   ByVal ecDT As GH_Structure(Of GH_Number)) Implements GH_Graph.parseEdgeDataTree
            Me.clear()
            If egDT.PathCount <> ecDT.PathCount Then
                Exit Sub
            End If
            For i As Integer = 0 To egDT.PathCount - 1
                If egDT.Branch(i).Count = 2 And ecDT.Branch(i).Count = 1 Then
                    Me.insert(New graphEdge(egDT.Branch(i).Item(0).value, egDT.Branch(i).Item(1).value, ecDT.Branch(i).Item(0).value))
                End If
            Next
        End Sub

        ' -------------------------- parse Out Datatree -------------------
        ''' <inheritdoc/>
        ReadOnly Property EG_DATATREE() As DataTree(Of Integer) Implements GH_Graph.EG_DATATREE
            Get
                Dim tmpDT As New DataTree(Of Integer)
                Dim cursor As Integer = 0
                For Each gE As graphEdge In Me.getEdgeList
                    tmpDT.EnsurePath(cursor)
                    tmpDT.Branch(cursor).Add(gE.A)
                    tmpDT.Branch(cursor).Add(gE.B)
                    cursor += 1
                Next
                Return tmpDT
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property EC_DATATREE() As DataTree(Of Double) Implements GH_Graph.EC_DATATREE
            Get
                Dim tmpDT As New DataTree(Of Double)
                Dim cursor As Integer = 0
                For Each gE As graphEdge In Me.getEdgeList
                    tmpDT.EnsurePath(cursor)
                    tmpDT.Branch(cursor).Add(gE.Cost)
                    cursor += 1
                Next
                Return tmpDT
            End Get
        End Property


        ''' <inheritdoc/>
        ReadOnly Property G_DATATREE() As Grasshopper.DataTree(Of Integer) Implements GH_Graph.G_DATATREE
            Get
                Dim tmpDT As New Grasshopper.DataTree(Of Integer)
                For Each gV As graphVertex In Me.getVertexList
                    tmpDT.EnsurePath(gV.index)
                    tmpDT.AddRange(gV.neighbours)
                Next
                Return tmpDT
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property GEC_DATATREE() As Grasshopper.DataTree(Of Double) Implements GH_Graph.GEC_DATATREE
            Get
                Dim tmpDT As New Grasshopper.DataTree(Of Double)
                For Each gV As graphVertex In Me.getVertexList
                    tmpDT.EnsurePath(gV.index)
                    tmpDT.AddRange(gV.cost)
                Next
                Return tmpDT
            End Get
        End Property


    End Class
End Namespace