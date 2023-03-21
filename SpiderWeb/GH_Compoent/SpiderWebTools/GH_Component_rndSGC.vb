Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphTools
Imports SpiderWebLibrary.graphElements.compare

Public Class GH_Component_rndSGC
    Inherits GH_Component

    Public Sub New()
        MyBase.New("randomized Sequential Graph Coloring", "rndSGC", "Randomized Sequential Graph Coloring", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("75b7f357-29e9-4613-8d35-bbe21be006f1")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Undirected (u)", AddressOf setUndirected, True, True = GetValue("undirected", True))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "Random (R)", AddressOf setRandom, True, True = GetValue("method", False))
        Menu_AppendItem(menu, "Degree (D)", AddressOf setDegree, True, False = GetValue("method", False))
    End Sub

    Private Sub setRandom()
        RecordUndoEvent("methodeUndoEvent")
        SetValue("method", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setDegree()
        RecordUndoEvent("methodeUndoEvent")
        SetValue("method", False)
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
        Select Case GetValue("method", False)
            Case False
                Message = Message & " \ D"
            Case True
                Message = Message & " \ R"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddIntegerParameter("maximum itterations", "maxItt", "Maximum Itterations", GH_ParamAccess.item, 100)
        pManager.AddIntegerParameter("aimed Colors", "aC", "Aimed Colors", GH_ParamAccess.item, 4)
        pManager.AddIntegerParameter("preset Colors", "pC", "Preset Colors [Optinal, Same Length as Elements Within the Graph]  ", GH_ParamAccess.list)
        pManager.Param(3).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Color/Vertex", "CV", "Color / Vertex")
        pManager.Register_IntegerParam("colourCount", "cC", "colourCount")
        ' pManager.Register_IntegerParam("vistingOrder", "VO", "vistingOrder")
        ' pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.rndSGC
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        If (Not DA.GetData("Graph", gVL)) Then Return

        Dim itt As Integer
        Dim aC As Integer
        Dim pC As New List(Of Integer)


        If (Not DA.GetData("maximum itterations", itt)) Then Return
        If (Not DA.GetData("aimed Colors", aC)) Then Return
        DA.GetDataList("preset Colors", pC)
        If (pC.Count > 0 And pC.Count <> gVL.vertexCount) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input pC. When setting pC, number of vertices of (G) must be equal to number of values (pC)")
            Return
        End If

        Dim rndSGC As New graphColoring(gVL)

        If (pC.Count > 0) Then
            rndSGC.colors = pC.ToArray
        End If

        If GetValue("method", False) Then
            rndSGC.rndSGC(aC, itt)
        Else

            rndSGC.SGC(New gVdegrreComp())
        End If
        DA.SetDataList(0, rndSGC.colors)
        DA.SetData(1, rndSGC.colorCount)
    End Sub

End Class
