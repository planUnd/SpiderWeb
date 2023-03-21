Option Explicit On

Imports Grasshopper
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
Imports SpiderWebLibrary.graphRepresentaions
Imports SpiderWebLibrary.graphElements
Imports GH_SpiderWebLibrary.R_Compare
Imports GH_SpiderWebLibrary.GH_compare
Imports MathNet.Numerics.LinearAlgebra

Namespace GH_graphRepresentaions
    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : GH_graphMatrix
    ''' 
    ''' <summary>
    ''' Matrix representation of a Graph.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   06/06/2014 created
    '''  [Richard Schaffranek]   16/11/2014 changed: meta.numerics -> math.net
    ''' </history>
    ''' 
    Public Class GH_graphMatrix
        Inherits graphMatrix
        Implements IGH_Goo

        ' ------------------------- Constructor ----------------------
        ''' <summary>
        '''  Construct a empty Graph represented by a matrix.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal G As Graph, Optional ByVal t As matrixType = matrixType.laplacianMatrix, Optional ByVal p As Double = 0)
            MyBase.New(G, t, p)
        End Sub

        Public Sub New(ByVal gM As GH_graphMatrix)
            MyBase.New(gM)
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
            If GetType(graphMatrix).IsAssignableFrom(source.GetType) Then
                Dim gM As graphMatrix = DirectCast(source, graphMatrix)
                Me.gM = gM.getMatrix
                Me.MT = gM.type
                Return True
            ElseIf GetType(Graph).IsAssignableFrom(source.GetType) Then
                Dim G As Graph = DirectCast(source, Graph)
                Dim gM As New graphMatrix(G)
                Me.gM = gM.getMatrix
                Me.MT = gM.type
                Return True
            ElseIf GetType(GH_Matrix).IsAssignableFrom(source.GetType) Then
                Dim GHmatrix As GH_Matrix = DirectCast(source, GH_Matrix)
                Dim tmpMatrix As Matrix = GHmatrix.ScriptVariable()
                If Not tmpMatrix.IsSquare Then
                    Return False
                End If
                Me.MT = GH_graphMatrix.matrixType.weightedAdjacencyMatirx
                Me.gM = Matrix(Of Double).Build.Dense(tmpMatrix.RowCount, tmpMatrix.ColumnCount)
                For i As Integer = 0 To Me.dimension - 1
                    For j As Integer = 0 To Me.dimension - 1
                        Me.gM(i, j) = tmpMatrix.Item(i, j)
                    Next
                Next
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
            If (GetType(T).IsAssignableFrom(GetType(GH_graphMatrix))) Then
                Dim tmpGraphMatrix As Object = Me
                target = DirectCast(tmpGraphMatrix, T)
                Return True
            ElseIf (GetType(T).IsAssignableFrom(GetType(GH_Matrix))) Then
                Dim tmpMatrix As New Matrix(Me.dimension, Me.dimension)
                For i As Integer = 0 To Me.dimension - 1
                    For j As Integer = 0 To Me.dimension - 1
                        tmpMatrix.Item(i, j) = Me.gM(i, j)
                    Next
                Next
                Dim retObj As Object = New GH_Matrix(tmpMatrix)
                target = DirectCast(retObj, T)
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
            Return New GH_graphMatrix(Me)
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
            Return New graphMatrix(Me)
        End Function
        ''' <summary>
        ''' Creates a string description of the current instance value 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToStringGH() As String Implements IGH_Goo.ToString
            Return "GH_" & Me.toString()
        End Function

        ''' <summary>
        ''' Gets the TypeDescription
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeDescription As String Implements IGH_Goo.TypeDescription
            Get
                Return "graphMatrix representation"
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
                Return "GH_graphMatrix"
            End Get
        End Property

    End Class
End Namespace