Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : distanceClustering
    ''' 
    ''' <summary>
    ''' Clustering
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   24/08/2014 created
    '''  [Richard Schaffranek]   16/11/2014 changed: meta.numerics -> math.net
    ''' </history>
    ''' 
    Public MustInherit Class distanceClustering

        ''' <summary>
        ''' Vectors to cluster as RowVectors
        ''' </summary>
        ''' <remarks></remarks>
        Protected rV() As Vector(Of Double)

        ''' <summary>
        ''' Clusters of the RowVectors 
        ''' </summary>
        ''' <remarks></remarks>
        Protected m As List(Of List(Of Integer))

        ''' <summary>
        ''' Construct an distanceClustering Class
        ''' </summary>
        ''' <param name="rV">Set of RowVector to Cluster</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal rV() As Vector(Of Double))
            Me.rV = rV
            Me.m = New List(Of List(Of Integer))
        End Sub

        ''' <summary>
        ''' Returns the match of the specified RowVector index
        ''' </summary>
        ''' <param name="index">0-based RowVector index</param>
        ''' <returns>0-based cluster assignment</returns>
        ''' <remarks>Will return -1 if the index could not be found.</remarks>
        ReadOnly Property match(ByVal index As Integer) As Integer
            Get
                If index >= 0 And index < Me.rV.Length Then
                    For ii As Integer = 0 To Me.m.Count - 1
                        If Me.m.Item(ii).BinarySearch(index) >= 0 Then
                            Return ii
                        End If
                    Next
                End If
                Return -1
            End Get
        End Property

        ''' <summary>
        ''' The match of the all RowVector index()
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns an array, with an entery for each RowVector, indicating the clusters of the RowVector</returns>
        ''' <remarks></remarks>
        ReadOnly Property match() As Integer()
            Get
                Dim mL(Me.rV.Length - 1) As Integer
                Dim count As Integer = 0

                For Each L As List(Of Integer) In Me.m
                    For Each item As Integer In L
                        mL(item) = count
                    Next
                    count += 1
                Next

                Return mL
            End Get
        End Property

        ''' <summary>
        ''' Number of Clusters
        ''' </summary>
        ''' <value></value>
        ''' <returns>Number of Clusters</returns>
        ''' <remarks></remarks>
        ReadOnly Property count() As Integer
            Get
                Return Me.m.Count
            End Get
        End Property

        ''' <summary>
        ''' Get the indices of the RowVectors asigned to a cluster.
        ''' </summary>
        ''' <param name="i">0-based Cluster index</param>
        ''' <value></value>
        ''' <returns>An array of indices of RowVectors</returns>
        ''' <remarks></remarks>
        ReadOnly Property indexCluster(ByVal i As Integer) As Integer()
            Get
                Return Me.m.Item(i).ToArray()
            End Get
        End Property

        ''' <summary>
        ''' Get the RowVectors of a cluster.
        ''' </summary>
        ''' <param name="i">0-based Cluster index</param>
        ''' <value></value>
        ''' <returns>An array of RowVectors</returns>
        ''' <remarks></remarks>
        ReadOnly Property rowVectorCluster(ByVal i As Integer) As Vector(Of Double)()
            Get
                Dim ret(Me.m.Item(i).Count - 1) As Vector(Of Double)
                Dim count As Integer = 0
                For Each item As Integer In Me.m.Item(i)
                    ret(count) = Me.rV(item)
                    count += 1
                Next
                Return ret
            End Get
        End Property

        ''' <summary>
        ''' Inserts an index into a cluster
        ''' </summary>
        ''' <param name="s">index of the Cluster to insert into</param>
        ''' <param name="t">index of RowVector to insert into Cluster s</param>
        ''' <remarks></remarks>
        Public Overridable Sub insert(ByVal s As Integer, ByVal t As Integer)
            If s >= Me.count Or t >= Me.rV.Length Then
                Return
            End If
            Dim cursor As Integer
            cursor = Me.m.Item(s).BinarySearch(t)
            If cursor < 0 Then
                cursor = cursor Xor -1
                Me.m.Item(s).Insert(cursor, t)
            End If
        End Sub

        ''' <summary>
        ''' Cluster the RowVectors
        ''' </summary>
        ''' <param name="nr">Number of Clusters wanted.</param>
        ''' <remarks></remarks>
        Public Overridable Sub cluster(ByVal nr As Integer)
            Me.m = New List(Of List(Of Integer))
            For i As Integer = 0 To nr - 1
                Me.m.Add(New List(Of Integer))
                Me.m.Item(i).Add(i)
            Next

            Dim indexT, indexS As Integer
            Dim minDist, tmpDist As Double

            For i As Integer = nr To Me.rV.Length - 1
                minDist = Double.PositiveInfinity
                For ii As Integer = 0 To nr - 1
                    tmpDist = Me.dist(i, ii)
                    If tmpDist < minDist Then
                        minDist = tmpDist
                        indexT = i
                        indexS = ii
                    End If
                Next
                Me.insert(indexS, indexT)
            Next
        End Sub

        ''' <summary>
        ''' Distance Function for Clustering
        ''' </summary>
        ''' <param name="cI">Index of point1 to measure the distance from.</param>
        ''' <param name="cII">Index of point2 to measure the distance from.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function dist(ByVal cI As Integer, ByVal cII As Integer) As Double
            Return dist(Me.rV(cI), Me.rV(cII))
        End Function

        Public MustOverride Function dist(ByVal V1 As Vector(Of Double), ByVal V2 As Vector(Of Double)) As Double

    End Class
End Namespace