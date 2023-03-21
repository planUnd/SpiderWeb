Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports MathNet.Numerics.LinearAlgebra
Imports SpiderWebLibrary.clustering

Namespace GH_Clustering

    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : GH_agglomerativeClustering
    ''' 
    ''' <summary>
    ''' Clustering Helper
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   18/11/2014 created
    ''' </history>
    ''' 

    Public Class GH_ClusteringHelper

        Public Shared Function VectorFromStructure(ByVal VecDT As GH_Structure(Of GH_Number)) As Vector(Of Double)()
            Dim vec(VecDT.PathCount - 1) As Vector(Of Double)
            Dim L As List(Of GH_Number)

            For i As Integer = 0 To VecDT.PathCount - 1
                L = VecDT.Branch(i)
                Dim tmpVec As Vector(Of Double)
                tmpVec = Vector(Of Double).Build.Dense(L.Count)
                For ii As Integer = 0 To L.Count - 1
                    tmpVec(ii) = L.Item(ii).Value
                Next
                vec(i) = tmpVec
            Next
            Return vec
        End Function

        Public Shared Function agglomerativeClusteringAsDataTree(ByVal agg As agglomerativeClustering) As DataTree(Of Integer)
            Dim cIDT As New DataTree(Of Integer)

            For i As Integer = 0 To agg.count - 1
                cIDT.EnsurePath(i)
                cIDT.Branch(i).AddRange(agg.indexCluster(i))
            Next

            Return cIDT
        End Function

        Public Shared Function distanceClusteringAsDataTree(ByVal dC As distanceClustering) As DataTree(Of Integer)
            Dim cIDT As New DataTree(Of Integer)

            For i As Integer = 0 To dC.count - 1
                cIDT.EnsurePath(i)
                cIDT.Branch(i).AddRange(dC.indexCluster(i))
            Next

            Return cIDT
        End Function

    End Class
End Namespace
