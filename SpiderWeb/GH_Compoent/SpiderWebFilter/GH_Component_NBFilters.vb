Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_NBFilters
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Image-GraphFilter", "IGF", "Iamge based Filters on Graphs", "Extra", "SpiderWebFilter")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("a7d3b7f6-46e1-46f4-ac07-2c5fd093ec49")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Median (M)", AddressOf setMedian, True, 0 = GetValue("filter", 1))
        Menu_AppendItem(menu, "Average (A)", AddressOf setAverage, True, 1 = GetValue("filter", 1))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("filter", 1)
            Case 0
                Message = "Median"
            Case 1
                Message = "Average"
        End Select
    End Sub

    Private Sub setMedian(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("filterUndoEvent")
        SetValue("filter", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setAverage(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("filterUndoEvent")
        SetValue("filter", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddNumberParameter("Values", "V", "Values to Filter", GH_ParamAccess.list)
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.NBFilter
        End Get
    End Property

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_DoubleParam("Filtered Values", "FV", "Filtered Values")
    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        Dim Values As New List(Of Double)

        If (Not DA.GetDataList("Values", Values)) Then Return
        If (Not DA.GetData("Graph", gVL)) Then Return

        If (Values.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input V(Values)")
            Return
        End If
        If (gVL.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        If (gVL.vertexCount <> Values.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input V. Number of graphVertex (G) must be equal to number of Values (V) ")
            Return
        End If

        Select Case GetValue("filter", 1)
            Case 0
                DA.SetDataList(0, gVL.Median(Values))
            Case 1
                DA.SetDataList(0, gVL.Average(Values))
            Case Else
        End Select

    End Sub
End Class
