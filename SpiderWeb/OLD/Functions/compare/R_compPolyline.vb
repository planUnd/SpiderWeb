Public Class R_compPolyline
    Implements IComparer(Of Rhino.Geometry.Polyline)

    Dim tol As Double

    Public Sub New()
        tol = 1000
    End Sub

    Public Sub New(ByVal t As Double)
        tol = 1 / t
    End Sub

    Public Function Compare(ByVal Pol1 As Rhino.Geometry.Polyline, _
        ByVal Pol2 As Rhino.Geometry.Polyline) As Integer _
        Implements IComparer(Of Rhino.Geometry.Polyline).Compare

        If Not Pol1.IsValid Then
            If Not Pol2.IsValid Then
                Return 0
            Else
                Return -1
            End If
        Else
            If Not Pol2.IsValid Then
                Return 1
            Else
                Dim P1, P2 As Rhino.Geometry.Point3d
                P1 = Pol1.BoundingBox.Min
                P2 = Pol2.BoundingBox.Min
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
