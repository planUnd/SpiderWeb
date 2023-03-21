Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_visualGraphParam
    Inherits GH_Param(Of GH_visualGraph)


    Public Sub New()
        MyBase.New("visualGraph", "vG", "visualGraph represented as list of vertices.", "Params", "SpiderWeb", GH_ParamAccess.item)
    End Sub


    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("C7FB9AEA-9593-4F33-9660-13CA25D13F28")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Drawing.Bitmap
        Get
            Return My.Resources.paramVGA
        End Get
    End Property
End Class