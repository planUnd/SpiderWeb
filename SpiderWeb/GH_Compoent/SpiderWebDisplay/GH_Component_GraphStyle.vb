Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_GraphStyle
    Inherits GH_Component


    Public Sub New()
        MyBase.New("Graph Style", "gS", "Greate New Style for Displaying a Graph", "Extra", "SpiderWebDisplay")

    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("933B770C-8E11-4239-8FD1-F06FDAC94545")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphProperties
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddColourParameter("Color", "C", "Color to Display the Graph", GH_ParamAccess.item, Drawing.Color.DeepPink)
        pManager.AddNumberParameter("txtSize", "tS", "Text Size to Display the Graph", GH_ParamAccess.item, 1.0)
        pManager.AddBooleanParameter("update", "u", "Update Graph Drawings", GH_ParamAccess.item, True)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("graphStyle", "gS", "Style for Displaying a Graph")
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim C As Drawing.Color
        Dim txtS As Double

        DA.GetData("Color", C)
        DA.GetData("txtSize", txtS)

        Dim gP As New GH_graphStyle(C, txtS)

        DA.SetData(0, gP)
    End Sub
End Class
