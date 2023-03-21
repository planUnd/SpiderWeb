Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports SpiderWebLibrary.clustering
Imports GH_SpiderWebLibrary.GH_Clustering
Imports MathNet.Numerics.LinearAlgebra

Public Class GH_Component_agglomerativeClustering
    Inherits GH_Component
    Public Sub New()
        MyBase.New("agglomerativeClustering", "aC", "Preforms a bottom up distance clustering.", "Extra", "SpiderWebClustering")

    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("1037CBCF-1F77-4219-8918-7A7F1C411BAA")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "singleLinkage (sL)", AddressOf setSingleLinkage, True, 0 = GetValue("type", 0))
        Menu_AppendItem(menu, "completeLinkage (cL)", AddressOf setCompleteLinkage, True, 1 = GetValue("type", 0))
        Menu_AppendItem(menu, "(UPGMA)", AddressOf setUPGMA, True, 2 = GetValue("type", 0))
        Menu_AppendItem(menu, "(UPGMC)", AddressOf setUPGMC, True, 3 = GetValue("type", 0))
        Menu_AppendItem(menu, "(WARD)", AddressOf setWARD, True, 4 = GetValue("type", 0))
        Menu_AppendItem(menu, "(WPGMA)", AddressOf setWPGMA, True, 5 = GetValue("type", 0))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("type", 0)
            Case 0
                Message = "sL"
            Case 1
                Message = "cL"
            Case 2
                Message = "UPGMA"
            Case 3
                Message = "UPGMC"
            Case 4
                Message = "WARD"
            Case 5
                Message = "WPGMA"
        End Select
    End Sub


    Private Sub setSingleLinkage(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setCompleteLinkage(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setUPGMA(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setUPGMC(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 3)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setWARD(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 4)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setWPGMA(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 5)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddNumberParameter("Vector", "V", "DataTree of Numbers to Cluster", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("number", "nr", "number of clusters", GH_ParamAccess.item)
        pManager.AddIntegerParameter("fixed", "f", "Optional, first n Vectors will stay in their own clusters", GH_ParamAccess.item, 0)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("match", "m", "clusterIndex for each Vector")
        pManager.Register_IntegerParam("clusterIndex", "cI", "vectorIndex for each Cluster")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.agglomerative
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim vDT As New GH_Structure(Of GH_Number)
        Dim fixed As Integer
        Dim nr As Integer

        If (Not DA.GetDataTree("Vector", vDT)) Then Return
        If (Not DA.GetData("fixed", fixed)) Then Return
        If (Not DA.GetData("number", nr)) Then Return

        nr = Math.Max(nr, fixed)

        Dim agg As agglomerativeClustering

        Select Case GetValue("type", 0)
            Case 1
                agg = New completeLinkage(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 2
                agg = New UPGMA(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 3
                agg = New UPGMC(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 4
                agg = New WARD(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 5
                agg = New WPGMA(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case Else
                agg = New singleLinkage(GH_ClusteringHelper.VectorFromStructure(vDT))
        End Select

        agg.cluster(nr, fixed)

        DA.SetDataList(0, agg.match())
        DA.SetDataTree(1, GH_ClusteringHelper.agglomerativeClusteringAsDataTree(agg))
    End Sub
End Class
