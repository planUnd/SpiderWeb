Option Explicit On

Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphRepresentaions

Namespace graphTools
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : BreadthFirstSearch
    ''' 
    ''' <summary>
    ''' Uses Vertex List representation to run "Breadth First Search".
    ''' Constructs a Graph with edges pointing to the predecessor of each vertex.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' [Richard Schaffranek]   03/03/2014 modified
    ''' [Richard Schaffranek]   09/05/2014 modified -> changed c to array
    ''' </history>
    ''' 
    Public MustInherit Class searchGraph
        Inherits graphVertexList
        Implements graphTree

        Protected c() As Double
        Protected root As Integer
        ' -------------------- Constructor --------------------------
        ''' <inheritdoc/>
        Public Sub New()
            MyBase.New()
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal G As Graph)
            MyBase.New(G)
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal tmpVertexList As List(Of graphVertex))
            MyBase.New(tmpVertexList)
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal tmpVertex As graphVertex)
            MyBase.New(tmpVertex)
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal vertices As Integer)
            MyBase.New(vertices)
            Me.initialize()
        End Sub

        ' -------------------- Property --------------------------
        ''' <inheritdoc/>
        ReadOnly Property rootIndex As Integer
            Get
                Return root
            End Get
        End Property

        ''' <inheritdoc/>
        Overloads ReadOnly Property split_tAt(ByVal t As Double) As Graph()
            Get
                Dim gEL(2) As Graph
                gEL(0) = New graphVertexList()
                gEL(1) = New graphVertexList()
                Dim sgE(2) As graphEdge
                Dim tmpT As Double
                Dim tmpVC As Integer = Me.vertexCount
                If Me.root >= 0 Then
                    For Each gE In Me.getEdgeList
                        If Me.c(gE.A) <= t And Me.c(gE.B) <= t Then ' wenn beide kleiner gleich
                            gEL(0).insert(gE)
                        ElseIf Me.c(gE.A) <= t And Me.c(gE.B) > t Then
                            tmpT = gE.tAt(Me.c(gE.A), Me.c(gE.B), t)
                            sgE = gE.split(tmpVC, tmpT)
                            gEL(0).insert(sgE(0))
                            gEL(1).insert(sgE(1))
                            tmpVC += 1
                        ElseIf Me.c(gE.A) > t Or Me.c(gE.B) > t Then ' wenn beide groesser
                            gEL(1).insert(gE)
                        End If
                    Next
                End If
                Return gEL
            End Get
        End Property

        ''' <inheritdoc/>
        Overloads ReadOnly Property tAt(ByVal t As Double) As List(Of Double)
            Get
                Dim tL As New List(Of Double)
                If Me.root >= 0 Then
                    For Each gE In Me.getEdgeList
                        tL.Add(gE.tAt(Me.c(gE.A), Me.c(gE.B), t))
                    Next
                End If
                Return tL
            End Get
        End Property

        ''' <inheritdoc/>
        Overloads ReadOnly Property graphEdge_tAt(ByVal t As Double) As List(Of graphEdge)
            Get
                Dim tgEL As New List(Of graphEdge)
                If Me.root >= 0 Then
                    For Each gE In Me.getEdgeList
                        If t < Me.c(gE.A) And Me.c(gE.B) > t Then
                            tgEL.Add(gE)
                        End If
                    Next
                End If
                Return tgEL
            End Get
        End Property

        ''' <inheritdoc/>
        Overloads ReadOnly Property graphEdgeIndex_tAt(ByVal t As Double) As List(Of Integer)
            Get
                Dim tgELi As New List(Of Integer)
                Dim index As Integer = 0
                If Me.root >= 0 Then
                    For Each gE In Me.getEdgeList
                        If t < Me.c(gE.A) And Me.c(gE.B) > t Then
                            tgELi.Add(index)
                        End If
                        index += 1
                    Next
                End If
                Return tgELi
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property dist As Double()
            Get
                Return Me.c
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property dist(ByVal i As Integer) As Double
            Get
                Return Me.c(i)
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property previous(ByVal i As Integer) As Integer()
            Get
                Return Me.Item(i).neighbours
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property gVLPath(ByVal EP As Integer) As graphVertexList
            Get
                If Me.root >= 0 And Not Double.IsPositiveInfinity(Me.c(EP)) Then
                    Dim retgVL As New graphVertexList()

                    Dim gV, gV2 As graphVertex
                    Dim iL As New List(Of graphVertex)
                    Dim nb As Integer
                    Dim c As Double

                    iL.Add(Me.Item(EP))

                    While iL.Count > 0
                        gV = iL.Item(0)
                        iL.RemoveAt(0)
                        For i As Integer = 0 To gV.outDegree - 1
                            nb = gV.neighbours(i)
                            c = gV.cost(i)
                            If nb <> EP Then
                                iL.Add(Me.Item(nb))
                                gV2 = New graphVertex(nb)
                                gV2.insert(gV.index, c)
                                retgVL.insert(gV2)
                            End If
                        Next
                    End While

                    Return retgVL
                End If
                Return New graphVertexList()
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property gELPath(ByVal EP As Integer) As graphEdgeList
            Get
                If Me.root >= 0 And Not Double.IsPositiveInfinity(Me.c(EP)) Then
                    Dim retgEL As New graphEdgeList()

                    Dim gV As graphVertex
                    Dim iL As New List(Of graphVertex)
                    Dim nb As Integer
                    Dim c As Double
                    iL.Add(Me.Item(EP))

                    While iL.Count > 0
                        gV = iL.Item(0)
                        iL.RemoveAt(0)
                        For i As Integer = 0 To gV.outDegree - 1
                            nb = gV.neighbours(i)
                            c = gV.cost(i)
                            If nb <> EP Then
                                iL.Add(Me.Item(nb))
                                retgEL.insert(New graphEdge(nb, gV.index, c))
                            End If
                        Next
                    End While

                    Return retgEL
                End If
                Return New graphEdgeList()
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property isValidTree As Boolean Implements graphTree.isValidTree
            Get
                Return (Me.vertexCount = Me.edgeCount + 1)
            End Get
        End Property


        ' -------------------- Protected --------------------------
        Protected Sub initialize()
            Me.root = -1
            ReDim Me.c(Me.vertexCount - 1)
            For i As Integer = 0 To Me.vertexCount - 1
                Me.c(i) = Double.PositiveInfinity
            Next
        End Sub

        ' -------------------- Public --------------------------

        ''' <inheritdoc/>
        Public MustOverride Sub find(ByVal SP As Integer, _
                            Optional ByVal tol As Double = 1)

        ''' <inheritdoc/>
        Public MustOverride Sub findALL(ByVal SP As Integer,
                           Optional ByVal tol As Double = 1)

    End Class

End Namespace