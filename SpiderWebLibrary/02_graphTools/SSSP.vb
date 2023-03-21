Option Explicit On


Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphRepresentaions

Namespace graphTools
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : SingleSourceShortestPath
    ''' 
    ''' <summary>
    ''' Uses Vertex List representation to compute the Single Source Shortest Path.
    ''' Constructs a Graph with edges pointing to the predecessor of each vertex.
    ''' Uses searchGraph as a base class and overloads find / findAll methodes
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Class SSSP
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
        ''' <inheritdoc/>
        Public Overrides Sub find(ByVal SP As Integer, Optional ByVal tol As Double = 1)

            Me.initialize()
            Me.root = SP

            Dim pGVL As New graphVertexList(Me.vertexCount)

            Dim Selected As New List(Of Integer)
            Dim nbL As Integer()
            Dim cL As Double()
            Dim cursor, tempNB As Integer
            Dim currDist, tempDist, tempValue, tempCurrValue As Double

            cursor = SP

            ' set the starting point
            ' pGVL.Item(cursor).insert(cursor)
            Me.c(cursor) = 0

            ' circle through the network...
            While (cursor <> -1)

                currDist = Me.c(cursor)
                nbL = Me.Item(cursor).neighbours
                cL = Me.Item(cursor).cost

                For i As Int32 = 0 To nbL.Count - 1
                    tempNB = nbL(i)
                    tempDist = cL(i)

                    tempValue = Math.Round(Me.c(tempNB) / tol)
                    tempCurrValue = Math.Round((currDist + tempDist) / tol)

                    If tempValue > tempCurrValue Then
                        pGVL.Item(tempNB).initialize()
                        Me.c(tempNB) = (tempCurrValue * tol)
                        pGVL.Item(tempNB).insert(cursor, tempDist)
                        Selected.Add(tempNB)
                    ElseIf tempValue > currDist / tol And Double.IsPositiveInfinity(tempDist) Then
                        pGVL.Item(tempNB).initialize()
                        Me.c(tempNB) = currDist
                        pGVL.Item(tempNB).insert(cursor, tempDist)
                    End If
                Next

                If (Selected.Count > 0) Then
                    cursor = Selected.Item(0)
                    Selected.RemoveAt(0)
                Else
                    cursor = -1
                End If
            End While


            Me.vertexList = pGVL.getVertexList
        End Sub
        ''' <inheritdoc/>
        Public Overrides Sub findALL(ByVal SP As Integer, Optional ByVal tol As Double = 1)

            Me.initialize()
            Me.root = SP

            Dim pGVL As New graphVertexList(Me.vertexCount)

            Dim Selected As New List(Of Integer)
            Dim nbL As Integer()
            Dim cL As Double()
            Dim cursor, tempNB As Integer
            Dim currDist, tempDist, tempValue, tempCurrValue As Double

            cursor = SP

            ' set the starting point
            ' pGVL.Item(cursor).insert(cursor)
            Me.c(cursor) = 0

            ' circle through the network...
            While (cursor <> -1)
                currDist = Me.c(cursor)
                nbL = Me.Item(cursor).neighbours
                cL = Me.Item(cursor).cost

                For i As Int32 = 0 To nbL.Count - 1
                    tempNB = nbL(i)
                    tempDist = cL(i)

                    tempValue = Math.Round(Me.c(tempNB) / tol)
                    tempCurrValue = Math.Round((currDist + tempDist) / tol)

                    If tempValue > tempCurrValue Then
                        pGVL.Item(tempNB).initialize()
                        Me.c(tempNB) = (tempCurrValue * tol)
                        pGVL.Item(tempNB).insert(cursor, tempDist)

                        Selected.Add(tempNB)
                    ElseIf tempValue > currDist / tol And Double.IsPositiveInfinity(tempDist) Then
                        pGVL.Item(tempNB).initialize()
                        Me.c(tempNB) = currDist
                        pGVL.Item(tempNB).insert(cursor, tempDist)

                    ElseIf tempValue = tempCurrValue Or _
                        tempValue = currDist / tol And Double.IsPositiveInfinity(tempDist) Then
                        If pGVL.Item(tempNB).findNB(SP) < 0 And tempNB <> SP Then ' dies sollte verhindern das wenn ein weg zum anfang gefunden ist neue wege hinzukommen
                            pGVL.Item(tempNB).insert(cursor, tempDist)
                        End If
                    End If

                Next

                If (Selected.Count > 0) Then
                    cursor = Selected.Item(0)
                    Selected.RemoveAt(0)
                Else
                    cursor = -1
                End If
            End While

            Me.vertexList = pGVL.getVertexList
        End Sub
    End Class
End Namespace