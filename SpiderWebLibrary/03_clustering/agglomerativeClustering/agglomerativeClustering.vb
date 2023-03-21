Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : agglomerativeClustering
    ''' 
    ''' <summary>
    ''' Clustering
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   20/08/2014 created
    '''  [Richard Schaffranek]   16/11/2014 changed: meta.numerics -> math.net
    ''' [Richard Schaffranek] 02/07/2015 complete Rework -> trastic Speed improvment
    ''' </history>
    ''' 
    Public MustInherit Class agglomerativeClustering

        ''' <summary>
        ''' Vectors to cluster as RowVectors
        ''' </summary>
        ''' <remarks></remarks>
        Protected rV() As Vector(Of Double)

        Public matchM As Matrix(Of Double)

        Public distM As Matrix(Of Double)

        ''' <summary>
        ''' Construct an agglomerativeClustering Class
        ''' </summary>
        ''' <param name="rV">Set of RowVector to Cluster</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal rV() As Vector(Of Double))
            Me.rV = rV
            Me.matchM = Matrix(Of Double).Build.DenseDiagonal(rV.Count, 1)
            Me.distM = Matrix(Of Double).Build.Dense(rV.Count, rV.Count, Double.PositiveInfinity)


        End Sub

        ''' <summary>
        ''' The match of the specified RowVector index
        ''' </summary>
        ''' <param name="index">0-based RowVector index</param>
        ''' <returns>0-based cluster assignment</returns>
        ''' <remarks></remarks>
        ReadOnly Property match(ByVal index As Integer) As Integer
            Get
                Return Me.matchM.Row(index).MaximumIndex()
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
                Dim mL(Me.matchM.RowCount - 1) As Integer

                For i As Integer = 0 To Me.matchM.RowCount - 1
                    mL(i) = Me.match(i)
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
                Return Me.matchM.ColumnCount
            End Get
        End Property


        ReadOnly Property indexClusterCount(ByVal i As Integer) As Integer
            Get
                Return Me.matchM.Column(i).Sum
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
                Dim mC(Me.matchM.RowCount - 1) As Integer
                Dim count As Integer = 0
                For index As Integer = 0 To Me.matchM.RowCount - 1
                    If Me.matchM.Column(i)(index) = 1 Then
                        mC(count) = index
                        count += 1
                    End If
                Next
                Array.Resize(mC, count)
                Return mC
            End Get
        End Property

        ReadOnly Property rowVector(ByVal i As Integer) As Vector(Of Double)
            Get
                Return Me.rV(i)
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
                Dim mC() As Integer = Me.indexCluster(i)
                Dim ret(mC.Length - 1) As Vector(Of Double)
                For index As Integer = 0 To mC.Length - 1
                    ret(index) = Me.rV(mC(index))
                Next
                Return ret
            End Get
        End Property

        Protected Friend Sub initDistMatrix()
            Dim tmpDist As Double
            For i As Integer = 0 To Me.distM.ColumnCount - 1
                For ii As Integer = i + 1 To Me.distM.ColumnCount - 1
                    tmpDist = (Me.rowVector(i) - Me.rowVector(ii)).L2Norm
                    Me.distM(i, ii) = tmpDist
                    Me.distM(ii, i) = tmpDist
                Next
            Next
        End Sub

        ''' <summary>
        ''' Merge two Clusters
        ''' </summary>
        ''' <param name="cI">Index 1 of the cluster to merge.</param>
        ''' <param name="cII">Index 2 of the cluster to merge.</param>
        ''' <remarks>Will merge the cluster with the larger index into the cluster with the smaller index.</remarks>
        Public Overridable Sub merge(ByVal cI As Integer, ByVal cII As Integer)
            If cI > cII Then
                Dim tmp As Integer = cI
                cI = cII
                cII = tmp
            End If

            Dim V1 As Vector(Of Double) = Me.matchM.Column(cI)
            Dim V2 As Vector(Of Double) = Me.matchM.Column(cII)

            Me.matchM.SetColumn(cI, (V1 + V2))
            Me.matchM = Me.matchM.RemoveColumn(cII)

            UpdateDist(cI, cII)

        End Sub


        ''' <summary>
        ''' Cluster the RowVectors
        ''' </summary>
        ''' <param name="nr">Number of Clusters wanted.</param>
        ''' <param name="fixed">Number of Cluster that shall stay fixed. Will keep the first n Clusters fixed.</param>
        ''' <remarks></remarks>
        Public Overridable Sub cluster(ByVal nr As Integer, Optional ByVal fixed As Integer = 0)
            fixed = Math.Min(nr, fixed)

            'fixed not implementet....

            Dim cI, cII As Integer
            Dim minDist As Double
            While (Me.count > nr)
                minDist = Double.MaxValue
                For i As Integer = fixed To Me.count - 1
                    If Me.distM.Row(i).Minimum() < minDist Then
                        minDist = Me.distM.Row(i).Minimum()
                        cI = i
                        cII = Me.distM.Row(i).MinimumIndex()
                    End If
                Next
                Me.merge(cI, cII)
            End While
        End Sub

        ''' <summary>
        ''' Distance Function for Clustering
        ''' </summary>
        ''' <param name="cI">Index 1 of the cluster to calculate the distance from.</param>
        ''' <param name="cII">Index 2 of the cluster to calculate the distance from.</param>
        ''' <remarks></remarks>
        Public MustOverride Sub UpdateDist(ByVal cI As Integer, ByVal cII As Integer)

    End Class
End Namespace