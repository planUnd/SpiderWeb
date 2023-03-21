Option Explicit On

Imports Rhino.Geometry

Namespace R_Compare

    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : R_compPoint3d
    ''' 
    ''' <summary>
    ''' Comperator for Rhino.Geometry.Point3d
    ''' </summary>
    ''' <remarks>
    ''' compares x y z values
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Class R_compPoint3d
        Implements IComparer(Of Point3d)

        ''' <summary>
        ''' Tolerance for comparing Rhino.Geometry.Point3d
        ''' </summary>
        ''' <remarks></remarks>
        Private tol As Double

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="t">Optional, if not speciefed t = 1</param>
        ''' <remarks></remarks>
        Public Sub New(Optional ByVal t As Double = 1)
            Me.tol = t
        End Sub


        ''' <summary>
        ''' Compare two Rhino.Geometry.Point3d
        ''' </summary>
        ''' <param name="P1">Rhino.Geometry.Point3d to compare</param>
        ''' <param name="P2">Rhino.Geometry.Point3d to compare</param>
        ''' <returns>-1 if P1 is smaller than P2, 0 if they are equal, 1 if P2 is smaller than P1.</returns>
        ''' <remarks>Compares points based on their coordinates first X then Y then Z. Uses the specified tolerance to compare the points</remarks>
        Public Function Compare(ByVal P1 As Point3d, _
        ByVal P2 As Point3d) As Integer _
        Implements IComparer(Of Point3d).Compare

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