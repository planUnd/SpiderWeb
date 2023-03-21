Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphTools

Public Class Eulerian_path
    Inherits GH_Component

    Public Sub New()
        MyBase.New("EulerianPath", "EulerianPath", "Checks if the Given Graph Has an Eulerian Path and Eeturns All Possible Starting Points", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("00d89f0c-a48b-4b66-beeb-d470d532e399")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Directed", AddressOf setDirected, True, False = GetValue("undirected", False))
    End Sub

    Private Sub setDirected()
        SetValue("undirected", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("undirected", False)
            Case False
                Message = "d"
            Case True
                Message = "u"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("StartingPoints", "SP", "Possible Starting Points of the Eulerian Path")
        pManager.Register_IntegerParam("EndPoints", "EP", "Possible End Points of the Eulerian Path")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.EulerPath
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()

        If (Not DA.GetData("Graph", gVL)) Then Return

        Dim epG As New eulerianPath(gVL)

        epG.computeSPEP()

        DA.SetDataList(0, epG.getSPindex)
        DA.SetDataList(1, epG.getEPindex)
    End Sub
End Class
