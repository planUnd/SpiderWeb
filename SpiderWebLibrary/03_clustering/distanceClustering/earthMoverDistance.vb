Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : earthMoverDistance
    ''' 
    ''' <summary>
    ''' Clustering
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   10/06/2015 created
    ''' </history>
    Public Class earthMoverDistance
        Inherits distanceClustering

        ''' <inheritdoc/>
        Public Sub New(ByVal rV() As Vector(Of Double))
            MyBase.New(rV)
        End Sub

        ''' <summary>
        ''' chiSquared Distance Function for Clustering
        ''' </summary>
        ''' <param name="V1">Vector to measure the distance from.</param>
        ''' <param name="V2">Vector to measure the distance to.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function dist(V1 As Vector(Of Double), V2 As Vector(Of Double)) As Double
            Dim result As Double = 0
            Dim EMD As Double = 0

            For i As Integer = 0 To V1.Count - 2
                EMD = (V1(i) + EMD) - V2(i)
                result += Math.Abs(EMD)
            Next

            Return result
        End Function

    End Class
End Namespace
