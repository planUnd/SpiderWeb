Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports SpiderWebLibrary.clustering
Imports GH_SpiderWebLibrary.GH_Clustering

Public Class GH_Component_distanceClustering
    Inherits GH_Component
    Public Sub New()
        MyBase.New("distanceClustering", "dC", "Preforms a top down distance clustering.", "Extra", "SpiderWebClustering")

    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("1F71F9EA-4892-44F3-B44C-F0C99296DBE8")
        End Get
    End Property


    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Euclidian (E)", AddressOf setEuclidian, True, 0 = GetValue("type", 0))
        Menu_AppendItem(menu, "Gower (G)", AddressOf setGower, True, 1 = GetValue("type", 0))
        Menu_AppendItem(menu, "Manhatten (M)", AddressOf setManhatten, True, 2 = GetValue("type", 0))
        Menu_AppendItem(menu, "Pearson (P)", AddressOf setPearson, True, 3 = GetValue("type", 0))
        Menu_AppendItem(menu, "Chi Squared (CS)", AddressOf chiSquared, True, 4 = GetValue("type", 0))
        Menu_AppendItem(menu, "Earth Mover (EM)", AddressOf earthMover, True, 5 = GetValue("type", 0))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("type", 0)
            Case 0
                Message = "E"
            Case 1
                Message = "G"
            Case 2
                Message = "M"
            Case 3
                Message = "P"
            Case 4
                Message = "CS"
            Case 5
                Message = "EM"
        End Select
    End Sub


    Private Sub setEuclidian(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setGower(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setManhatten(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setPearson(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 3)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub chiSquared(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 4)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub earthMover(ByVal sender As Object, ByVal e As EventArgs)
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
        pManager.AddNumberParameter("Vector", "V", "DataTree of Vectors to Cluster", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("number", "nr", "number of clusters", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("match", "m", "clusterIndex for each Vector")
        pManager.Register_IntegerParam("clusterIndex", "cI", "vectorIndex for each Cluster")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.distance
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim vDT As New GH_Structure(Of GH_Number)
        Dim nr As Integer

        If (Not DA.GetDataTree("Vector", vDT)) Then Return
        If (Not DA.GetData("number", nr)) Then Return

        Dim dC As distanceClustering

        Select Case GetValue("type", 0)
            Case 1
                dC = New gowerDistance(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 2
                dC = New manhattenDistance(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 3
                dC = New pearsonDistance(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 4
                dC = New chiSquaredDistance(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case 5
                dC = New earthMoverDistance(GH_ClusteringHelper.VectorFromStructure(vDT))
            Case Else
                dC = New euclidianDistance(GH_ClusteringHelper.VectorFromStructure(vDT))
        End Select

        dC.cluster(nr)

        DA.SetDataList(0, dC.match())
        DA.SetDataTree(1, GH_ClusteringHelper.distanceClusteringAsDataTree(dC))
    End Sub
End Class
