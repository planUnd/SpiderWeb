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
    ''' [Richard Schaffranek] 20/08/2014 created
    ''' [Richard Schaffranek] 16/11/2014 changed: meta.numerics -> math.net
    ''' [Richard Schaffranek] 02/07/2015 complete Rework -> trastic Speed improvment
    ''' </history>
    Public Class UPGMA
        Inherits agglomerativeClustering

        Protected distMSum As Matrix(Of Double)

        ''' <inheritdoc/>
        Public Sub New(ByVal rV() As Vector(Of Double))
            MyBase.New(rV)
            Me.initDistMatrix()
            Me.distMSum = Matrix(Of Double).Build.DenseOfMatrix(Me.distM)
        End Sub

        ''' <summary>
        ''' UPGMA Distance Function for Clustering
        ''' </summary>
        ''' <param name="cI">Index 1 of the cluster to calculate the distance from.</param>
        ''' <param name="cII">Index 1 of the cluster to calculate the distance from.</param>
        ''' <remarks>Calculated as described http://de.wikipedia.org/wiki/Hierarchische_Clusteranalyse#Agglomerative_Berechnung </remarks>
        Public Overrides Sub UpdateDist(ByVal cI As Integer, ByVal cII As Integer)
            Dim VI As Vector(Of Double) = Me.distMSum.Column(cI)
            Dim VII As Vector(Of Double) = Me.distMSum.Column(cII)

            VI += VII

            Me.distMSum.SetColumn(cI, VI)
            Me.distMSum.SetRow(cI, VI)
            Me.distMSum(cI, cI) = Double.PositiveInfinity

            Me.distMSum = Me.distMSum.RemoveColumn(cII)
            Me.distMSum = Me.distMSum.RemoveRow(cII)

            VI /= Me.indexClusterCount(cI)

            Me.distM.SetColumn(cI, VI)
            Me.distM.SetRow(cI, VI)

            Me.distM(cI, cI) = Double.PositiveInfinity

            Me.distM = Me.distM.RemoveColumn(cII)
            Me.distM = Me.distM.RemoveRow(cII)
        End Sub

    End Class
End Namespace
