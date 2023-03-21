Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : completeLinkage
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
    Public Class completeLinkage
        Inherits agglomerativeClustering

        ''' <inheritdoc/>
        Public Sub New(ByVal rV() As Vector(Of Double))
            MyBase.New(rV)
            Me.initDistMatrix()
        End Sub

        ''' <summary>
        ''' CompleteLinkage Distance Function for Clustering
        ''' </summary>
        ''' <param name="cI">Index 1 of the cluster to calculate the distance from.</param>
        ''' <param name="cII">Index 1 of the cluster to calculate the distance from.</param>
        ''' <remarks>Calculated as described http://de.wikipedia.org/wiki/Hierarchische_Clusteranalyse#Agglomerative_Berechnung </remarks>
        Public Overrides Sub UpdateDist(ByVal cI As Integer, ByVal cII As Integer)
            Dim VI As Vector(Of Double) = Me.distM.Column(cI)
            Dim VII As Vector(Of Double) = Me.distM.Column(cII)

            For i As Integer = 0 To VI.Count - 1
                VI(i) = Math.Max(VI(i), VII(i))
            Next

            Me.distM.SetColumn(cI, VI)
            Me.distM.SetRow(cI, VI)

            Me.distM(cI, cI) = Double.PositiveInfinity

            Me.distM = Me.distM.RemoveColumn(cII)
            Me.distM = Me.distM.RemoveRow(cII)
        End Sub

    End Class
End Namespace
