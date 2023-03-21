Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
Imports SpiderWebLibrary

Public Class GH_SpiderWebShared

    Public Shared Function removeDoubleLines(ByVal line_L As List(Of Rhino.Geometry.Line), _
                                      ByVal undirected As Boolean, _
                                      ByVal tol As Double _
                                      ) As List(Of Integer)
        Dim compL As New R_compLine()
        Dim IR As New List(Of Integer)
        Dim L, LL As Rhino.Geometry.Line

        For i As Integer = line_L.Count - 1 To 0 Step -1
            L = line_L.Item(i)
            If L.Length <> 0 Then
                For ii As Integer = 0 To i - 1
                    LL = line_L.Item(ii)
                    If compL.Equals_(L, LL, undirected, tol) Then
                        line_L.RemoveAt(i)
                        IR.Insert(0, i)
                        Exit For
                    End If
                Next
            Else
                line_L.RemoveAt(i)
                IR.Insert(0, i)
            End If
        Next

        Return IR
    End Function

End Class
