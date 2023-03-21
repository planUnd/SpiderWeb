Option Explicit On


Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphRepresentaions

Namespace graphTools
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : minSpanningTree
    ''' 
    ''' <summary>
    ''' Uses Edge List representation to compute the minSpanningTree.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' [Richard Schaffranek]   26/03/2014 changed computeTree methode
    ''' </history>
    ''' 
    Public Class minSpanningTree
        Inherits graphEdgeList
        Implements graphTree

        ' -------------------- Constructor --------------------------
        ''' <inheritdoc/>
        Public Sub New()
            MyBase.New()
            MyClass.setup()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal G As Graph)
            MyBase.New(G)
            MyClass.setup()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal gE As graphEdge)
            MyBase.New(gE)
            MyClass.setup()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal tmpEdgeList As List(Of graphEdge))
            MyBase.New(tmpEdgeList)
            MyClass.setup()
        End Sub

        ' -------------------- Private --------------------------
        Private Sub setup()
            MyClass.ensureUndirected()
        End Sub

        Private Function createsCircle(ByVal E As graphEdge, ByRef Circles As List(Of List(Of Integer))) As Boolean
            Dim C As List(Of Integer)

            Dim c1, c2 As Integer

            c1 = -1
            c2 = -1

            For i As Integer = 0 To Circles.Count - 1
                C = Circles.Item(i)
                If C.Contains(E.A) And C.Contains(E.B) Then
                    Return True
                ElseIf C.Contains(E.A) Then
                    c1 = i
                ElseIf C.Contains(E.B) Then
                    c2 = i
                End If
                If c1 <> -1 And c2 <> -1 Then
                    i = Circles.Count
                End If
            Next

            If c1 <> -1 And c2 <> -1 Then
                Circles.Item(c1).AddRange(Circles.Item(c2))
                Circles.RemoveAt(c2)
            ElseIf c1 <> -1 Then
                Circles.Item(c1).Add(E.B)
            ElseIf c2 <> -1 Then
                Circles.Item(c2).Add(E.A)
            Else
                C = New List(Of Integer)
                C.Add(E.A)
                C.Add(E.B)
                Circles.Add(C)
            End If

            Return False
        End Function

        ' -------------------- Public --------------------------
        ''' <summary>
        ''' Computes the minimal Spanning Tree of the Graph.
        ''' </summary>
        ''' <param name="comp">Comparater determining the sorting order of the graphEdges. Use gEminCost to the minimalSpanningTree</param>
        ''' <remarks>Turns the Graph into an undirected Graph at first.</remarks>
        Public Sub computeTree(ByVal comp As gEComp)
            Dim minSPTree As New graphEdgeList()
            Dim E As graphEdge
            Dim Circles As New List(Of List(Of Integer))

            Me.Sort(comp)

            While MyClass.edgeCount > 0
                E = MyClass.Item(0)
                MyClass.delete(0)
                If Not createsCircle(E, Circles) Then
                    minSPTree.insert(E)
                End If
            End While

            Me.Sort()

            minSPTree.ensureUndirected()
            Me.edgeList = minSPTree.getEdgeList
        End Sub

        ' -------------------- graphTree --------------------------
        ''' <inheritdoc/>
        ReadOnly Property isValidTree As Boolean Implements graphTree.isValidTree
            Get
                Return (MyClass.vertexCount = MyClass.edgeCount / 2 + 1)
            End Get
        End Property
    End Class
End Namespace