Option Explicit On


Namespace graphElements
    Namespace compare
        ''' -------------------------------------------
        ''' Project : SpiderWebLibrary
        ''' Class : gVrndComp
        ''' 
        ''' <summary>
        ''' Comperator to randomly sort a List of graphVertex.
        ''' </summary>
        ''' <remarks></remarks>
        ''' ''' <history>
        ''' [Richard Schaffranek]   22/11/2013 created
        ''' [Richard Schaffranek]   26/03/2014 changed compare function
        ''' </history>
        ''' 

        Public Class gVrndComp
            Implements gVComp

            Private seed As Integer

            ''' <summary>
            ''' Construct a new random graphVertex comperator
            ''' </summary>
            ''' <param name="seed">Comperators with the same seed value will generate the same sorting order.</param>
            ''' <remarks></remarks>
            Public Sub New(Optional ByVal seed As Integer = 42)
                Me.seed = seed
            End Sub

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
                        Dim s1, s2 As Double
                        Dim rnd1, rnd2 As Random
                        rnd1 = New Random(Math.Min(V1.index + Me.seed, Int32.MaxValue))
                        s1 = rnd1.NextDouble()
                        rnd2 = New Random(Math.Min(V2.index + Me.seed, Int32.MaxValue))
                        s2 = rnd2.NextDouble()

                        If s1 > s2 Then
                            Return 1
                        ElseIf s2 > s1 Then
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