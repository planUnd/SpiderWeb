Imports Grasshopper.Kernel

Public Class SpiderWebAssemblyInfo
    Inherits GH_AssemblyInfo

    Public Overrides ReadOnly Property Description As String
        Get
            Return MyBase.Description
        End Get
    End Property


    Public Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            Return My.Resources.SpiderWebIcon
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "SpiderWeb :: Graph Tools"
        End Get
    End Property

    Public Overrides ReadOnly Property Version As String
        Get
            Return MyBase.Version
        End Get
    End Property

    Public Overrides ReadOnly Property AuthorName As String
        Get
            Return "Richard Schaffranek"
        End Get
    End Property

    Public Overrides ReadOnly Property AuthorContact As String
        Get
            Return "http://www.gbl.tuwien.ac.at/Archiv/digital.html?name=Spider_Web"
        End Get
    End Property


End Class
