Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_graphFromPoint
    Inherits GH_Component
    Public Sub New()
        MyBase.New("graphFromPoints", "gFP", "Create a Graph From a Set of Points and a Connection Distance", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("c3cfe040-6b49-11e0-ae3e-0800200c9a66")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Undirected (u)", AddressOf setUndirected, True, True = GetValue("undirected", True))
    End Sub

    Private Sub setUndirected()
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
    End Sub


    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("undirected", True)
            Case False
                Message = "d"
            Case True
                Message = "u"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddPointParameter("graphPoints", "GP", "3d points of the Graph", GH_ParamAccess.list)
        pManager.AddNumberParameter("ConnectingDistance", "CD", "Maximal distance between two points to make them neighbours", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of points")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphFromPoints
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim GP_L As New List(Of Rhino.Geometry.Point3d)
        Dim ConDist As Double = 0

        If (Not DA.GetDataList("graphPoints", GP_L)) Then Return
        If (Not DA.GetData("ConnectingDistance", ConDist)) Then Return
        If (GP_L.Count = 0) Then Return
        If (Not ConDist > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Connection distance smaller than 0!")
            Return
        End If

        Dim gVL As New GH_GraphVertexList()
        gVL.GraphFromPoint(GP_L, ConDist, GH_Component.DocumentTolerance())
        DA.SetData(0, gVL)
    End Sub
End Class
