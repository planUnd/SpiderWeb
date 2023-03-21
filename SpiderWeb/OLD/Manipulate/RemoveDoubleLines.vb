Imports Grasshopper.Kernel

Public Class RemoveDoubleLines
    Inherits GH_Component

    Public Sub New()
        MyBase.New("cleanSetOfLines", "cleanSetOfLines", "Removes all Double Lines and 0 Length Lines From a List of Lines", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("b934160c-1afc-436d-890c-bc9ec65384e5")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "directed", AddressOf setDirected, True, False = GetValue("undirected", False))
        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", False))
    End Sub

    Private Sub setDirected()
        SetValue("undirected", False)
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
        Select Case GetValue("undirected", False)
            Case False
                Message = "directed"
            Case True
                Message = "undirected"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddLineParameter("Lines", "L", "List of lines to Clean", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Indices Removed", "IR", "Indices Removed")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.removeMultiEdges
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim line_L As New List(Of Rhino.Geometry.Line)

        If (Not DA.GetDataList("Lines", line_L)) Then Return
        If (line_L.Count = 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input L(Lines)")
            Return
        End If

        Dim undirected As Boolean = GetValue("undirected", False)
        Dim IR As New List(Of Integer)

        Dim DebuggStr As New List(Of String)

        IR = GH_SpiderWebShared.removeDoubleLines(line_L, undirected, 1.0 / GH_Component.DocumentTolerance())

        SetMessage()
        If IR.Count > 0 Then
            Message = Message & " : " & IR.Count & " removed"
        End If

        DA.SetDataList(0, IR)
    End Sub



End Class
