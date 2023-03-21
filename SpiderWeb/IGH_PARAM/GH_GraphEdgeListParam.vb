Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_GraphEdgeListParam
    Inherits GH_Param(Of GH_GraphEdgeList)

    Public Sub New()
        MyBase.New("graphEdgeList", "gEL", "Graph represented as list of edges.", "Params", "SpiderWeb", GH_ParamAccess.item)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("220ddedf-5169-4d93-bb6b-a1496f78daad")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Drawing.Bitmap
        Get
            Return My.Resources.gEL
        End Get
    End Property

End Class
