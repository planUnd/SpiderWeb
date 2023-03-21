Public Class R_compLine

    Public Function Equals_(ByVal L1 As Rhino.Geometry.Line, ByVal L2 As Rhino.Geometry.Line, ByVal sort As Boolean, ByVal tol As Double) As Boolean
        If Math.Round(L1.FromX * tol) = Math.Round(L2.FromX * tol) And Math.Round(L1.FromY * tol) = Math.Round(L2.FromY * tol) And Math.Round(L1.FromZ * tol) = Math.Round(L2.FromZ * tol) And _
           Math.Round(L1.ToX * tol) = Math.Round(L2.ToX * tol) And Math.Round(L1.ToY * tol) = Math.Round(L2.ToY * tol) And Math.Round(L1.ToZ * tol) = Math.Round(L2.ToZ * tol) Then
            Return True
        ElseIf sort And Math.Round(L1.FromX * tol) = Math.Round(L2.ToX * tol) And Math.Round(L1.FromY * tol) = Math.Round(L2.ToY * tol) And Math.Round(L1.FromZ * tol) = Math.Round(L2.ToZ * tol) And _
           Math.Round(L1.ToX * tol) = Math.Round(L2.FromX * tol) And Math.Round(L1.ToY * tol) = Math.Round(L2.FromY * tol) And Math.Round(L1.ToZ * tol) = Math.Round(L2.FromZ * tol) Then
            Return True
        End If

        Return False
    End Function
End Class
