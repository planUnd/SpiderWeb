Option Explicit On

Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphRepresentaions


Namespace graphTools
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : graphColoring
    ''' 
    ''' <summary>
    ''' Uses Vertex List representation to run apply graph coloring.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' [Richard Schaffranek]   26/03/2014 add SGC methode, changed rndSGC methode
    ''' [Richard Schaffranek]   09/05/2014 modified -> changed c to array
    ''' </history>
    ''' 
    Public Class graphColoring
        Inherits graphVertexList

        ''' <summary>
        ''' An Array of Integer() storing the color data
        ''' </summary>
        ''' <remarks></remarks>
        Private c As Integer()
        Private cC As Integer

        ' -------------------- Constructor --------------------------
        ''' <inheritdoc/>
        Public Sub New()
            MyBase.New()
            Me.initialize()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="G"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal G As graphColoring)
            MyBase.New(G)
            ReDim Me.c(G.vertexCount - 1)
            Array.Copy(G.colors, Me.c, G.vertexCount - 1)
            Me.cC = G.colorCount
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property colors As Integer()
            Get
                Return Me.c
            End Get
            Set(value As Integer())
                If value.Count = Me.vertexCount Then
                    Array.Copy(value, Me.c, Me.vertexCount)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Get/Set the color of a graphVertex
        ''' </summary>
        ''' <param name="i">index of the graphVertex to get/set the color.</param>
        ''' <value>Assign a precoloring to a graphVertex.</value>
        ''' <returns>Returns the coloring of a graphVertex.</returns>
        ''' <remarks>-1 will mark a graphVertex as uncolored / or not found</remarks>
        Property color(i As Integer) As Integer
            Get
                If i < Me.vertexCount Then
                    Return Me.c(i)
                End If
                Return -1
            End Get
            Set(value As Integer)
                If i < Me.vertexCount Then
                    Me.c(i) = value
                End If
            End Set
        End Property

        ReadOnly Property unColored() As List(Of graphVertex)
            Get
                Dim unvisited As New List(Of graphVertex)
                For i As Integer = 0 To Me.vertexCount - 1
                    If Me.color(i) = -1 Then
                        unvisited.Add(Me.Item(i))
                    End If
                Next
                Return unvisited
            End Get
        End Property

        ''' <summary>
        ''' Count the number of colors within the Graph.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property colorCount As Integer
            Get
                Return Me.cC
            End Get
        End Property

        ' -------------------- Private --------------------------
        Private Sub initialize()
            Me.cC = 0
            ReDim Me.c(Me.vertexCount - 1)
            For i As Integer = 0 To Me.vertexCount - 1
                Me.c(i) = -1
            Next
        End Sub

        ' -------------------- Public --------------------------

        Public Function sameColor(ByVal gV As graphVertex, ByVal curr As Integer) As Boolean
            For Each n As Integer In gV.neighbours
                If Me.color(n) = curr And n <> gV.index Then
                    Return True
                End If
            Next
            Return False
        End Function


        ''' <summary>
        ''' Use randomized sequential Graph to try to color the Graph with the targeted number of colors.
        ''' </summary>
        ''' <param name="target">Number of colors to color the Graph. Set to 2 for the least amount of colors.</param>
        ''' <param name="iterations">Maximum number of itterations to try to color the Graph.</param>
        ''' <remarks>Increase amount of itterations to find a better solution.</remarks>
        Public Sub rndSGC(Optional ByVal target As Integer = 3, _
                               Optional ByVal iterations As Integer = 100)
            Dim tmpG As graphColoring
            Dim bestG As New graphColoring(Me)
            Dim rnd As New Random()

            bestG.SGC(New gVrndComp(rnd.Next()))

            For i As Integer = 0 To iterations - 1
                tmpG = New graphColoring(Me)
                tmpG.SGC(New gVrndComp(rnd.Next()))
                If tmpG.cC < bestG.cC Then
                    bestG.cC = tmpG.colorCount
                    bestG.colors = tmpG.colors
                End If
                If tmpG.cC <= target Then
                    Exit For
                End If
            Next

            Me.cC = bestG.colorCount
            Me.colors = bestG.colors
        End Sub

        Public Sub SGC(ByVal comp As gVComp)
            Dim currColor As Integer = 0
            Dim gV As graphVertex
            Dim unVisited As List(Of graphVertex) = Me.unColored

            unVisited.Sort(comp)

            'Start coloring
            While (unVisited.Count > 0)
                For i As Integer = unVisited.Count - 1 To 0 Step -1
                    gV = unVisited.Item(i)
                    If Not Me.sameColor(gV, currColor) Then
                        Me.color(gV.index) = currColor
                        unVisited.RemoveAt(i)
                    End If
                Next
                currColor += 1
            End While
            Me.cC = currColor
        End Sub

    End Class
End Namespace