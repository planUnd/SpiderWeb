Option Explicit On

Imports Rhino.Geometry

Namespace R_Compare

    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : R_compPolyline
    ''' 
    ''' <summary>
    ''' Comperator for Rhino.Geometry.Polyline
    ''' </summary>
    ''' <remarks>
    ''' Based on the minimum of the Bounding Box
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 

    Public Class R_compPolyline
        Implements IComparer(Of Rhino.Geometry.Polyline)

        ''' <summary>
        ''' Tolerance for comparing Rhino.Geometry.Polyline
        ''' </summary>
        ''' <remarks></remarks>
        Private tol As Double

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="t">Optional, if not speciefed t = 1</param>
        ''' <remarks></remarks>
        Public Sub New(Optional ByVal t As Double = 1)
            tol = t
        End Sub


        ''' <summary>
        ''' Compare two Rhino.Geometry.Polyline
        ''' </summary>
        ''' <param name="Pol1">Rhino.Geometry.Polyline to compare</param>
        ''' <param name="Pol2">Rhino.Geometry.Polyline to compare</param>
        ''' <returns>-1 if Pol1.BoundingBox.Min  is smaller than Pol2.BoundingBox.Min, 0 if they are equal, 1 if Pol2.BoundingBox.Min is smaller than Pol1.BoundingBox.Min.</returns>
        ''' <remarks>Compares Polylines based on the coordinates of Pol1.BoundingBox.Min first X then Y then Z. Uses the specified tolerance to compare the points</remarks>
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
                    If Math.Round(P1.X / tol) > Math.Round(P2.X / tol) Then
                        Return 1
                    ElseIf Math.Round(P1.X / tol) < Math.Round(P2.X / tol) Then
                        Return -1
                    ElseIf Math.Round(P1.Y / tol) > Math.Round(P2.Y / tol) Then
                        Return 1
                    ElseIf Math.Round(P1.Y / tol) < Math.Round(P2.Y / tol) Then
                        Return -1
                    ElseIf Math.Round(P1.Z / tol) > Math.Round(P2.Z / tol) Then
                        Return 1
                    ElseIf Math.Round(P1.Z / tol) < Math.Round(P2.Z / tol) Then
                        Return -1
                    Else
                        Return 0
                    End If
                End If
            End If
        End Function
    End Class
End Namespace