Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_GraphVertexListParam
    Inherits GH_Param(Of GH_GraphVertexList)


    Public Sub New()
        MyBase.New("graphVertexList", "gVL", "Graph represented as list of vertices.", "Params", "SpiderWeb", GH_ParamAccess.item)
    End Sub


    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("c9b88f4d-cd7d-4a30-9327-0c01af5d6f66")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Drawing.Bitmap
        Get
            Return My.Resources.gVL
        End Get
    End Property

End Class