Option Explicit On

Imports Grasshopper.Kernel.Types

Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Namespace GH_graphRepresentaions
    ''' -------------------------------------------
    ''' Project : GH_SpiderWebLibrary
    ''' Class   : GH_graphProperties
    ''' 
    ''' <summary>
    ''' Graph Representation Properties
    ''' </summary>
    ''' <remarks>
    ''' Settings for the Graph Display
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   13/07/2015 created
    ''' </history>
    ''' 
    Public Class GH_graphStyle
        Implements IGH_Goo

        Private C As Drawing.Color
        Private txtS As Double

        ' ------------------------- Constructor ----------------------
        ''' <summary>
        ''' Construct an New Graph Properties Object
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Me.C = Drawing.Color.DeepPink
            Me.txtS = 1.0
        End Sub

        Public Sub New(ByVal C As Drawing.Color, ByVal txtS As Double)
            Me.C = C
            Me.txtS = txtS
        End Sub

        ''' <summary>
        ''' Construct a New Graph Properties Object from another Graph Properties Object
        ''' </summary>
        ''' <param name="gP"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal gP As GH_graphStyle)
            Me.C = gP.C
            Me.txtS = gP.txtS
        End Sub


        ' ------------------------- GH_Property ----------------------

        Public Property txtSize As Double
            Get
                Return Me.txtS
            End Get
            Set(value As Double)
                Me.txtS = value
            End Set
        End Property


        Public Property displayColor As Drawing.Color
            Get
                Return Me.C
            End Get
            Set(value As Drawing.Color)
                Me.C = value
            End Set
        End Property


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
            If GetType(GH_graphStyle).IsAssignableFrom(source.GetType) Then
                Dim gP As GH_graphStyle = DirectCast(source, GH_graphStyle)
                Me.C = gP.C
                Me.txtS = gP.txtS
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
            If (GetType(T).IsAssignableFrom(GetType(GH_graphStyle))) Then
                Dim pG As Object = Me
                target = DirectCast(pG, T)
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
            Return New GH_graphStyle(Me)
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
            Return New GH_graphStyle(Me)
        End Function
        ''' <summary>
        ''' Creates a string description of the current instance value 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GHToString() As String Implements IGH_Goo.ToString
            Return "GH_graphStyle, " & Me.C.ToString() & ", txtSize " & Me.txtS
        End Function

        ''' <summary>
        ''' Gets the TypeDescription
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeDescription As String Implements IGH_Goo.TypeDescription
            Get
                Return "Settings How to Represent a Graph"
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
                Return "GH_graphStyle"
            End Get
        End Property
    End Class
End Namespace