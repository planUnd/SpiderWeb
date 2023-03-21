Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
Imports SpiderWebLibrary.graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports GH_SpiderWebLibrary.R_Compare
Imports GH_SpiderWebLibrary.GH_compare

Namespace GH_graphRepresentaions
    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : GH_visualGraph
    ''' 
    ''' <summary>
    ''' Vertex List representation of a Graph.
    ''' </summary>
    ''' <remarks>
    ''' Extends the SpiderWebLibrary class graphVertexList with additional methodes linked to Grasshopper and Rhino.
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Class GH_visualGraph
        Inherits visualGraph
        Implements IGH_Goo

        ' ------------------------- Constructor ----------------------


        ''' <summary>
        ''' Construct an empty visualGraph
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Construct a new GH_visualGraph
        ''' </summary>
        ''' <param name="w">Width of the grid (x)</param>
        ''' <param name="h">Height of the grid (y)</param>
        ''' <remarks>Will set all gridCells to State.void</remarks>
        Public Sub New(ByVal w As Integer, ByVal h As Integer)
            MyBase.New(w, h)
        End Sub

        Public Sub New(ByVal gV As GH_visualGraph)
            MyBase.New(gV)
        End Sub

        ' ------------------------- IGH_Goo ----------------------
        ''' <summary>
        ''' This method is called whenever the instance is required to deserialize itself. (Inherited from GH_ISerializable.)
        ''' </summary>
        ''' <returns>Returns <code>False</code></returns>
        ''' <remarks>Not implemented!</remarks>
        Public Function Read(reader As GH_IO.Serialization.GH_IReader) As Boolean Implements GH_IO.GH_ISerializable.Read
            Return False
        End Function

        ''' <summary>
        ''' This method is called whenever the instance is required to serialize itself. (Inherited from GH_ISerializable.) 
        ''' </summary>
        ''' <returns>Returns <code>False</code></returns>
        ''' <remarks>Not implemented!</remarks>
        Public Function Write(writer As GH_IO.Serialization.GH_IWriter) As Boolean Implements GH_IO.GH_ISerializable.Write
            Return False
        End Function

        ''' <summary>
        ''' Attempt a cast from generic object 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CastFrom(source As Object) As Boolean Implements IGH_Goo.CastFrom
            If source Is Nothing Then Return False
            If GetType(visualGraph).IsAssignableFrom(source.GetType) Then
                Dim vG As visualGraph = DirectCast(source, visualGraph)
                Me.vertexList = vG.getVertexList
                Me.grid = vG.gridCell
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Attempt a cast to type T 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="target"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CastTo(Of T)(ByRef target As T) As Boolean Implements IGH_Goo.CastTo
            If (GetType(T).IsAssignableFrom(GetType(GH_visualGraph))) Then
                Dim tmpGraph As Object = Me
                target = DirectCast(tmpGraph, T)
                Return True
            Else If (GetType(T).IsAssignableFrom(GetType(GH_GraphVertexList))) Then
                Dim tmpGraph As Object = Me
                target = DirectCast(tmpGraph, T)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Make a complete duplicate of this instance. No shallow copies. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Duplicate() As IGH_Goo Implements IGH_Goo.Duplicate
            Return New GH_visualGraph(Me)
        End Function

        ''' <summary>
        ''' Create a new proxy for this instance. Return Null if this class does not support proxies. 
        ''' </summary>
        ''' <returns>Returns <code>Nothing</code></returns>
        ''' <remarks>Not implemented!</remarks>
        Public Function EmitProxy() As IGH_GooProxy Implements IGH_Goo.EmitProxy
            Return Nothing
        End Function

        ''' <summary>
        ''' Gets a value indicating whether or not the current value is valid. 
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns true.</returns>
        ''' <remarks> Graphs should always be valid, if not please contact the author.</remarks>
        Public ReadOnly Property IsValid As Boolean Implements IGH_Goo.IsValid
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Gets a string describing the state of "invalidness". If the instance is valid, then this property should return Nothing or String.Empty. 
        ''' </summary>
        ''' <value></value> 
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsValidWhyNot As String Implements IGH_Goo.IsValidWhyNot
            Get
                Return "If you can read this please contact the author of SpiderWeb and ask for help."
            End Get
        End Property

        ''' <summary>
        ''' Get the "naked" SpiderWebLibrary represnetation.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ScriptVariable() As Object Implements IGH_Goo.ScriptVariable
            Return New visualGraph(Me)
        End Function
        ''' <summary>
        ''' Creates a string description of the current instance value 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToStringGH() As String Implements IGH_Goo.ToString
            Return "GH_" & Me.ToString()
        End Function

        ''' <summary>
        ''' Gets the TypeDescription
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeDescription As String Implements IGH_Goo.TypeDescription
            Get
                Return "visualGraph represented as List of Vertecies"
            End Get
        End Property

        ''' <summary>
        ''' Gets the TypeName
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeName As String Implements IGH_Goo.TypeName
            Get
                Return "GH_visualGraph"
            End Get
        End Property

        ' ------------------------- GH_visualGraph ----------------------

        ''' <summary>
        ''' Set the State the visualGraphVertices to Solid / Void.
        ''' </summary>
        ''' <param name="Boundary">Boundary outline that defines the area in which to construct the visualGraph grid.</param>
        ''' <param name="obstacles">Obstacles within the Boundary</param>
        ''' <param name="sXY">Wanted size of the gridCells</param>
        ''' <param name="dt">If the midpoint is not within an obstacle a gridCell is also 
        ''' defined as solid if the obstacles is within the given distance form the midpoint</param>
        ''' <returns>A list of rectangles, representing the gridCells</returns>
        ''' <remarks></remarks>
        Public Function visualGraphGrid(ByVal Boundary As Curve, ByVal obstacles As List(Of Curve), _
                                        ByVal sXY As Double, Optional ByVal dt As Double = 0) As List(Of Rectangle3d)
            Dim BB As BoundingBox
            Dim w, h As Integer
            Dim tmpP As Point3d
            Dim tmpR As Rectangle3d
            Dim RList As New List(Of Rectangle3d)
            Dim minX, minY, hXY As Double

            BB = Boundary.GetBoundingBox(True)
            w = Math.Round((BB.Max().X - BB.Min().X) / sXY)
            h = Math.Round((BB.Max().Y - BB.Min().Y) / sXY)
            Me.setup(w, h)

            hXY = sXY / 2.0
            minX = BB.Min().X + hXY
            minY = BB.Min().Y + hXY

            For iY As Integer = 0 To h - 1
                For iX As Integer = 0 To w - 1
                    tmpP = New Point3d(minX + sXY * iX, minY + sXY * iY, 0)
                    tmpR = New Rectangle3d(New Plane(tmpP, New Vector3d(0, 0, 1)), New Interval(-hXY, hXY), New Interval(-hXY, hXY))
                    RList.Add(tmpR)
                    If Me.Inside(obstacles, Boundary, tmpP, dt) Then
                        Me.gridCell(iX, iY) = State.void
                    Else
                        Me.gridCell(iX, iY) = State.solid
                    End If
                Next
            Next

            Return RList
        End Function

        ' ----------------------------- GH_visualGraph Private -------------------------

        ''' <summary>
        ''' Test if a point is within an obstacle or outside of a Boundary.
        ''' </summary>
        ''' <param name="Obstacle">Obstacles within the Boundary</param>
        ''' <param name="Boundary">Boundary outline that defines the area in which to construct the visualGraph grid.</param>
        ''' <param name="p">Point to test</param>
        ''' <param name="dt">If the midpoint is not within an obstacle a gridCell is also defined as solid if the obstacles is within the given distance form the midpoint</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Inside(ByVal Obstacle As List(Of Curve), ByVal Boundary As Curve, ByVal p As Point3d, Optional ByVal dt As Double = 0) As Boolean
            Dim t As Double

            If Boundary.Contains(p) = PointContainment.Outside Then
                Return False
            ElseIf dt > 0 Then
                Boundary.ClosestPoint(p, t)
                If p.DistanceTo(Boundary.PointAt(t)) < dt Then
                    Return False
                End If
            End If

            For Each c As Curve In Obstacle
                If c.Contains(p) = PointContainment.Inside Then
                    Return False
                ElseIf dt > 0 Then
                    c.ClosestPoint(p, t)
                    If p.DistanceTo(c.PointAt(t)) < dt Then
                        Return False
                    End If
                End If
            Next

            Return True
        End Function
    End Class
End Namespace