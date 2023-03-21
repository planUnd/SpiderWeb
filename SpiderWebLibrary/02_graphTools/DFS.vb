Option Explicit On

Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphRepresentaions

Namespace graphTools
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : DeapthFirstSearch
    ''' 
    ''' <summary>
    ''' Uses Vertex List representation to run "Depth First Search".
    ''' Constructs a Graph with edges pointing to the predecessor of each vertex.
    ''' uses searchGraph as a base class and overloads find / findAll methodes
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' [Richard Schaffranek]   03/03/2014 modified
    ''' </history>
    ''' 

    Public Class DFS
        Inherits searchGraph

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


        ' -------------------- Public --------------------------
        ' maybe revisit
        ''' <inheritdoc/>
        Public Overrides Sub find(ByVal SP As Integer, _
                            Optional ByVal tol As Double = 1)
            Me.initialize()
            Me.root = SP

            Dim pGVL As New graphVertexList(Me.vertexCount)
            Dim TOQ As New List(Of Integer)

            TOQ.Add(SP)
            Me.c(SP) = 0

            Dim cursor As Integer
            Dim level As Double

            While TOQ.Count > 0
                cursor = TOQ.Item(0)
                TOQ.RemoveAt(0)
                level = Me.c(cursor)
                For Each nb In Me.Item(cursor).neighbours
                    If Me.c(nb) > (level + 1) Then
                        Me.c(nb) = level + 1
                        pGVL.Item(nb).initialize()
                        pGVL.Item(nb).insert(cursor, 1)
                        TOQ.Insert(0, nb)
                        Exit For
                    End If
                Next

            End While

            Me.vertexList = pGVL.getVertexList
        End Sub

        ''' <inheritdoc/>
        Public Overrides Sub findALL(ByVal SP As Integer, _
                           Optional ByVal tol As Double = 1)

            Me.initialize()
            Me.root = SP

            Dim pGVL As New graphVertexList(Me.vertexCount)
            Dim TOQ As New List(Of Integer)

            TOQ.Add(SP)

            Me.c(SP) = 0

            Dim cursor As Integer
            Dim level As Double

            While TOQ.Count > 0
                cursor = TOQ.Item(0)
                TOQ.RemoveAt(0)
                level = Me.c(cursor)
                For Each nb In Me.Item(cursor).neighbours
                    If Me.c(nb) > (level + 1) Then
                        Me.c(nb) = level + 1
                        pGVL.Item(nb).initialize()
                        pGVL.Item(nb).insert(cursor, 1)
                        TOQ.Insert(0, nb)
                        Exit For
                    ElseIf Me.c(nb) = (level + 1) Then
                        pGVL.Item(nb).insert(cursor, 1)
                    End If
                Next


            End While

            Me.vertexList = pGVL.getVertexList
        End Sub


    End Class
End Namespace