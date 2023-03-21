Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_graphMatrixParam
    Inherits GH_Param(Of GH_graphMatrix)


    Public Sub New()
        MyBase.New("graphMatrix", "gM", "graph Matrix representation", "Params", "SpiderWeb", GH_ParamAccess.item)

    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("19BD45D8-A437-47E1-A734-003B6E193326")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Drawing.Bitmap
        Get
            Return My.Resources.graphMatrix
        End Get
    End Property

End Class