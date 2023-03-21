Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_GraphToDataTree
    Inherits GH_Component

    Public Sub New()
        MyBase.New("GraphToDataTree", "GDT", "convert a Graph to a DataTree", "Extra", "SpiderWebBasic")
    End Sub


    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("576C4E7B-C2B5-4B88-9D15-BF013A856FC6")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.toDataTree
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)
        Menu_AppendItem(menu, "To Vertex List (VL)", AddressOf toVertexList, True, True = GetValue("methode", True))
        Menu_AppendItem(menu, "To Edge List (EL)", AddressOf toEdgeList, True, False = GetValue("methode", True))
    End Sub

    Private Sub toVertexList()
        SetValue("methode", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub toEdgeList()
        SetValue("methode", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub SetMessage()
        Message = ""
        Select Case GetValue("methode", True)
            Case True
                Message = Message & "VL"
            Case False
                Message = Message & "EL"
        End Select
    End Sub

    Public Overrides Sub AddedToDocument(document As GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (graphVertexList / graphEdgeList)", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("DataTree", "T", "DataTree representing the Graph.")
        pManager.Register_DoubleParam("Costs", "C", "Edge costs of the Graph.")
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        If (Not DA.GetData("Graph", gVL)) Then Return

        Dim gDT As New DataTree(Of Integer)
        Dim gecDT As New DataTree(Of Double)
        Dim gehDT As New DataTree(Of Double)

        Select Case GetValue("methode", True)
            Case True
                DA.SetDataTree(0, gVL.G_DATATREE)
                DA.SetDataTree(1, gVL.GEC_DATATREE)
            Case False
                DA.SetDataTree(0, gVL.EG_DATATREE)
                DA.SetDataTree(1, gVL.EC_DATATREE)
        End Select
    End Sub
End Class
