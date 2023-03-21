Public Class GH_compNumber
    Implements IComparer(Of Grasshopper.Kernel.Types.GH_Number)

    Public Function Compare(ByVal V1 As Grasshopper.Kernel.Types.GH_Number, _
    ByVal V2 As Grasshopper.Kernel.Types.GH_Number) As Integer _
    Implements IComparer(Of Grasshopper.Kernel.Types.GH_Number).Compare

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
