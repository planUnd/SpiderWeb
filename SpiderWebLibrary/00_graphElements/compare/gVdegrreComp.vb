Option Explicit On


Namespace graphElements
    Namespace compare
        ''' -------------------------------------------
        ''' Project : SpiderWebLibrary
        ''' Class : gVdegrreComp
        ''' 
        ''' <summary>
        ''' Comperator to sort a list of graphVertex based on the graphVertex.outDegree.
        ''' </summary>
        ''' <remarks></remarks>
        ''' <history>
        ''' [Richard Schaffranek]   22/11/2013 created
        ''' </history>
        ''' 

        Public Class gVdegrreComp
            Implements gVComp

            ''' <inheritdoc/>
            Public Function Compare(ByVal V1 As graphVertex, ByVal V2 As graphVertex) As Integer Implements IComparer(Of graphVertex).Compare
                If Not V1.isValid Then
                    If Not V2.isValid Then
                        Return 0
                    Else
                        Return -1
                    End If
                Else
                    If Not V2.isValid Then
                        Return 1
                    Else
                        If V1.outDegree > V2.outDegree Then
                            Return 1
                        ElseIf V2.outDegree > V1.outDegree Then
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