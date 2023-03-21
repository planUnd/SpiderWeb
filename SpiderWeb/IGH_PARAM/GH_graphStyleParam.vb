Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_graphStyleParam
    Inherits GH_Param(Of GH_graphStyle)


    Public Sub New()
        MyBase.New("graphStyle", "gS", "Style to Represent a Graph", "Params", "SpiderWeb", GH_ParamAccess.item)
    End Sub


    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("33835D18-08EC-4A09-A7BD-6CA175992158")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Drawing.Bitmap
        Get
            Return My.Resources.graphPropertiesParam
        End Get
    End Property
End Class