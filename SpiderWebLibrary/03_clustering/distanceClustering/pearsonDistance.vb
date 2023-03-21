Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : pearsonDistance
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
    Public Class pearsonDistance
        Inherits distanceClustering

        Protected s As Vector(Of Double)

        ''' <inheritdoc/>
        Public Sub New(ByVal rV() As Vector(Of Double))
            MyBase.New(rV)
            Dim mV As Vector(Of Double)
            mV = Vector(Of Double).Build.Dense(rV(0).Count)
            s = Vector(Of Double).Build.Dense(rV(0).Count)

            For i As Integer = 0 To mV.Count - 1
                For Each V As Vector(Of Double) In rV
                    mV(i) += V(i)
                Next
                mV(i) /= rV.Length
            Next

            For i As Integer = 0 To mV.Count - 1
                For Each V As Vector(Of Double) In rV
                    Me.s(i) += Math.Pow(V(i) - mV(i), 2)
                Next
                Me.s(i) *= 1 / (rV.Length - 1)
            Next

        End Sub

        ''' <summary>
        ''' Pearson Distance Function for Clustering
        ''' </summary>
        ''' <param name="V1">Vector to measure the distance from.</param>
        ''' <param name="V2">Vector to measure the distance to.</param>
        ''' <returns></returns>
        ''' <remarks>Calculated as described http://de.wikipedia.org/wiki/Hierarchische_Clusteranalyse#Distanz-_und_.C3.84hnlichkeitsma.C3.9Fe </remarks>
        Public Overrides Function dist(V1 As Vector(Of Double), V2 As Vector(Of Double)) As Double
            Dim ret As Double = 0
            Dim mV As Vector(Of Double) = (V1 - V2)
            For i As Integer = 0 To mV.Count - 1
                If Me.s(i) <> 0 Then
                    ret += Math.Pow(mV(i), 2) / Me.s(i)
                End If
            Next
            Return Math.Sqrt(ret)
        End Function

    End Class
End Namespace
