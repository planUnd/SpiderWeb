Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphTools
Imports SpiderWebLibrary.graphElements.compare

Public Class GH_Component_MinSpannTree
    Inherits GH_Component

    Public Sub New()
        MyBase.New("MinST", "MinST", "Mininmal Spanning Tree", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("971e332d-6595-43f0-a91a-f138b6b5ebf8")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Undirected (u)", AddressOf setUndirected, True, True = GetValue("undirected", True))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "(Min)", AddressOf setMin, True, True = GetValue("method", True))
        Menu_AppendItem(menu, "(Max)", AddressOf setMax, True, False = GetValue("method", True))
    End Sub

    Private Sub setMax()
        RecordUndoEvent("methodeUndoEvent")
        SetValue("method", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setMin()
        RecordUndoEvent("methodeUndoEvent")
        SetValue("method", True)
        SetMessage()
        ExpireSolution(True)
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
        Select Case GetValue("method", True)
            Case True
                Message = Message & " \ Min"
            Case False
                Message = Message & " \ Max"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphEdgeListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("MininmalSpanningTree", "MinST", "Mininmal Spanning Tree")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.minTree
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gEL As New GH_GraphEdgeList()

        If (Not DA.GetData("Graph", gEL)) Then Return

        Dim minSP As New minSpanningTree(gEL)
        If GetValue("method", True) Then
            minSP.computeTree(New gEminCost())
        Else
            minSP.computeTree(New gEmaxCost())
        End If


        DA.SetData(0, minSP)
    End Sub
End Class
