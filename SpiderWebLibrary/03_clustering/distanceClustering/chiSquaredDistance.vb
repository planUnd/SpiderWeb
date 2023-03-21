Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : chiSquaredDistance
    ''' 
    ''' <summary>
    ''' Clustering
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   10/06/2015 created
    ''' </history>
    Public Class chiSquaredDistance
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
        Public Overrides Function dist(ByVal V1 As Vector(Of Double), ByVal V2 As Vector(Of Double)) As Double
            Dim result As Double = 0
            Dim nenner As Double
            For i As Integer = 0 To V1.Count - 1
                nenner = (V1(i) + V2(i))
                If nenner <> 0 Then
                    result = Math.Pow((V1(i) - V2(i)), 2) / nenner
                End If
            Next
            Return result / 2
        End Function

    End Class
End Namespace
