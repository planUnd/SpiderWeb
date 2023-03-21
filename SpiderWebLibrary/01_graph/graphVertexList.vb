Option Explicit On


Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports MathNet.Numerics.LinearAlgebra

Namespace graphRepresentaions
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : graphVertexList
    ''' 
    ''' <summary>
    ''' Vertex List representation of a Graph.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    '''  [Richard Schaffranek]   16/11/2014 changed: meta.numerics -> math.net
    ''' </history>
    ''' 

    Public Class graphVertexList
        Implements Graph

        Protected vertexList As List(Of graphVertex)

        ' -------------------- Constructor --------------------------

        ''' <summary>
        '''  Construct a empty Graph represented by a list of graphVertex.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Me.vertexList = New List(Of graphVertex)
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphVertex, from another Graph.
        ''' </summary>
        ''' <param name="G">Object, implementing the Graph interface.</param>
        ''' <remarks>Computes in O(n), where n is the number of graphVertex in the Graph.</remarks>
        Public Sub New(ByVal G As Graph)
            Me.vertexList = New List(Of graphVertex)
            For Each gV As graphVertex In G.getVertexList
                Me.add(New graphVertex(gV))
            Next
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphVertex, from a List(of graphVertex)
        ''' </summary>
        ''' <param name="tmpVertexList"></param>
        ''' <remarks>The List(of graphVertex) will be sorted  befor adding the vertices to the Graph. 
        ''' On Average this computes in O(n log n), where n is the number of graphVertex in the Graph.
        ''' If your are constructing the Graph from an existing Graph (G) use:
        ''' <code>
        ''' new graphVertex(G)
        ''' </code>
        ''' </remarks>
        Public Sub New(ByVal tmpVertexList As List(Of graphVertex))
            Me.vertexList = New List(Of graphVertex)
            Dim gV As New graphVertex()
            tmpVertexList.Sort(gV)
            For Each gV In tmpVertexList
                Me.add(New graphVertex(gV))
            Next
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphVertex and inserts the gV.
        ''' </summary>
        ''' <param name="gV"></param>
        ''' <remarks>Constructs a Graph where the vertexCount-1 = gV.index</remarks>
        Public Sub New(ByVal gV As graphVertex)
            Me.vertexList = New List(Of graphVertex)
            For i As Integer = 0 To gV.index - 1
                Me.add(New graphVertex(i))
            Next
            Me.add(New graphVertex(gV))
        End Sub

        ''' <summary>
        ''' Construct a Graph represented by a list of graphVertex.
        ''' </summary>
        ''' <param name="vertices"></param>
        ''' <remarks>Constructs a Graph where the vertexCount-1 = vertices</remarks>
        Public Sub New(ByVal vertices As Integer)
            Me.vertexList = New List(Of graphVertex)
            For i As Integer = 0 To vertices - 1
                Me.add(New graphVertex(i))
            Next
        End Sub

        ' -------------------- Overrides --------------------------
        Public Overrides Function ToString() As String
            Return "graphVertexList: " & Me.vertexCount & " vertices, " & Me.edgeCount & " edges"
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
                    For Each gE In Me.getEdgeList
                        tmpT = gE.tAt(costs.Item(gE.A), costs.Item(gE.B), t)
                        If tmpT > 1 Then
                            gEL(0).insert(gE)
                        ElseIf tmpT < 0 Then
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
                    For Each gE In Me.getEdgeList
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
                    For Each gE In Me.getEdgeList
                        If costs.Item(gE.A) < t And t < costs.Item(gE.B) Then
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
                    For Each gE In Me.getEdgeList
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
        ReadOnly Property negativeCost(Optional ByVal tol As Double = 1) As Boolean Implements Graph.negativeCost
            Get
                For Each gV In Me.vertexList
                    If gV.negativeCost(tol) Then
                        Return True
                    End If
                Next

                Return False
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property vertexCount As Integer Implements Graph.vertexCount
            Get
                Return Me.vertexList.Count
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property edgeCount() As Integer Implements Graph.edgeCount
            Get
                Dim eC As Integer = 0
                For Each gV As graphVertex In vertexList
                    eC += gV.outDegree
                Next
                Return eC
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getVertexList() As List(Of graphVertex) Implements Graph.getVertexList
            Get
                Return Me.vertexList
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getEdgeList() As List(Of graphEdge) Implements Graph.getEdgeList
            Get
                Dim gEL As New graphEdgeList()
                For Each gV As graphVertex In Me.vertexList
                    gEL.add(gV)
                Next
                Return gEL.getEdgeList
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getInEdges(i As Integer) As List(Of graphEdge) Implements Graph.getInEdges
            Get
                Dim gEL As New List(Of graphEdge)
                Dim gE As graphEdge
                Dim cursor As Double
                For Each gV As graphVertex In vertexList
                    cursor = gV.findNB(i)
                    If cursor >= 0 Then
                        gE = New graphEdge(gV.index, i, gV.cost(cursor))
                        cursor = gEL.BinarySearch(gE, gE)
                        If cursor < 0 Then
                            cursor = cursor Xor -1
                            gEL.Insert(cursor, gE)
                        End If
                    End If
                Next
                Return gEL
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getOutEdges(i As Integer) As List(Of graphEdge) Implements Graph.getOutEdges
            Get
                Dim gV As graphVertex
                gV = Me.Item(i)
                Return gV.outEdges()
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getInNB(ByVal i As Integer) As List(Of graphVertex) Implements Graph.getInNB
            Get
                Dim gVList As New List(Of graphVertex)
                For Each gV As graphVertex In vertexList
                    If gV.findNB(i) >= 0 Then
                        gVList.Add(gV)
                    End If
                Next
                Return gVList
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property getOutNB(ByVal i As Integer) As List(Of graphVertex) Implements Graph.getOutNB
            Get
                Dim gVList As New List(Of graphVertex)
                For Each index As Integer In Me.Item(i).neighbours
                    gVList.Add(Me.Item(index))
                Next
                Return gVList
            End Get
        End Property

        ''' <inheritdoc/>
        Public Function union(ByVal G As Graph) As Integer Implements Graph.union
            Dim ret As Integer = 0
            For Each gV In G.getVertexList()
                ret += Me.insert(gV)
            Next
            Return ret
        End Function

        ''' <inheritdoc/>
        Public Function insert(ByVal tmpVertex As graphVertex) As Integer Implements Graph.insert
            If Not tmpVertex.isValid() Then Return 0

            Dim ret As Integer = 0

            If tmpVertex.maxIndex >= Me.vertexCount Then
                For i As Integer = Me.vertexCount To tmpVertex.maxIndex
                    Me.vertexList.Add(New graphVertex(i))
                    ret += 1
                Next
            End If

            Me.vertexList.Item(tmpVertex.index).merge(tmpVertex)

            Return ret
        End Function

        ''' <inheritdoc/>
        Public Function insert(ByVal tmpEdge As graphEdge) As Boolean Implements Graph.insert
            Dim tmpGV As New graphVertex(tmpEdge)
            Return Me.insert(tmpGV)
        End Function

        ''' <inheritdoc/>
        Friend Sub add(ByVal gE As graphEdge) Implements Graph.add
            Me.vertexList.Add(New graphVertex(gE))
        End Sub

        ''' <inheritdoc/>
        Friend Sub add(ByVal gV As graphVertex) Implements Graph.add
            Me.vertexList.Add(gV)
        End Sub

        ''' <inheritdoc/>
        Public Sub clear() Implements Graph.clear
            Me.vertexList = New List(Of graphVertex)
        End Sub

        ''' <inheritdoc/>
        Public Function delete(ByVal index As Integer) As Boolean Implements Graph.delete
            If index < 0 Or index >= Me.vertexCount Then Return False

            Me.vertexList.Item(index) = New graphVertex(index)
            For Each gV As graphVertex In Me.vertexList
                gV.removeNB(index)
            Next

            Return True
        End Function

        ''' <inheritdoc/>
        Public Function delete(ByVal indices As List(Of Integer)) As Integer Implements Graph.delete
            Dim ret As Integer = 0
            For Each i As Integer In indices
                ret += Convert.ToInt32(Me.delete(i))
            Next
            Return ret
        End Function

        ''' <inheritdoc/>
        Public Function delete(ByVal gE As graphEdge) As Boolean Implements Graph.delete
            Dim gV As graphVertex = Me.Item(gE.A)
            If gV.isValid Then
                Return gV.removeNB(gE.B)
            End If
            Return False
        End Function

        ''' <inheritdoc/>
        Public Function delete(ByVal gV As graphVertex) As Integer Implements Graph.delete
            Return Me.delete(gV.index)
        End Function

        ''' <inheritdoc/>
        Sub subGraph(ByVal indices As List(Of Integer)) Implements Graph.subGraph
            Dim tmpgVL As New graphVertexList()
            For Each i As Integer In indices
                tmpgVL.insert(Me.Item(i))
            Next
            Me.clear()
            Me.vertexList = tmpgVL.getVertexList
        End Sub

        ''' <inheritdoc/>
        Public Sub ensureUndirected() Implements Graph.ensureUndirected
            Dim tmpGV As graphVertex
            Dim cursor As Integer
            For Each gV As graphVertex In Me.vertexList
                For i As Integer = 0 To gV.neighbours.Count - 1
                    cursor = gV.neighbours(i)
                    tmpGV = Me.Item(cursor)
                    tmpGV.insert(gV.index, gV.cost(i))
                Next
            Next
        End Sub

        ''' <inheritdoc/>
        Public Function simplify() As List(Of Integer) Implements Graph.simplify
            Dim indices As New List(Of Integer)
            Dim cursor As Integer
            Dim gV As graphVertex

            For i As Integer = Me.vertexCount - 1 To 0 Step -1
                If Me.getOutNB(i).Count > 0 Or Me.getInNB(i).Count > 0 Then
                    gV = Me.Item(i)
                    cursor = indices.BinarySearch(gV.index)
                    If cursor < 0 Then
                        cursor = cursor Xor -1
                        indices.Insert(cursor, gV.index)
                    End If
                    For Each nb In gV.neighbours
                        cursor = indices.BinarySearch(nb)
                        If cursor < 0 Then
                            cursor = cursor Xor -1
                            indices.Insert(cursor, nb)
                        End If
                    Next
                Else
                    Me.vertexList.RemoveAt(i)
                End If
            Next

            For Each gV In Me.getVertexList
                gV.remapVertex(indices)
            Next

            Return indices
        End Function

        ''' <inheritdoc/>
        Public Sub dualGraph() Implements Graph.dualGraph
            Dim dGraph As New graphVertexList()
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

            Me.vertexList = dGraph.vertexList
        End Sub

        ''' <inheritdoc/>
        Public Sub dualVertexGraph() Implements Graph.dualVertexGraph
            Dim dGraph As New graphVertexList()
            Dim tmpE As graphEdge
            Dim cursor, eIndex As Integer
            Dim gVC As Integer = Me.vertexCount
            Dim gEList As List(Of graphEdge) = Me.getEdgeList
            Dim gVList As List(Of graphVertex) = Me.getVertexList

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

            Me.vertexList = dGraph.vertexList
        End Sub

        ' -------------------- Function --------------------------
        ''' <summary>
        ''' Retrieve a graphVertex from the graphVertexList.
        ''' </summary>
        ''' <param name="i">index of the graphVertex</param>
        ''' <returns>Returns the specified graphVertex otherwise an inValid graphVertex.</returns>
        ''' <remarks></remarks>
        Public Function Item(ByVal i As Integer) As graphVertex
            If i >= 0 And i < Me.vertexCount Then
                Return Me.vertexList.Item(i)
            End If
            Return New graphVertex()
        End Function

        'tested
        ''' <summary>
        ''' Callcualtes the median neighbouring values for each graphVertex in the graphVertexList.
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns>Returns the median values.</returns>
        ''' <remarks></remarks>
        Public Function Median(ByVal Values As List(Of Double)) As List(Of Double)
            Dim retList As New List(Of Double)

            If Values.Count = Me.vertexCount Then
                For Each gV As graphVertex In Me.vertexList
                    retList.Add(gV.median(Values))
                Next
            End If
            Return retList
        End Function

        'tested
        ''' <summary>
        ''' Callcualtes the average neighbouring values for each graphVertex in the graphVertexList.
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns>Returns the average values</returns>
        ''' <remarks></remarks>
        Public Function Average(ByVal Values As List(Of Double)) As List(Of Double)
            Dim retList As New List(Of Double)

            If Values.Count = Me.vertexCount Then
                For Each gV As graphVertex In Me.vertexList
                    retList.Add(gV.average(Values))
                Next
            End If

            Return retList
        End Function

        ''' <summary>
        ''' Finds all Points of Interest of a graphVertexList. 
        ''' A graphVertex is a POI if the values of the neighbours are larger or equals to the value of the graphVertex. 
        ''' </summary>
        ''' <param name="Values">The number of elements has to be the same as the number graphVertex in the graphVertexList.</param>
        ''' <returns>Returns the indices of the POI as List(of Integer).</returns>
        ''' <remarks></remarks>
        Public Function POI(ByVal Values As List(Of Double)) As List(Of Integer)
            Dim retList As New List(Of Integer)

            If Values.Count = Me.vertexCount Then
                For Each gV As graphVertex In Me.vertexList
                    If gV.isPOI(Values) Then
                        retList.Add(gV.index)
                    End If
                Next
            End If

            Return retList
        End Function

        ' -------------------- Protected --------------------------
        Protected Friend Sub Sort(ByVal comp As gVComp)
            Me.vertexList.Sort(comp)
        End Sub

        Protected Friend Sub Sort()
            Me.vertexList.Sort(New graphVertex())
        End Sub


    End Class
End Namespace

