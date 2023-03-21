Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : gowerDistance
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
    Public Class gowerDistance
        Inherits distanceClustering

        Private r As Vector(Of Double)

        ''' <inheritdoc/>
        Public Sub New(ByVal rV() As Vector(Of Double))
            MyBase.New(rV)
            Dim rMin As Vector(Of Double)
            Dim rMax As Vector(Of Double)
            rMin = Vector(Of Double).Build.Dense(rV(0).Count)
            rMax = Vector(Of Double).Build.Dense(rV(0).Count)
            For i As Integer = 0 To rMin.Count - 1
                rMin(i) = Double.PositiveInfinity
                rMax(i) = 0
            Next
            For Each V As Vector(Of Double) In rV
                For i As Integer = 0 To V.Count - 1
                    rMin(i) = Math.Min(rMin(i), V(i))
                    rMax(i) = Math.Max(rMax(i), V(i))
                Next
            Next

            Me.r = rMax - rMin
        End Sub

        ''' <summary>
        ''' Gower Distance Function for Clustering
        ''' </summary>
        ''' <param name="V1">Vector to measure the distance from.</param>
        ''' <param name="V2">Vector to measure the distance to.</param>
        ''' <returns></returns>
        ''' <remarks>Calculated as described http://de.wikipedia.org/wiki/Hierarchische_Clusteranalyse#Distanz-_und_.C3.84hnlichkeitsma.C3.9Fe </remarks>
        Public Overrides Function dist(V1 As Vector(Of Double), V2 As Vector(Of Double)) As Double
            Dim ret As Double = 0
            Dim mV As Vector(Of Double) = (V1 - V2)
            For i As Integer = 0 To mV.Count - 1
                If Me.r(i) <> 0 Then
                    ret += Math.Abs(mV(i)) / Me.r(i)
                End If
            Next
            Return ret
        End Function

    End Class
End Namespace
