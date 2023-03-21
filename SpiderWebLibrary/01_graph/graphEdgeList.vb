Option Explicit On


Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports MathNet.Numerics.LinearAlgebra

Namespace graphRepresentaions
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : graphEdgeList
    ''' 
    ''' <summary>
    ''' Edge List representation of a Graph
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    '''  [Richard Schaffranek]   16/11/2014 changed: meta.numerics -> math.net
    ''' </history>
    ''' 

    Public Class graphEdgeList
        Implements Graph

        Protected edgeList As List(Of graphEdge)
        ' -------------------- Constructor --------------------------

        ''' <summary>
        '''  Construct a empty Graph represented by a list of graphEdges.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            edgeList = New List(Of graphEdge)
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphEdges, from another Graph.
        ''' </summary>
        ''' <param name="G">Graph implementing the Graph interface.</param>
        ''' <remarks>Computes in O(n), where n is the number of graphEdges in the Graph.</remarks>
        Public Sub New(ByVal G As Graph)
            Me.edgeList = New List(Of graphEdge)
            For Each gE As graphEdge In G.getEdgeList
                Me.add(New graphEdge(gE))
            Next
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphEdges and inserts the gE.
        ''' </summary>
        ''' <param name="gE"></param>
        ''' <remarks> </remarks>
        Public Sub New(ByVal gE As graphEdge)
            edgeList = New List(Of graphEdge)
            Me.add(gE)
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphEdges, from a List(of graphEdge)
        ''' </summary>
        ''' <param name="tmpEdgeList"></param>
        ''' <remarks>The List(of graphEdge) will be sorted  befor adding the edges to the Graph. 
        ''' On Average this computes in O(n log n), where n is the number of graphEdges in the Graph.
        ''' If your are constructing the Graph from an existing Graph (G) use:
        ''' <code>
        ''' new graphEdgeList(G)
        ''' </code>
        ''' </remarks>
        Public Sub New(ByVal tmpEdgeList As List(Of graphEdge))
            MyClass.edgeList = New List(Of graphEdge)
            tmpEdgeList.Sort(New graphEdge())
            For Each gE As graphEdge In tmpEdgeList
                Me.add(New graphEdge(gE))
            Next
        End Sub

        ' -------------------- Overrides --------------------------
        ''' <inheritdoc/>
        Public Overrides Function ToString() As String
            Return "graphEdgeList: " & Me.vertexCount & " vertices, " & Me.edgeCount & " edges"
        End Function

        ' -------------------- GraphInterface --------------------------

        ''' <inheritdoc/>
        ReadOnly Property split_tAt(ByVal t As Double, ByVal costs As List(Of Double)) As Graph() Implements Graph.split_tAt
            Get
                Dim gEL(2) As Graph
                gEL(0) = New graphEdgeList()
                gEL(1) = New graphEdgeList()
                Dim sgE(2) As graphEdge
                Dim tmpT As Double
                Dim tmpVC As Integer = Me.vertexCount
                If tmpVC = costs.Count Then
                    For Each gE In Me.edgeList
                        tmpT = gE.tAt(costs.Item(gE.A), costs.Item(gE.B), t)
                        If tmpT >= 1 Then
                            gEL(0).insert(gE)
                        ElseIf tmpT <= 0 Then
                            gEL(1).insert(gE)
                        Else
                            sgE = gE.split(tmpVC, tmpT)
                            gEL(0).insert(sgE(0))
                            gEL(1).insert(sgE(1))
                            tmpVC += 1
                        End If
                    Next
                End If
                Return gEL
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property tAt(ByVal t As Double, ByVal costs As List(Of Double)) As List(Of Double) Implements Graph.tAt
            Get
                Dim tL As New List(Of Double)
                If Me.vertexCount = costs.Count Then
                    For Each gE In Me.edgeList
                        tL.Add(gE.tAt(costs.Item(gE.A), costs.Item(gE.B), t))
                    Next
                End If
                Return tL
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property graphEdgeAt(ByVal t As Double, ByVal costs As List(Of Double)) As List(Of graphEdge) Implements Graph.graphEdge_tAt
            Get
                Dim tgEL As New List(Of graphEdge)
                If Me.vertexCount = costs.Count Then
                    For Each gE In Me.edgeList
                        If t < costs.Item(gE.A) And costs.Item(gE.B) > t Then
                            tgEL.Add(gE)
                        End If
                    Next
                End If
                Return tgEL
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property graphEdgeIndexAt(ByVal t As Double, ByVal costs As List(Of Double)) As List(Of Integer) Implements Graph.graphEdgeIndex_tAt
            Get
                Dim tgELi As New List(Of Integer)
                Dim index As Integer = 0
                If Me.vertexCount = costs.Count Then
                    For Each gE In Me.edgeList
                        If t < costs.Item(gE.A) And costs.Item(gE.B) > t Then
                            tgELi.Add(index)
                        End If
                        index += 1
                    Next
                End If
                Return tgELi
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property edgeCount() As Integer Implements Graph.edgeCount
            Get
                Return MyClass.edgeList.Count
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property vertexCount() As Integer Implements Graph.vertexCount
            Get
                Dim ret As Integer = 0
                For Each gE As graphEdge In MyClass.edgeList
                    ret = Math.Max(ret, gE.A + 1)
                    ret = Math.Max(ret, gE.B + 1)
                Next
                Return ret
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property negativeCost(Optional ByVal tol As Double = 1) As Boolean Implements Graph.negativeCost
            Get
                For Each gE In Me.edgeList
                    If gE.negativeCost(tol) Then
                        Return True
                    End If
                Next
                Return False
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getInEdges(ByVal i As Integer) As List(Of graphEdge) Implements Graph.getInEdges
            Get
                Dim inEdges As New List(Of graphEdge)

                For Each item As graphEdge In edgeList
                    If item.B = i Then
                        inEdges.Add(item)
                    End If
                Next

                Return inEdges
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getOutEdges(ByVal i As Integer) As List(Of graphEdge) Implements Graph.getOutEdges
            Get
                Dim outEdges As New List(Of graphEdge)

                For Each item As graphEdge In edgeList
                    If item.A = i Then
                        outEdges.Add(item)
                    End If
                Next

                Return outEdges
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getInNB(ByVal i As Integer) As List(Of graphVertex) Implements Graph.getInNB
            Get
                Dim gVList As New List(Of graphVertex)
                Dim tmpGVL As New List(Of graphVertex)
                tmpGVL = MyClass.getVertexList

                For Each gE As graphEdge In edgeList
                    If gE.B = i Then
                        gVList.Add(tmpGVL.Item(gE.A))
                    End If
                Next

                Return gVList
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getOutNB(ByVal i As Integer) As List(Of graphVertex) Implements Graph.getOutNB
            Get
                Dim gVList As New List(Of graphVertex)
                Dim tmpGVL As New List(Of graphVertex)
                tmpGVL = MyClass.getVertexList

                For Each gE As graphEdge In edgeList
                    If gE.A = i Then
                        gVList.Add(tmpGVL.Item(gE.B))
                    End If
                Next

                Return gVList
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getEdgeList() As List(Of graphEdge) Implements Graph.getEdgeList
            Get
                Return Me.edgeList
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getVertexList() As List(Of graphVertex) Implements Graph.getVertexList
            Get
                Dim gVL As New graphVertexList()
                For Each gE In MyClass.edgeList
                    gVL.insert(gE)
                Next
                Return gVL.getVertexList
            End Get
        End Property

        ''' <inheritdoc/>
        Public Function union(ByVal G As Graph) As Integer Implements Graph.union
            Dim ret As Integer = 0
            For Each gE As graphEdge In G.getEdgeList()
                ret += MyClass.insert(gE)
            Next
            Return ret
        End Function

        ''' <inheritdoc/>
        Public Function insert(ByVal tmpVertex As graphVertex) As Integer Implements Graph.insert
            Dim ret As Integer = 0
            If Not tmpVertex.isValid() Then
                Return ret
            End If
            For Each gE In tmpVertex.outEdges
                ret += MyClass.insert(gE)
            Next
            Return ret
        End Function

        ''' <inheritdoc/>
        Public Function insert(ByVal tmpEdge As graphEdge) As Boolean Implements Graph.insert
            If Not tmpEdge.isValid() Then Return False
            Dim cursor As Integer
            cursor = Me.find(tmpEdge)
            If cursor < 0 Then
                cursor = cursor Xor -1
                Me.edgeList.Insert(cursor, tmpEdge)
                Return True
            End If
            Return False
        End Function

        ''' <inheritdoc/>
        Friend Sub add(ByVal gE As graphEdge) Implements Graph.add
            Me.edgeList.Add(gE)
        End Sub

        ''' <inheritdoc/>
        Friend Sub add(ByVal gV As graphVertex) Implements Graph.add
            Me.edgeList.AddRange(gV.outEdges)
        End Sub

        ''' <inheritdoc/>
        Public Sub clear() Implements Graph.clear
            MyClass.edgeList = New List(Of graphEdge)
        End Sub

        ''' <inheritdoc/>
        Public Function delete(ByVal index As Integer) As Boolean Implements Graph.delete
            If index >= 0 And index < MyClass.edgeCount Then
                MyClass.edgeList.RemoveAt(index)
                Return True
            End If
            Return False
        End Function

        ''' <inheritdoc/>
        Public Function delete(ByVal indices As List(Of Integer)) As Integer Implements Graph.delete
            Dim ret As Integer = 0
            indices.Sort()
            For i As Integer = indices.Count - 1 To 0 Step -1
                ret += MyClass.delete(indices.Item(i))
            Next
            Return ret
        End Function

        ''' <inheritdoc/>
        Public Function delete(ByVal gE As graphEdge) As Boolean Implements Graph.delete
            Dim index As Integer = MyClass.find(gE)
            Return MyClass.delete(index)
        End Function

        ''' <inheritdoc/>
        Function delete(ByVal gV As graphVertex) As Integer Implements Graph.delete
            Dim ret As Integer = 0
            For Each gE In MyClass.getOutEdges(gV.index)
                ret += MyClass.delete(gE)
            Next
            For Each gE In MyClass.getInEdges(gV.index)
                ret += MyClass.delete(gE)
            Next
            Return ret
        End Function

        ''' <inheritdoc/>
        Sub subGraph(ByVal indices As List(Of Integer)) Implements Graph.subGraph
            Dim tmpgEL As New graphEdgeList()
            For Each i As Integer In indices
                tmpgEL.insert(Me.Item(i))
            Next
            Me.clear()
            Me.edgeList = tmpgEL.getEdgeList
        End Sub

        ''' <inheritdoc/>
        Public Sub ensureUndirected() Implements Graph.ensureUndirected
            Dim tmpgEL As New List(Of graphEdge)

            For Each gE In MyClass.edgeList
                tmpgEL.Add(New graphEdge(gE.B, gE.A, gE.Cost))
            Next

            For Each gE In tmpgEL
                MyClass.insert(gE)
            Next
        End Sub

        ''' <inheritdoc/>
        Public Function simplify() As List(Of Integer) Implements Graph.simplify
            Dim indices As New List(Of Integer)
            Dim cursor As Integer
            Dim gE As graphEdge

            For Each gE In MyClass.getEdgeList
                cursor = indices.BinarySearch(gE.A)
                If cursor < 0 Then
                    cursor = cursor Xor -1
                    indices.Insert(cursor, gE.A)
                End If
                cursor = indices.BinarySearch(gE.B)
                If cursor < 0 Then
                    cursor = cursor Xor -1
                    indices.Insert(cursor, gE.B)
                End If
            Next

            For Each gE In MyClass.edgeList
                gE.remapVertex(indices)
            Next
            Return indices
        End Function

        ''' <inheritdoc/>
        Public Sub dualGraph() Implements Graph.dualGraph
            Dim dGraph As New graphEdgeList()
            Dim tmpE As graphEdge
            Dim cursor, eIndex As Integer
            Dim gEList As List(Of graphEdge) = Me.getEdgeList
            Dim gVList As List(Of graphVertex) = Me.getVertexList

            cursor = 0
            For Each gE As graphEdge In gEList
                For Each nb As Integer In gVList.Item(gE.B).neighbours
                    tmpE = New graphEdge(gE.B, nb)
                    eIndex = gEList.BinarySearch(tmpE, tmpE)
                    If eIndex <> cursor Then ' braucht man das ?
                        tmpE = New graphEdge(cursor, eIndex) ' wie kann man die kosten hinzufuegen ?
                        dGraph.insert(tmpE)
                    End If
                Next
                cursor += 1
            Next

            MyClass.edgeList = dGraph.edgeList
        End Sub

        ''' <inheritdoc/>
        Public Sub dualVertexGraph() Implements Graph.dualVertexGraph
            Dim dGraph As New graphEdgeList()
            Dim tmpE As graphEdge
            Dim cursor, eIndex As Integer
            Dim gVC As Integer = MyClass.vertexCount
            Dim gEList As List(Of graphEdge) = MyClass.getEdgeList
            Dim gVList As List(Of graphVertex) = MyClass.getVertexList

            cursor = 0
            For Each gE As graphEdge In gEList
                For Each nb As Integer In gVList.Item(gE.B).neighbours
                    tmpE = New graphEdge(gE.B, nb)
                    eIndex = gEList.BinarySearch(tmpE, tmpE)
                    If eIndex <> cursor Then ' braucht man das ?
                        tmpE = New graphEdge(gVC + cursor, gVC + eIndex) ' und kann man damit was machen
                        dGraph.insert(tmpE)
                        tmpE = New graphEdge(gVC + cursor, gE.B)
                        dGraph.insert(tmpE)
                        tmpE = New graphEdge(gE.A, gVC + cursor, 0)
                        dGraph.insert(tmpE)
                    End If
                Next
                cursor += 1
            Next

            MyClass.edgeList = dGraph.edgeList
        End Sub


        ' -------------------- Public --------------------------
        ''' <overloads>
        ''' Merge two GrapEdges.
        ''' </overloads>
        ''' <summary>
        ''' Merge two GrapEdges.
        ''' </summary>
        ''' <param name="gE1">GraphEdge 1</param>
        ''' <param name="gE2">GraphEdge 2</param>
        ''' <returns>Returns true if successful adding the new graphEdge, also if the resulting graphEdge allready exists within the graphEdgeList.</returns>
        ''' <remarks>Merges two GraphEdges and adds the new Edge to the List. Delets the original GraphEdges.</remarks>
        Public Function Merge(ByVal gE1 As graphEdge, _
                              ByVal gE2 As graphEdge) As Boolean
            Dim gE As New graphEdge(gE1)
            If gE.Merge(gE2) Then
                Me.delete(gE1)
                Me.delete(gE2)
                Me.insert(gE)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Retrieves a specific GraphEdge from a List
        ''' </summary>
        ''' <param name="gE"></param>
        ''' <returns>Returns the index of the GraphEdge within the List. If the GraphEdge isn't found returns a negative number.</returns>
        ''' <remarks>Uses BinarySearch to find the GraphEdge. List must be sorted.</remarks>
        Public Function find(ByVal gE As graphEdge) As Integer
            Return Me.edgeList.BinarySearch(gE, gE)
        End Function

        ''' <summary>
        ''' tries to seperate a GraphEdge into two.
        ''' </summary>
        ''' <param name="gE">GraphEdge to Split</param>
        ''' <param name="nI">Vertex to insert.</param>
        ''' <param name="t">Split at t</param>
        ''' <returns>Return true if succesful, false if the Edge doesn't exist.</returns>
        ''' <remarks>Splits a GraphEdge into two Edges. Removes the GraphEdge from the List and inserts the newly generated Edge.</remarks>
        Public Function seperateEdge(ByVal gE As graphEdge, ByVal nI As Integer, Optional ByVal t As Double = 0.5) As Boolean

            If Not Me.find(gE) Then
                Return False
            End If

            Dim SgE(2) As graphEdge
            SgE = gE.split(nI, t)

            Me.delete(gE)
            Me.insert(SgE(0))
            Me.insert(SgE(1))

            Return True
        End Function

        ''' <summary>
        ''' Retrieve a graphEdge from the graphEdgeList.
        ''' </summary>
        ''' <param name="i">index of the graphEdge</param>
        ''' <returns>Returns the specified graphEdge otherwise an inValid graphEdge.</returns>
        ''' <remarks></remarks>
        Public Function Item(ByVal i As Integer) As graphEdge
            If i >= 0 And i < Me.edgeCount Then
                Return Me.edgeList.Item(i)
            End If
            Return New graphEdge()
        End Function

        ''' <summary>
        ''' Flips the edges of a graph
        ''' </summary>
        ''' <param name="indices">List of indices to flip</param>
        Sub flipEdges(ByVal indices As List(Of Integer))
            Dim gE As graphEdge
            For Each i In indices
                gE = Me.Item(i)
                If gE.isValid Then
                    gE.Flip()
                End If
            Next
            Me.Sort()
        End Sub

        ''' <summary>
        ''' Flips the Graph
        ''' </summary>
        ''' <remarks></remarks>
        Sub flipGraph()
            Dim gE As graphEdge
            For Each gE In Me.edgeList
                gE.Flip()
            Next
            Me.Sort()
        End Sub

        ' -------------------- Protected --------------------------
        Protected Friend Sub Sort(ByVal comp As gEComp)
            Me.edgeList.Sort(comp)
        End Sub

        Protected Friend Sub Sort()
            Me.edgeList.Sort(New graphEdge())
        End Sub
    End Class
End Namespace