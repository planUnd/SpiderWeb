Option Explicit On

Imports Grasshopper.Kernel.Types

Namespace GH_compare
    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : GH_compInteger
    ''' 
    ''' <summary>
    ''' Comperator for GH_Integer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Class GH_compInteger
        Implements IComparer(Of GH_Integer)

        ''' <summary>
        ''' Compare two GH_Integer
        ''' </summary>
        ''' <param name="V1">GH_Integer to compare</param>
        ''' <param name="V2">GH_Integer to compare</param>
        ''' <returns>-1 if V1 is smaller than V2, 0 if they are equal, 1 if V2 is smaller than V1.</returns>
        ''' <remarks></remarks>
        Public Function Compare(ByVal V1 As GH_Integer, _
                                ByVal V2 As GH_Integer) As Integer _
                                Implements IComparer(Of GH_Integer).Compare

            If Not V1.IsValid Then
                If Not V2.IsValid Then
                    Return 0
                Else
                    Return -1
                End If
            Else
                If Not V2.IsValid Then
                    Return 1
                Else
                    If V1.Value < V2.Value Then
                        Return -1
                    ElseIf V1.Value = V2.Value Then
                        Return 0
                    Else
                        Return 1
                    End If
                End If
            End If
        End Function

    End Class
End Namespace