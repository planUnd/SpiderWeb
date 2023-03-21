Option Explicit On

Imports MathNet.Numerics.LinearAlgebra

Namespace clustering
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : implement Hyperplan LSH 
    ''' 
    ''' <summary>
    ''' Munkers
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   18/05/2018 created
    ''' </history>
    Public Class hyperplaneHashing

        ''' <summary>
        ''' hyperPlaneNormals as Vector
        ''' </summary>
        ''' <remarks></remarks>
        Private hPN() As Vector(Of Double)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dimension"></param>
        ''' <param name="count"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal dimension As Integer, ByVal count As Integer)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dimension"></param>
        ''' <param name="count"></param>
        ''' <param name="seed"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal dimension As Integer, ByVal count As Integer, ByVal seed As Integer)

        End Sub


        Public Function getHash(ByVal V As Vector(Of Double)) As Integer

        End Function

    End Class
End Namespace
