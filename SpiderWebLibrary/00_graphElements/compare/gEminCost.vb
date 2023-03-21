Option Explicit On

Namespace graphElements
    Namespace compare

        ''' -------------------------------------------
        ''' Project : SpiderWebLibrary
        ''' Class : gEminCost
        ''' 
        ''' <summary>
        ''' Comperator to sort a list of graphEdges asscending based on their cost values.
        ''' </summary>
        ''' <remarks></remarks>
        ''' <history>
        ''' [Richard Schaffranek]   22/11/2013 created
        ''' </history>

        Public Class gEminCost
            Implements gEComp

            ''' <inheritdoc/>
            Public Function Compare(ByVal E1 As graphEdge, ByVal E2 As graphEdge) As Integer Implements System.Collections.Generic.IComparer(Of graphEdge).Compare
                If Not E1.isValid Then
                    If Not E2.isValid Then
                        Return 0
                    Else
                        Return -1
                    End If
                Else
                    If Not E2.isValid Then
                        Return 1
                    Else
                       If E1.Cost > E2.Cost Then
                            Return 1
                        ElseIf E2.Cost > E1.Cost Then
                            Return -1
                        Else
                            Return 0
                        End If
                End If
                End If
            End Function

        End Class
    End Namespace
End Namespace