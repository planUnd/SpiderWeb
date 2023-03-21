Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class graphFromPoint
    Inherits GH_Component
    Public Sub New()
        MyBase.New("graphFromPoints", "graphFromPoints", "Create a Graph From a Set of Points and a Connection Distance", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("c3cfe040-6b49-11e0-ae3e-0800200c9a66")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", True))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "VertexList", AddressOf setVL, True, False = GetValue("representation", False))
        Menu_AppendItem(menu, "EdgeList", AddressOf setEL, True, True = GetValue("representation", False))
    End Sub

    Private Sub setUndirected()
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setVL()
        SetValue("representation", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setEL()
        SetValue("representation", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("representation", False)
            Case False
                Message = "VL/"
            Case True
                Message = "EL/"
        End Select

        Select Case GetValue("undirected", False)
            Case False
                Message = Message & "directed"
            Case True
                Message = Message & "undirected"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddPointParameter("GraphVertex", "GV", "Points to Callculate the Distance to", GH_ParamAccess.list)
        pManager.AddNumberParameter("ConnectingDistance", "CD", "Maximal Distance Between Two Points to Make Them Neighbours", GH_ParamAccess.item)
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
        Dim Vertex As New List(Of Rhino.Geometry.Point3d)
        Dim ConDist As Double = 0
        Dim rep As Boolean = GetValue("representation", False)

        If (Not DA.GetDataList("GraphVertex", Vertex)) Then Return
        If (Not DA.GetData("ConnectingDistance", ConDist)) Then Return
        If (Vertex.Count = 0) Then Return
        If (Not ConDist > 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Connection distance smaller than 0!")
            Return
        End If

        If (Vertex.Count > 65534 And rep) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Callulation might return false results. Graph limited to 65534 vertices!")
            Return
        End If

        If rep Then
            Dim GH_gEL As New GH_graphEdgeList()
            GH_gEL.GraphFromPoint(Vertex, ConDist, GH_Component.DocumentTolerance())
            DA.SetData(0, GH_gEL)
        Else
            Dim GH_gVL As New GH_graphVertexList()
            GH_gVL.GraphFromPoint(Vertex, ConDist, GH_Component.DocumentTolerance())
            DA.SetData(0, GH_gVL)
        End If

    End Sub
End Class
