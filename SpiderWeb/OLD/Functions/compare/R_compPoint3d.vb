Public Class R_compPoint3d
    Implements IComparer(Of Rhino.Geometry.Point3d)

    Dim tol As Double

    Public Sub New()
        tol = 1000
    End Sub

    Public Sub New(ByVal t As Double)
        tol = 1 / t
    End Sub

    Public Function Compare(ByVal P1 As Rhino.Geometry.Point3d, _
    ByVal P2 As Rhino.Geometry.Point3d) As Integer _
    Implements IComparer(Of Rhino.Geometry.Point3d).Compare

        If Not P1.IsValid Then
            If Not P2.IsValid Then
                Return 0
            Else
                Return -1
            End If
        Else
            If Not P2.IsValid Then
                Return 1
            Else
                If Math.Round(P1.X * tol) > Math.Round(P2.X * tol) Then
                    Return 1
                ElseIf Math.Round(P1.X * tol) < Math.Round(P2.X * tol) Then
                    Return -1
                ElseIf Math.Round(P1.X * tol) = Math.Round(P2.X * tol) And Math.Round(P1.Y * tol) > Math.Round(P2.Y * tol) Then
                    Return 1
                ElseIf Math.Round(P1.X * tol) = Math.Round(P2.X * tol) And Math.Round(P1.Y * tol) < Math.Round(P2.Y * tol) Then
                    Return -1
                ElseIf Math.Round(P1.X * tol) = Math.Round(P2.X * tol) And Math.Round(P1.Y * tol) = Math.Round(P2.Y * tol) And Math.Round(P1.Z * tol) > Math.Round(P2.Z * tol) Then
                    Return 1
                ElseIf Math.Round(P1.X * tol) = Math.Round(P2.X * tol) And Math.Round(P1.Y * tol) = Math.Round(P2.Y * tol) And Math.Round(P1.Z * tol) < Math.Round(P2.Z * tol) Then
                    Return -1
                Else
                    Return 0
                End If
            End If
        End If
    End Function
End Class
