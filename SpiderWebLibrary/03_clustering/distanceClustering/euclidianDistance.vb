Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : euclidianDistance
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
    Public Class euclidianDistance
        Inherits distanceClustering

        ''' <inheritdoc/>
        Public Sub New(ByVal rV() As Vector(Of Double))
            MyBase.New(rV)
        End Sub

        ''' <summary>
        ''' Euclidian Distance Function for Clustering
        ''' </summary>
        ''' <param name="V1">Vector to measure the distance from.</param>
        ''' <param name="V2">Vector to measure the distance to.</param>
        ''' <returns></returns>
        ''' <remarks>Calculated as described http://de.wikipedia.org/wiki/Hierarchische_Clusteranalyse#Distanz-_und_.C3.84hnlichkeitsma.C3.9Fe </remarks>
        Public Overrides Function dist(V1 As Vector(Of Double), V2 As Vector(Of Double)) As Double
            Return (V1 - V2).L2Norm
        End Function

    End Class
End Namespace
