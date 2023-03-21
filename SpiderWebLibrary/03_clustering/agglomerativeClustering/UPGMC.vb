Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : UPGMA
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
    Public Class UPGMC
        Inherits agglomerativeClustering

        ''' <summary>
        ''' vectorSum of the Clusters
        ''' </summary>
        ''' <remarks></remarks>
        Protected vectorSum As Matrix(Of Double)

        ''' <inheritdoc/>
        Public Sub New(ByVal rV() As Vector(Of Double))
            MyBase.New(rV)

            Me.vectorSum = Matrix(Of Double).Build.Dense(Me.rowVector(0).Count, rV.Count)
            For i As Integer = 0 To rV.Count - 1
                Me.vectorSum.SetColumn(i, rV(i))
            Next

            Me.initDistMatrix()
        End Sub

        ''' <summary>
        ''' Calculate the Centroid of the given Cluster
        ''' </summary>
        ''' <param name="i">0-based cluster index</param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property centroid(ByVal i As Integer) As Vector(Of Double)
            Get
                Return vectorSum.Column(i) / Me.indexClusterCount(i)
            End Get
        End Property


        ''' <summary>
        ''' UPGMC Distance Function for Clustering
        ''' </summary>
        ''' <param name="cI">Index 1 of the cluster to calculate the distance from.</param>
        ''' <param name="cII">Index 1 of the cluster to calculate the distance from.</param>
        ''' <remarks>Calculated as described http://de.wikipedia.org/wiki/Hierarchische_Clusteranalyse#Agglomerative_Berechnung </remarks>
        Public Overrides Sub updateDist(ByVal cI As Integer, ByVal cII As Integer)
            Dim cVI, cVII As Vector(Of Double)
            Dim tmpDist As Double

            Dim V1 As Vector(Of Double) = Me.vectorSum.Column(cI)
            Dim V2 As Vector(Of Double) = Me.vectorSum.Column(cII)

            Me.vectorSum.SetColumn(cI, (V1 + V2))
            Me.vectorSum = Me.vectorSum.RemoveColumn(cII)

            Me.distM = Me.distM.RemoveColumn(cII)
            Me.distM = Me.distM.RemoveRow(cII)

            For i As Integer = 0 To Me.count - 1
                cVI = Me.centroid(cI)
                cVII = Me.centroid(i)
                tmpDist = (cVI - cVII).L2Norm()
                Me.distM(i, cI) = tmpDist
                Me.distM(cI, i) = tmpDist
            Next

            Me.distM(cI, cI) = Double.PositiveInfinity

        End Sub

    End Class
End Namespace
